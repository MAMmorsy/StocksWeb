using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using StocksWeb.ViewModels;

namespace StocksWeb.Models
{
    public static class Helper
    {
        
        public static async Task<Response<T>> GetAPIAsync<T>(string fullUrl,string accessToken, string sentObject="")
        {
            Response<T> response = new Response<T>();
            var client = new HttpClient();
            client.BaseAddress = new Uri(fullUrl);
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer "+ accessToken);
                httpClient.DefaultRequestHeaders.Add("sendData", sentObject);
                using (HttpResponseMessage result = await httpClient.GetAsync(client.BaseAddress.AbsoluteUri))
                {
                    apiResponse = await result.Content.ReadAsStringAsync();

                    response = JsonConvert.DeserializeObject<Response<T>>(apiResponse);
                }

            }
            return response;
        }

        
        public static async Task<Response<bool>> PostAPIAsync<T>(string fullUrl, T viewModel, string accessToken)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(fullUrl);

            Response<bool> response = new Response<bool>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                string serilaizedData = JsonConvert.SerializeObject(viewModel);

                StringContent data = new StringContent(serilaizedData, Encoding.UTF8, "application/json");

                using (HttpResponseMessage result = await httpClient.PostAsync(client.BaseAddress.AbsoluteUri, data))
                {
                    string apiResponse = await result.Content.ReadAsStringAsync();

                    response = JsonConvert.DeserializeObject<Response<bool>>(apiResponse);
                }
            }

            return response;
        }

        public static async Task<Response<Y>> PostAPIAsync<Y,T>(string fullUrl, T viewModel, string accessToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(fullUrl);
            Response<Y> response = new Response<Y>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                string serilaizedData = JsonConvert.SerializeObject(viewModel);
                StringContent data = new StringContent(serilaizedData, Encoding.UTF8, "application/json");
                using (HttpResponseMessage result = await httpClient.PostAsync(client.BaseAddress.AbsoluteUri, data))
                {
                    string apiResponse = await result.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<Response<Y>>(apiResponse);
                }
            }
            return response;
        }

    }
}
