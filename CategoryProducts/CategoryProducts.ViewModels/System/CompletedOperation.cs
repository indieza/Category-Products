using Microsoft.JSInterop;

namespace CategoryProducts.ViewModels.System
{
    public class CompletedOperation<T>
    {
        public string Key { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public T Response { get; set; }

        public async Task DisplayMessageAsync(IJSRuntime jsRuntime)
        {
            switch (this.Key)
            {
                case "Warning":
                    await jsRuntime.InvokeVoidAsync("DisplayWaringNotification", this.Title, this.Message);
                    break;

                case "Success":
                    await jsRuntime.InvokeVoidAsync("DisplaySuccessfulNotification", this.Title, this.Message);
                    break;

                case "Info":
                    await jsRuntime.InvokeVoidAsync("DisplayInfoNotification", this.Title, this.Message);
                    break;

                case "Error":
                    await jsRuntime.InvokeVoidAsync("DisplayErrorNotification", this.Title, this.Message);
                    break;

                default:
                    break;
            }
        }
    }
}