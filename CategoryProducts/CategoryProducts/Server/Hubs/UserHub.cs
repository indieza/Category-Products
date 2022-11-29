namespace CategoryProducts.Server.Hubs
{
    using CategoryProducts.Constraints;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.SignalR;

    public class UserHub : Hub
    {
        public async Task UpdateUserInfo(UserViewModel user)
        {
            await this.Clients.All.SendAsync(HubConstants.ReceiveUpdateUserInfo, user);
        }
    }
}