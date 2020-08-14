using Alura.ListaLeitura.Modelos;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<LivroApi> GetLivroAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetLivroCapaAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"livros/{id}/capa");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsByteArrayAsync();
        }
    }
}
