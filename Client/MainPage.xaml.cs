using Common;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace Client;

public partial class MainPage : ContentPage
{
    private readonly IHttpClientFactory httpClientFactory;
    private ObservableCollection<ToDoDto> toDoCollection = new ObservableCollection<ToDoDto>();

    // Csak localhost
    private const string ApiBaseUrl = "https://localhost:7241/";

    public MainPage(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        this.httpClientFactory = httpClientFactory;
        ToDosView.ItemsSource = toDoCollection;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();
            var toDos = await httpClient.GetFromJsonAsync<List<ToDoDto>>(ApiBaseUrl + "list");

            toDoCollection.Clear();
            if (toDos != null)
            {
                foreach (var toDo in toDos)
                    toDoCollection.Add(toDo);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", "Nem sikerült betölteni az adatokat:\n" + ex.Message, "OK");
        }
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
                await LoadDataAsync();
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
}
