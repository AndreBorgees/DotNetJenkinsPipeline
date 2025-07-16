using DotNetJenkinsPipeline.API.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace DotNetJenkinsPipeline.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CpfController : ControllerBase
    {
        [HttpGet("validar")]
        public IActionResult Validar(string cpf)
        {
            var resultado = Cpf.Criar(cpf);

           return Ok(resultado);
        }
    }
}
