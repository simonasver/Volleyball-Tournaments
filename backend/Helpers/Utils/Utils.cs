using System.Net;

namespace Backend.Helpers.Utils;

public class Utils
{
    public static async Task<bool> IsLinkImage(string imageUrl)
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(imageUrl);
            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (!string.IsNullOrEmpty(contentType) && contentType.StartsWith("image/"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}