namespace CategoryProducts.Server.Hubs
{
    using CategoryProducts.Constraints;
    using CategoryProducts.ViewModels.Shop;
    using CategoryProducts.ViewModels.User;

    using Microsoft.AspNetCore.SignalR;

    public class ShopHub : Hub
    {
        public async Task AddCategory(CategoryViewModel category)
        {
            await this.Clients.All.SendAsync(HubConstants.ReceiveAddCategory, category);
        }
    }
}