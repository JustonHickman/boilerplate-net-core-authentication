using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using boilerplate.API.Data;
using boilerplate.API.DataObjectTransfer;
using boilerplate.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
//using boilerplate.API.Models;

namespace boilerplate.API.Controllers {
    [Route ("api/user")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public UsersController (IAuthRepository repo, IConfiguration config) {
            _config = config;
            _repo = repo;
        }

        // POST api/users/login
        [HttpPost ("login")]
        public async Task<IActionResult> Login (LoginUser loginUser) {
            loginUser.Username = loginUser.Username.ToLower ();
            var user = await _repo.Login (loginUser.Username, loginUser.Password);

            if (user == null) {
                return Unauthorized ("Username or Password is incorrect");
            }

            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),

                new Claim (ClaimTypes.GivenName, user.Username)
            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection("AppSettings:JWTSecretKey").Value));

            var authIt = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDecoder = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = authIt
            };

            var jwtHandler = new JwtSecurityTokenHandler();

            var token = jwtHandler.CreateToken(tokenDecoder);
            var savedToken =  new {
                token = jwtHandler.WriteToken(token)
            };

            return Ok($"User has successfully logged in and the token is Bearer {savedToken.token}");
        }

        // POST api/users/register
        [HttpPost ("register")]
        public async Task<IActionResult> Register (RegisterUser registerUser) {
            registerUser.Username = registerUser.Username.ToLower ().TrimEnd ();
            registerUser.FirstName = registerUser.FirstName.TrimEnd ();
            registerUser.Email = registerUser.Email.TrimEnd ();
            registerUser.LastName = registerUser.LastName.TrimEnd ();

            if (await _repo.IsUser (registerUser.Username)) {
                return BadRequest ("User already exists");
            }

            if (await _repo.IsEmail (registerUser.Username)) {
                return BadRequest ("Email address is associated with another account");
            }

            var createUser = new User {
                Username = registerUser.Username,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email
            };
            var user = await _repo.RegisterAsync (createUser, registerUser.Password);

            return StatusCode (201);
        }

        // PUT api/users/5
        [HttpPut ("{id}")]
        public void Put (int id, [FromBody] User user) { 
           
        }

        // DELETE api/users/5
        [HttpDelete ("{id}")]
        public void DeleteById (int id) { }
    }
}