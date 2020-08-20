using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.Api.Controllers
{
    public static class ListaLeituraRepository
    {
        public static Lista CriaLista(TipoListaLeitura tipo, IRepository<Livro> repo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = repo.All
                    .Where(l => l.Lista == tipo)
                    .Select(l => l.ToApi())
                    .ToList()
            };
        }
    }


    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ListasLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }


        [HttpGet]
        public IActionResult TodasListas()
        {
            Lista paraLer = ListaLeituraRepository.CriaLista(TipoListaLeitura.ParaLer, _repo);
            Lista lendo = ListaLeituraRepository.CriaLista(TipoListaLeitura.Lendo, _repo);
            Lista lidos = ListaLeituraRepository.CriaLista(TipoListaLeitura.Lidos, _repo);

            var colecao = new List<Lista> { paraLer, lendo, lidos };
            return Ok(colecao);
        }

        [HttpGet("{tipo}")]
        public IActionResult Lista(TipoListaLeitura tipo)
        {
            //var header = this.HttpContext.Request.Headers;

            //if ((!header.ContainsKey("authorization")) || !(header["authorization"] == "123"))
            //{
            //    return StatusCode(401);
            //}

            var lista = ListaLeituraRepository.CriaLista(tipo, _repo);
            return Ok(lista);
        }

    }
}
