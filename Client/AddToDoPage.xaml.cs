using Common;
using System.Net.Http.Json;

namespace Client;

public partial class AddToDoPage : ContentPage
{
    private readonly IHttpClientFactory httpClientFactory;
    private const string ApiBaseUrl = "https://localhost:7241/";

    public AddToDoPage(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        this.httpClientFactory = httpClientFactory;
    }

    private async void OnSaveClickedAsync(object sender, EventArgs e)
    {
        try
        {
            var newToDo = new ToDoDto
            {
                Title = TitleEntry.Text,
                Description = DescriptionEditor.Text,
                Created = DateTime.Now,
                Deadline = DeadlinePicker.Date,
                IsReady = IsReadySwitch.IsToggled
            };

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync(ApiBaseUrl + "create", newToDo);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Siker", "�j ToDo l�trehozva!", "OK");
                await Shell.Current.GoToAsync(".."); // vissza a list�hoz
            }
            else
            {
                await DisplayAlert("Hiba", "Nem siker�lt l�trehozni a ToDo-t", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", ex.Message, "OK");
        }
    }

    private async void OnCancelClickedAsync(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage"); // vissza a list�hoz
    }
}
