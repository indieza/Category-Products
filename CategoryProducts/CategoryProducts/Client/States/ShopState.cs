namespace CategoryProducts.Client.States
{
    using Blazored.LocalStorage;

    using CategoryProducts.Constraints;
    using CategoryProducts.ExtensionMethods;
    using CategoryProducts.InputModels.Shop;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.ViewModels.Shop;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Localization;
    using Microsoft.JSInterop;

    using System.Net.Http.Headers;
    using System.Net.Http.Json;

    public class ShopState
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly NavigationManager navManager;
        private readonly IJSRuntime jsRuntime;
        private HubConnection? hubConnection;

        public ShopState(HttpClient client, ILocalStorageService localStorage, IStringLocalizer<Resource> localizer, NavigationManager navManager, IJSRuntime jsRuntime)
        {
            this.httpClient = client;
            this.localStorage = localStorage;
            this.localizer = localizer;
            this.navManager = navManager;
            this.jsRuntime = jsRuntime;

            this.hubConnection = null;

            this.Categories = new List<CategoryViewModel>();
            this.CategoryInputModel = new CategoryInputModel();
        }

        public event Action OnChange;

        public List<CategoryViewModel> Categories { get; set; }

        public CategoryInputModel CategoryInputModel { get; set; }

        public async Task GetAllCategories()
        {
            var response = await this.httpClient.GetAsync($"api/Shop/GetAllCategories");
            var result = await response.SetupApiResponse<List<CategoryViewModel>?>(this.localizer);

            if (result.Key == "Success")
            {
                this.Categories = result.Response;
            }
            else
            {
                this.Categories = new List<CategoryViewModel>();
                await result.DisplayMessageAsync(this.jsRuntime);
            }

            this.NotifyStateChanged();
        }

        public async Task AddCategory()
        {
            var token = await this.localStorage.GetItemAsync<string>(AppConstants.LocalStorageAuthToken);
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AppConstants.AppAuhtHeader, token);
            var response = await this.httpClient.PostAsJsonAsync($"api/Shop/AddCategory", this.CategoryInputModel);
            var result = await response.SetupApiResponse<CategoryViewModel?>(this.localizer);

            if (result.Key == "Success")
            {
                await this.hubConnection.SendAsync(
                    HubConstants.AddCategory,
                    result.Response);
                await result.DisplayMessageAsync(this.jsRuntime);
                this.CategoryInputModel = new CategoryInputModel();
            }
            else
            {
                this.CategoryInputModel = new CategoryInputModel();
                await result.DisplayMessageAsync(this.jsRuntime);
            }

            this.httpClient.DefaultRequestHeaders.Authorization = null;
            this.NotifyStateChanged();
        }

        public async Task SetupHubConnectionAsync()
        {
            if (this.hubConnection == null)
            {
                this.hubConnection = new HubConnectionBuilder()
                    .WithUrl(this.navManager.ToAbsoluteUri("/shophub"))
                    .Build();

                this.hubConnection.On<CategoryViewModel>(HubConstants.ReceiveAddCategory, (info) =>
                {
                    this.Categories.Insert(0, info);
                    this.NotifyStateChanged();
                });

                this.hubConnection.On<CategoryViewModel>(HubConstants.ReceiveUpdateCategory, (info) =>
                {
                    var target = this.Categories.FirstOrDefault(x => x.Id == info.Id);
                    if (target != null)
                    {
                        var index = this.Categories.IndexOf(target);

                        if (index != -1)
                        {
                            this.Categories[index] = info;
                            this.NotifyStateChanged();
                        }
                    }
                });

                await this.hubConnection.StartAsync();
            }
        }

        public async Task DisposeHubConnection()
        {
            if (this.hubConnection != null)
            {
                await this.hubConnection.DisposeAsync();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}