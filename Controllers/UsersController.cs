using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boilerplate.API.Data;
using boilerplate.API.DataObjectTransfer;
using boilerplate.API.Models;
using Microsoft.AspNetCore.Mvc;
//using boilerplate.API.Models;

namespace boilerplate.API.Controllers {
    [Route ("api/user")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly IAuthRepository _repo;

        public UsersController (IAuthRepository repo) {
            _repo = repo;
        }

        // POST api/users
        [HttpPost ("register")]
        public async Task<IActionResult> Register (RegisterUser registerUser) {
            registerUser.Username = registerUser.Username.ToLower();
            if (await _repo.IsUser(registerUser.Username)) {
                return BadRequest("User already exists");
            }
            
            var createUser = new User{Username = registerUser.Username};
            var user = await _repo.RegisterAsync(createUser, registerUser.Password);
            
            return StatusCode(201);
        }

        // PUT api/users/5
        [HttpPut ("{id}")]
        public void Put (int id, [FromBody] string value) { }

        // DELETE api/users/5
        [HttpDelete ("{id}")]
        public void DeleteById (int id) { }
    }
}