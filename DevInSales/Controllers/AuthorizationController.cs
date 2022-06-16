using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInSales.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet]
        [Route("anonimo")]
        [AllowAnonymous]
        public string Anonimo() => "Usuário Anônimo.";

        [HttpGet]
        [Route("autenticado")]
        public string Autenticar() => String.Format("Autenticado - {0} | Role - {1}", User.Identity.Name, User.IsInRole);

        [HttpGet]
        [Route("usuario")]
        [Authorize(Roles = "Usuario, Gerente, Administrador")]
        public string Usuario() => "Permissão para Usuário com sucesso.";

        [HttpGet]
        [Route("gerente")]
        [Authorize(Roles = "Gerente, Administrador")]
        public string Gerente() => "Permissão para Gerente com sucesso.";

        [HttpGet]
        [Route("administrador")]
        [Authorize(Roles = "Administrador")]
        public string Administrador() => "Permissão para Administrador com sucesso.";
    }
}
