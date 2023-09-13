using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace ParkingManagement.Client
{

        public interface IApiClient
        {
            Task<TResult?> GetFromJsonAsync<TResult>(string? requestUri);
            Task<HttpResponseMessage> PostAsJsonAsync<T>(string? requestUri, T value);
        }


        public class ApiClient : IApiClient
    {
            private readonly HttpClient http;

            public ApiClient()
            {
                this.http = new HttpClient();
                this.http.BaseAddress = new Uri("https://localhost:5010/");
            }

          
            public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string? requestUri, T value)
            {
                var response =  await http.PostAsJsonAsync(requestUri, value);
                return response;
            }


            public async Task<TResult?> GetFromJsonAsync<TResult>(string? requestUri)
                {
                    var response = await http.GetAsync(requestUri);

                    if (!response.IsSuccessStatusCode) return default;

                    var result = await response.Content.ReadFromJsonAsync<TResult>();
                    return result;
                }
        }

   
}
