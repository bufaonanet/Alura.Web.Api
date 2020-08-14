using System.Linq;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alura.ListaLeitura.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repo)
        {
            _repo = repo;
        }

        [HttpGet()]
        [Route("{id:int}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);

            if (model == null)
                return NotFound();

            return Ok(model.ToApi());
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

        [HttpGet()]
        [Route("")]
        public IActionResult RecuperarTodos()
        {
            var model = _repo.All.Select(x => x.ToApi()).ToList();

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Novo([FromBody] LivroUpload model)
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
        public IActionResult Alterar([FromBody] LivroUpload model)
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

        [HttpDelete()]
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
