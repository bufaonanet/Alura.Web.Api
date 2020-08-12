using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Api.Formatters
{
    public class LivrosCsvFormatters : TextOutputFormatter
    {
        public LivrosCsvFormatters()
        {
            var csvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var applicationMediaType = MediaTypeHeaderValue.Parse("application/csv");
            SupportedMediaTypes.Add(csvMediaType);
            SupportedMediaTypes.Add(applicationMediaType);
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {

            return type == typeof(LivroApi);
        }


        public override Task WriteResponseBodyAsync(
            OutputFormatterWriteContext context,
            Encoding selectedEncoding)
        {
            var livroEmCsv = "";

            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;
                livroEmCsv = $"{livro.Autor};{livro.Subtitulo};{livro.Autor};{livro.Lista}";
            }

            using (var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(livroEmCsv);
            }//escritor.close();            
        }
    }
}
