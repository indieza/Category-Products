namespace CategoryProducts.Client
{
    using Blazored.LocalStorage;

    using CategoryProducts.Constraints;
    using CategoryProducts.ExtensionMethods;
    using CategoryProducts.InputModels.User;
    using CategoryProducts.Resources;
    using CategoryProducts.Shared.Jwt;
    using CategoryProducts.ViewModels.Auth;
    using CategoryProducts.ViewModels.System;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.Localization;
    using Microsoft.JSInterop;

    using System.Net.Http.Json;
    using System.Security.Claims;

    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly IJSRuntime jsRuntime;
        private readonly AuthenticationState anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IStringLocalizer<Resource> localizer, IJSRuntime jsRuntime)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.localizer = localizer;
            this.jsRuntime = jsRuntime;
            this.anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            this.CurrentUser = new CurrentUserViewModel();
        }

        public event Action OnChange;

        public CurrentUserViewModel CurrentUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await this.localStorage.GetItemAsync<string>(AppConstants.LocalStorageAuthToken);
            if (string.IsNullOrWhiteSpace(token))
            {
                return this.anonymous;
            }

            var claims = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            this.SetupCurrentUser(claims);
            return new AuthenticationState(claims);
        }

        public void NotifyUserAuthentication(string email)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            this.NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(this.anonymous);
            this.NotifyAuthenticationStateChanged(authState);
        }

        public async Task<CompletedOperation<string?>> LoginAsync(LoginInputModel model)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/User/Login", model);
            var result = await response.SetupApiResponse<string?>(this.localizer);

            if (result.Key == "Success")
            {
                await this.localStorage.SetItemAsync(AppConstants.LocalStorageAuthToken, result.Response);
                this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
                return result;
            }

            return result;
        }

        public async Task<CompletedOperation<LoginInputModel?>> RegisterAsync(RegisterInputModel model)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/User/Register", model);
            var result = await response.SetupApiResponse<LoginInputModel?>(this.localizer);
            if (result.Key == "Success")
            {
                await this.LoginAsync(result.Response);
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await this.localStorage.RemoveItemAsync(AppConstants.LocalStorageAuthToken);
            this.NotifyUserLogout();
        }

        public void SetupCurrentUser(ClaimsPrincipal claims)
        {
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var username = claims.FindFirst(ClaimTypes.Name).Value;

            this.CurrentUser = new CurrentUserViewModel()
            {
                Id = userId,
                UserName = username,
            };
            this.NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}