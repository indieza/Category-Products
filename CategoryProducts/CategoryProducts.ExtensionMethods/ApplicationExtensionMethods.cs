using CategoryProducts.Resources;
using CategoryProducts.ViewModels.System;

using Microsoft.Extensions.Localization;

using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CategoryProducts.ExtensionMethods
{
    public static class ApplicationExtensionMethods
    {
        public static bool IsPropertyRequired<T>(this T model, string propertyName)
        where T : class
        {
            var attribute = model?.GetType()
                .GetProperty(propertyName)?
                .GetCustomAttribute<RequiredAttribute>();

            return attribute != null;
        }

        public static async Task<CompletedOperation<T>> SetupApiResponse<T>(this HttpResponseMessage? response, IStringLocalizer<Resource> localizer)
            where T : class
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await response.Content.ReadFromJsonAsync<CompletedOperation<T>>();

                case HttpStatusCode.BadRequest:
                    return await response.Content.ReadFromJsonAsync<CompletedOperation<T>>();

                case HttpStatusCode.Unauthorized:
                    return await response.Content.ReadFromJsonAsync<CompletedOperation<T>>();

                case HttpStatusCode.TooManyRequests:
                    return new CompletedOperation<T>()
                    {
                        Key = "Error",
                        Title = localizer["Error"],
                        Message = localizer["You try to perform too many requests"],
                    };

                default:
                    return new CompletedOperation<T>()
                    {
                        Key = "Error",
                        Title = localizer["Error"],
                        Message = response.StatusCode.ToString().SplitCamelCase(),
                    };
            }
        }

        public static string SplitCamelCase(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(source, pattern);
            return string.Join(" ", matches.Select(x => x.Value).ToList());
        }
    }
}