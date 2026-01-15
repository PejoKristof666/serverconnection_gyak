using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace serverconnection_gyak
{
    public class ServerConnection
    {
        private HttpClient _client = new HttpClient();
        string baseUrl = "";

        private string Token { get; set; }
        public ServerConnection(string url)
        {
            if (!url.StartsWith("http://")) throw new ArgumentException("Hibás url (http://)");
            baseUrl = url;
            _client.BaseAddress = new Uri(baseUrl); 
        }

        public async Task<Message> registerUser(string username, string password)
        {
            Message msg = new Message();
            string url = baseUrl + "/artworks";
            try
            {
                var JsonData = new
                {
                    username = username,
                    password = password
                };
                string jsonString = JsonSerializer.Serialize(JsonData);
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                msg = JsonSerializer.Deserialize<Message>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return msg;
        }

        public async Task<Message> loginUser(string username, string password)
        {
            Message msg = new Message();
            string url = baseUrl + "/artworks";
            try
            {
                var JsonData = new
                {
                    username = username,
                    password = password
                };

                string jsonString = JsonSerializer.Serialize(JsonData);
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                msg = JsonSerializer.Deserialize<Message>(responseBody);

                Token = msg.token;
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return msg;
        }

        public async Task<List<artwork>> getArtworks()
        {
            List<artwork> ListOfArtworks = new List<artwork>();
            string url = baseUrl + "/artworks";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                ListOfArtworks = JsonSerializer.Deserialize<List<artwork>>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ListOfArtworks;
        }

        public async Task<Message> createArtwork(string title, int value)
        {
            Message msg = new Message();
            string url = baseUrl + "/artworks";
            if(string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("User is not logged in");
            }
            try
            {
                var JsonData = new
                {
                    title = title,
                    value = value
                };
                string jsonString = JsonSerializer.Serialize(JsonData);
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                msg = JsonSerializer.Deserialize<Message>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return msg;
        }
    }
}
