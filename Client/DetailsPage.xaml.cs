using Common;
using System.Net.Http.Json;

namespace Client;

public partial class DetailsPage : ContentPage, IQueryAttributable
{
    private readonly IHttpClientFactory httpClientFactory;
    private ToDoDto toDo;
    private int id;

    public DetailsPage(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        InitializeComponent();
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
        BindData();
    }

    private async ValueTask LoadDataAsync()
    {
        if (id == 0)
        {
            toDo = new ToDoDto { Title = "Ez egy új todo" };
        }
        else
        {
            var httpClient = httpClientFactory.CreateClient();
            toDo = await httpClient.GetFromJsonAsync<ToDoDto>($"https://localhost:7241/get/{id}");
        }
    }

    private void BindData()
    {
        if (toDo == null) return;
        TitleEntry.Text = toDo.Title;
        DescriptionEditor.Text = toDo.Description;
        DeadlinePicker.Date = toDo.Deadline;
        IsReadySwitch.IsToggled = toDo.IsReady;
        TodoId.Text = toDo.Id.ToString();
    }

    private async void OnSaveClickedAsync(object sender, EventArgs e)
    {
        try
        {
            var updatedToDo = new ToDoDto
            {
                Id = id,
                Title = TitleEntry.Text,
                Description = DescriptionEditor.Text,
                Created = toDo.Created,
                Deadline = DeadlinePicker.Date,
                IsReady = IsReadySwitch.IsToggled
            };

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PutAsJsonAsync("https://localhost:7241/update", updatedToDo);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Siker", "ToDo frissítve!", "OK");
                await Shell.Current.GoToAsync("..");
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
}