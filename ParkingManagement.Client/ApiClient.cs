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
            Uri BaseAddress { get; set; }
            Task<TResult?> PostAsync<TResult>(string? requestUri, TResult value);
    }


        public class ApiClient : IApiClient
    {
            private readonly HttpClient http;
        public Uri BaseAddress { get; set; } = new Uri("https://localhost:5010/");

        public ApiClient()
            {
                this.http = new HttpClient();
                this.http.BaseAddress = BaseAddress;
            }

          
            public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string? requestUri, T value)
            {
                var response =  await http.PostAsJsonAsync(requestUri, value);
                return response;
            }


            public async Task<TResult?> PostAsync<TResult>(string? requestUri, TResult value)
            {

                string json = JsonConvert.SerializeObject(value);   

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await http.PostAsync(requestUri, httpContent);

                if (!response.IsSuccessStatusCode) return default;

                var result = await response.Content.ReadFromJsonAsync<TResult?>();

                 return result;
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
