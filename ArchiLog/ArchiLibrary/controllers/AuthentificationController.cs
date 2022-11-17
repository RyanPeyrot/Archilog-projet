using ArchiLibrary.Authentification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ArchiLibrary.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/[controller]/v{version:apiVersion}/")]
    public class AuthentificationController : ControllerBase
    {
        private readonly IJwtAuthentificationServices _JwtAuthentificationServices;
        private readonly IConfiguration _config;

        public AuthentificationController(IJwtAuthentificationServices JwtAuthentificationServices, IConfiguration config)
        {
            _JwtAuthentificationServices = JwtAuthentificationServices;
            _config = config;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login()
        {
            var token = _JwtAuthentificationServices.GenerateToken(_config["Jwt:Key"]);
            return Ok(token);
        }
    }
}
