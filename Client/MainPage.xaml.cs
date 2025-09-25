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

    private const string ApiBaseUrl = "https://localhost:7241/";
    private bool showPendingOnly = false;

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
    private async void OnTogglePendingToggled(object sender, ToggledEventArgs e)
    {
        showPendingOnly = e.Value; 
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
        await Shell.Current.GoToAsync("AddToDoPage");
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
        var originalValue = !e.Value;

        checkBox.IsEnabled = false;

        try
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            var url = ApiBaseUrl + "update"; // csak /update van
            var response = await httpClient.PutAsJsonAsync(url, toDo);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Hiba",
                    $"Nem sikerült frissíteni a státuszt\n" +
                    $"Kód: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                    $"Válasz: {msg}",
                    "OK");

                toDo.IsReady = originalValue; // visszaállítás
            }
        }
        catch (Exception ex)
        {
            toDo.IsReady = originalValue;
            await DisplayAlert("Hiba", "Kivétel: " + ex.Message, "OK");
        }
        finally
        {
            checkBox.IsEnabled = true;
        }
    }


}