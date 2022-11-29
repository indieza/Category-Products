namespace CategoryProducts.Client.States
{
    using Azure;

    using Blazored.LocalStorage;

    using CategoryProducts.Constraints;
    using CategoryProducts.ExtensionMethods;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.ViewModels.System;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Localization;
    using Microsoft.JSInterop;

    using System;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class UserAppState
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly NavigationManager navManager;
        private readonly IJSRuntime jsRuntime;
        private HubConnection? hubConnection;

        public UserAppState(HttpClient client, ILocalStorageService localStorage, IStringLocalizer<Resource> localizer, NavigationManager navManager, IJSRuntime jsRuntime)
        {
            this.httpClient = client;
            this.localStorage = localStorage;
            this.localizer = localizer;
            this.navManager = navManager;
            this.jsRuntime = jsRuntime;
            this.UserProfile = new UserViewModel();
            this.EditUserInputModel = new EditUserInputModel();

            this.hubConnection = null;
        }

        public event Action OnChange;

        public UserViewModel UserProfile { get; set; }

        public EditUserInputModel EditUserInputModel { get; set; }

        public async Task<CompletedOperation<string?>> ChangeAppLanguage(string cultureName)
        {
            var result = await this.httpClient
                .GetFromJsonAsync<CompletedOperation<string?>>($"api/Culture/SetCulture/{cultureName}");
            return result;
        }

        public void SetupEditUserInputModel()
        {
            this.EditUserInputModel = new EditUserInputModel()
            {
                FirstName = this.UserProfile.FirstName,
                LastName = this.UserProfile.LastName,
            };
            this.NotifyStateChanged();
        }

        public async Task GetUserProfileAsync(string username, string currentUsername)
        {
            var response = await this.httpClient.GetAsync($"api/User/GetUserProfile/{username}");
            var result = await response.SetupApiResponse<UserViewModel?>(this.localizer);

            if (result.Key == "Success")
            {
                this.UserProfile = result.Response;
            }
            else
            {
                this.UserProfile = new UserViewModel();
                this.EditUserInputModel = new EditUserInputModel();
                this.navManager.NavigateTo("/");
                await result.DisplayMessageAsync(this.jsRuntime);
            }

            this.NotifyStateChanged();
        }

        public async Task<CompletedOperation<UserViewModel?>> EditUserProfileAsync()
        {
            var token = await this.localStorage.GetItemAsync<string>(AppConstants.LocalStorageAuthToken);
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AppConstants.AppAuhtHeader, token);
            var response = await this.httpClient.PostAsJsonAsync($"api/User/EditUserProfile", this.EditUserInputModel);
            var result = await response.SetupApiResponse<UserViewModel?>(this.localizer);

            if (result.Key == "Success")
            {
                await this.hubConnection.SendAsync(
                    HubConstants.UpdateUserInfo,
                    result.Response);
                this.EditUserInputModel = new EditUserInputModel();
            }

            this.NotifyStateChanged();
            return result;
        }

        public async Task SetupHubConnectionAsync()
        {
            if (this.hubConnection == null)
            {
                this.hubConnection = new HubConnectionBuilder()
                    .WithUrl(this.navManager.ToAbsoluteUri("/userhub"))
                    .Build();

                this.hubConnection.On<UserViewModel>(HubConstants.ReceiveUpdateUserInfo, (info) =>
                {
                    if (this.UserProfile.Id == info.Id)
                    {
                        this.UserProfile = info;
                        this.NotifyStateChanged();
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