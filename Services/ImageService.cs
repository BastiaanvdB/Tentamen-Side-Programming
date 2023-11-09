using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Services
{
    public class ImageService : IImageService
    {
        private static readonly HttpClient _httpClient;
        private readonly string _url = "https://picsum.photos/640/640";

        static ImageService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<byte[]> GetImageAsync()
        {
            using var response = await _httpClient.GetAsync(_url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            throw new HttpRequestException($"Failed to get the image. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }
    }
}
