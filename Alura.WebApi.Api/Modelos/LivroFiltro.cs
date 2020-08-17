using Alura.ListaLeitura.Modelos;
using System.Linq;

namespace Alura.WebApi.Api.Modelos
{
    public static class LivrosFiltroExtensions
    {
        public static IQueryable<Livro> AplicarFiltro(this IQueryable<Livro> query, LivroFiltro filtro)
        {

            if (!string.IsNullOrEmpty(filtro.Titulo))
            {
                query = query.Where(x => x.Titulo.Contains(filtro.Titulo));
            }

            if (!string.IsNullOrEmpty(filtro.Autor))
            {
                query = query.Where(x => x.Autor.Contains(filtro.Autor));
            }

            if (!string.IsNullOrEmpty(filtro.Subtitulo))
            {
                query = query.Where(x => x.Subtitulo.Contains(filtro.Subtitulo));
            }

            if (!string.IsNullOrEmpty(filtro.Lista))
            {
                query = query.Where(x => x.Lista == filtro.Lista.ParaTipo());
            }

            return query;
        }
    }

    public class LivroFiltro
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Autor { get; set; }
        public string Lista { get; set; }
    }
}


