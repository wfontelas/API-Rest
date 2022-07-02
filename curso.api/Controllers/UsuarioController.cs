using curso.api.Business.Entities;
using curso.api.Business.Repositories;
using curso.api.Configurations;
using curso.api.Filters;
using curso.api.Infraestruture.Data;
using curso.api.Models;
using curso.api.Models.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace curso.api.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
                private readonly IAuthenticationService _authenticationService;
        

        public UsuarioController(
            IUsuarioRepository usuarioRepository, 
            IAuthenticationService authenticationService)
        {
            _usuarioRepository = usuarioRepository;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Este serviço permite autenticar um usuário cadastrado e ativo.
        /// </summary>
        /// <param name="loginViewModelInput">View Model do login</param>
        /// <returns>Retorna status Ok, dados do usuário e o token em caso de sucesso</returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao autenticar", Type = typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos Obrigatórios", Type = typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro interno", Type =typeof(ErroGenericoViewModel))]
        [HttpPost]
        [Route("logar")]
        [ValidacaoModelStateCustomizado]
        public IActionResult Logar(LoginViewModelInput loginViewModelInput)
        {
           var usuario = _usuarioRepository.ObterUsuario(loginViewModelInput.Login);

            if (usuario ==null)
            {
                return BadRequest("Houve um erro ao tentar acessar.");
            }
            
            var usuarioViewModelOutput = new UsuarioViewModelOutput()
            {
                Codigo = 1,
                Login = "WilliamFontelas",
                Email = "email@email.com"
            };
                      
            var token = _authenticationService.GerarToken(usuarioViewModelOutput);

            return Ok(new
            {   
                Token = token,
                Usuario = usuarioViewModelOutput
            });

            //return Ok(loginViewModelInput);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginViewModelInput">View Model do registro de login</param>
        /// <returns></returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao autenticar", Type = typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos Obrigatórios", Type = typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro interno", Type = typeof(ErroGenericoViewModel))]
        [HttpPost]
        [Route("registrar")]
        public IActionResult Registrar(RegistroViewModelInput loginViewModelInput)
        {
            //var optionsBuilder = new DbContextOptionsBuilder<CursoDbContext>();
            //optionsBuilder.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True");

            //CursoDbContext contexto = new CursoDbContext(optionsBuilder.Options);

            //var migracoesPendentes = contexto.Database.GetPendingMigrations();

            //if (migracoesPendentes.Count() > 0)
            //{
            //    contexto.Database.Migrate();
            //}

            var usuario = new Usuario();
            usuario.Login = loginViewModelInput.Login;
            usuario.Senha = loginViewModelInput.Senha;
            usuario.Email = loginViewModelInput.Email;
            

            _usuarioRepository.Adicionar(usuario);
            _usuarioRepository.Commit();

            return Created("", loginViewModelInput);
        }
    }
}
