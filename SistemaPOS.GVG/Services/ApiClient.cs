using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SistemaPOS.Desktop.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        // Importante: Cambia este puerto por el que Visual Studio te asignó en Swagger
        private readonly string _baseUrl = "http://localhost:5000/api/";

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }
        
        // Método genérico para consultar datos (Ej. Listar inventario)
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        // Método genérico para enviar datos (Ej. Registrar nueva venta)
        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }
    }
}