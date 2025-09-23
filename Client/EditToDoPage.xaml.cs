using Common;
using System.Net.Http.Json;

namespace Client;

public partial class EditToDoPage : ContentPage, IQueryAttributable
{
    private readonly IHttpClientFactory httpClientFactory;
    private ToDoDto toDo;
    private int id;
    private const string ApiBaseUrl = "https://localhost:7241/";

    public EditToDoPage(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        this.httpClientFactory = httpClientFactory;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        query.TryGetValue("Id", out var idObject);
        id = (int)(idObject ?? 0);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (id == 0)
            return;

        var httpClient = httpClientFactory.CreateClient();
        toDo = await httpClient.GetFromJsonAsync<ToDoDto>(ApiBaseUrl + $"get/{id}");

        if (toDo == null)
        {
            await DisplayAlert("Error", "ToDo data is not loaded.", "OK");
            return;
        }

        // Populate UI controls
        TitleEntry.Text = toDo.Title;
        DescriptionEditor.Text = toDo.Description;
        DeadlinePicker.Date = toDo.Deadline;
        IsReadySwitch.IsToggled = toDo.IsReady;
    }

    private async void OnSaveClickedAsync(object sender, EventArgs e)
    {
        if (toDo == null)
        {
            await DisplayAlert("Error", "ToDo data is not loaded.", "OK");
            return;
        }
        try
        {
            var updatedToDo = new ToDoDto
            {
                Id = id,
                Title = TitleEntry.Text,
                Description = DescriptionEditor.Text,
                Created = toDo.Created, // preserve original creation date
                Deadline = DeadlinePicker.Date,
                IsReady = IsReadySwitch.IsToggled
            };

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PutAsJsonAsync(ApiBaseUrl + "update", updatedToDo);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Siker", "ToDo frissítve!", "OK");
                var parameters = new Dictionary<string, object> { { "Id", toDo.Id } };
                await Shell.Current.GoToAsync("///edit", parameters);
            }
            else
            {
                await DisplayAlert("Hiba", "Nem sikerült frissíteni a ToDo-t", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", ex.Message, "OK");
        }
    }

    private async void OnCancelClickedAsync(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}