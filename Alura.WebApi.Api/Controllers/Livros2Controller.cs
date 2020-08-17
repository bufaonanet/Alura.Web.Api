using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebApi.Api.Modelos;

namespace Alura.WebApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/livros")]
    public class Livros2Controller : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public Livros2Controller(IRepository<Livro> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [HttpGet]
        [Route("{id:int}/capa")]
        public IActionResult Capa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        [Route("")]
        public IActionResult RecuperarTodos(
            [FromQuery] LivroFiltro filtro,
            [FromQuery] LivroOrdem ordem,
            [FromQuery] LivroPaginacao paginacao)
        {
            var livroPaginado = _repo
                .All
                .AplicarFiltro(filtro)
                .AplicarOrdem(ordem)
                .Select(x => x.ToApi())
                .ToLivroPaginado(paginacao);

            if (livroPaginado == null)
                return NotFound();

            return Ok(livroPaginado);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Novo([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);

                var url = Url.Action("Recuperar", new { id = livro.Id });
                return Created(url, livro); //retorna 201
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("")]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok();//retorna 200
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent();//retorna 204
        }
    }
}
