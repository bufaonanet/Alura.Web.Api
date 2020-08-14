using Alura.ListaLeitura.Modelos;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            var resposta = await _httpClient.GetAsync($"ListasLeitura/{tipo}");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsAsync<Lista>();
        }

        public async Task DeleteLivro(int id)
        {
            var respota = await _httpClient.DeleteAsync($"livros/{id}");
            respota.EnsureSuccessStatusCode();
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
        private string EnvolvoComAspasDuplas(string valor)
        {
            return $"\"{valor}\"";
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), EnvolvoComAspasDuplas("titulo"));
            content.Add(new StringContent(model.Subtitulo), EnvolvoComAspasDuplas("subtitulo"));

            if (!string.IsNullOrEmpty(model.Resumo))
                content.Add(new StringContent(model.Resumo), EnvolvoComAspasDuplas("resumo"));

            if (!string.IsNullOrEmpty(model.Autor))
                content.Add(new StringContent(model.Autor), EnvolvoComAspasDuplas("autor"));

            if (!string.IsNullOrEmpty(model.Lista.ParaString()))
                content.Add(new StringContent(model.Lista.ParaString()), EnvolvoComAspasDuplas("lista"));

            if (model.Id > 0)
                content.Add(new StringContent(model.Id.ToString()), EnvolvoComAspasDuplas("id"));

            if (model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imagemContent,
                    EnvolvoComAspasDuplas("capa"),
                    EnvolvoComAspasDuplas("capa.png")
                );
            }

            return content;
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync("livros", content);
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync("livros", content);
            resposta.EnsureSuccessStatusCode();
        }

    }
}
