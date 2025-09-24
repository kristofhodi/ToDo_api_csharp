using Common;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Client;

public partial class MainPage : ContentPage
{
    private readonly IHttpClientFactory httpClientFactory;
    private ObservableCollection<ToDoDto> toDoCollection = new ObservableCollection<ToDoDto>();

    // Csak localhost
    private const string ApiBaseUrl = "https://localhost:7241/";
    private bool showPendingOnly = false;

    // Only one LoadDataAsync method, with a parameter to control pending filtering
    private async Task LoadDataAsync(bool loadPendingOnly)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();
            string endpoint = loadPendingOnly ? "list/pending" : "list";
            var toDos = await httpClient.GetFromJsonAsync<List<ToDoDto>>(ApiBaseUrl + endpoint);

            toDoCollection.Clear();
            if (toDos != null)
            {
                foreach (var toDo in toDos)
                {
                    toDoCollection.Add(toDo);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", "Nem sikerült betölteni az adatokat:\n" + ex.Message, "OK");
        }
    }

    // Call this from a button or switch event handler:
    private async void OnTogglePendingClickedAsync(object sender, EventArgs e)
    {
        showPendingOnly = !showPendingOnly;
        await LoadDataAsync(showPendingOnly);
    }

    public MainPage(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        this.httpClientFactory = httpClientFactory;
        ToDosView.ItemsSource = toDoCollection;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync(showPendingOnly);
    }

    private async void OnAddNewClickedAsync(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///AddToDoPage");
    }

    private async void OnDeleteClickedAsync(object sender, EventArgs e)
    {
        try
        {
            var toDo = (ToDoDto)((Button)sender).BindingContext;
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(ApiBaseUrl + $"delete/{toDo.Id}");

            if (response.IsSuccessStatusCode)
                await LoadDataAsync(showPendingOnly);
            else
                await DisplayAlert("Hiba", "Nem sikerült törölni a feladatot", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", ex.Message, "OK");
        }
    }

    private async void OnEditClickedAsync(object sender, EventArgs e)
    {
        var toDo = (ToDoDto)((Button)sender).BindingContext;
        var parameters = new Dictionary<string, object> { { "Id", toDo.Id } };
        await Shell.Current.GoToAsync("details", parameters);
    }

    private async void OnStatusChangedAsync(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = (CheckBox)sender;
        var toDo = (ToDoDto)checkBox.BindingContext;
        var originalValue = toDo.IsReady;

        // lock UI amíg dolgozunk
        checkBox.IsEnabled = false;

        // optimisztikusan állítjuk be a modellben
        toDo.IsReady = e.Value;

        try
        {
            var httpClient = httpClientFactory.CreateClient();

            // Próbáljuk elsőként a /update/{id} végpontot
            var urlWithId = ApiBaseUrl + $"update/{toDo.Id}";
            var response = await httpClient.PutAsJsonAsync(urlWithId, toDo);

            if (!response.IsSuccessStatusCode)
            {
                // ha nem sikerült, próbáljuk meg a /update végpontot (sok projekt ezt használja)
                var urlNoId = ApiBaseUrl + "update";
                var body = JsonSerializer.Serialize(toDo);
                var altResponse = await httpClient.PutAsync(urlNoId, new StringContent(body, Encoding.UTF8, "application/json"));

                if (altResponse.IsSuccessStatusCode)
                {
                    // siker — frissítjük listát (biztonság kedvéért)
                    await LoadDataAsync(showPendingOnly);
                }
                else
                {
                    // mindkét próbálkozás hibára futott: olvassuk ki a szerverválaszt és mutassuk meg
                    var primaryText = await SafeReadContentAsync(response);
                    var altText = await SafeReadContentAsync(altResponse);

                    await DisplayAlert("Hiba",
                        $"Frissítés sikertelen.\nElső próbálkozás: {(int)response.StatusCode} {response.ReasonPhrase}\n{primaryText}\n\nMásodik próbálkozás: {(int)altResponse.StatusCode} {altResponse.ReasonPhrase}\n{altText}",
                        "OK");

                    // visszaállítjuk az eredeti értéket
                    toDo.IsReady = originalValue;
                    await LoadDataAsync(showPendingOnly);
                }
            }
            else
            {
                // siker: opcionálisan újratöltjük a listát, hogy minden mező szinkron legyen
                await LoadDataAsync(showPendingOnly);
            }
        }
        catch (Exception ex)
        {
            // hálózati vagy egyéb kivétel
            await DisplayAlert("Hiba", "Kivétel: " + ex.Message, "OK");

            // visszaállítjuk az eredeti értéket
            toDo.IsReady = originalValue;
            await LoadDataAsync(showPendingOnly);
        }
        finally
        {
            checkBox.IsEnabled = true;
        }

        // segédfüggvény a tartalom kiolvasására (nem dob, ha üres)
        static async Task<string> SafeReadContentAsync(HttpResponseMessage resp)
        {
            try
            {
                if (resp?.Content == null) return string.Empty;
                return await resp.Content.ReadAsStringAsync();
            }
            catch { return string.Empty; }
        }
    }
}