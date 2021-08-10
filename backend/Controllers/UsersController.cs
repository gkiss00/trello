using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prid_2021_a06.Models;
using System.ComponentModel.DataAnnotations;
using PRID_Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Security.Claims;
using prid_2021_a06.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace prid_2021_a06.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly PridContext _context;

        public UsersController(PridContext context) {
            _context = context;
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll() {
            var users = await _context.Users.ToListAsync();
            if (users == null)
                return NotFound();
            return users.Select(u => new User(){Id = u.Id, Pseudo = u.Pseudo}).ToDTO();
        }

        [HttpGet("{pseudo}")]
        public async Task<ActionResult<UserDTO>> GetOne(string pseudo) {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Pseudo.Equals(pseudo));
            if (user == null)
                return NotFound();
            return user.ToDTO();
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("byName/{keyWords}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersStartingBy(string keyWords) {
            var users = await _context.Users.Where(u => u.Pseudo.StartsWith(keyWords, StringComparison.InvariantCultureIgnoreCase)).OrderBy(u => u.Pseudo).ToListAsync();
            if (users == null)
                return NotFound();
            return users.Select(u => new User(){Id = u.Id, Pseudo = u.Pseudo}).ToDTO();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> Signup(UserDTO data) {
            Hasher hasher = new Hasher();
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Pseudo.Equals(data.Pseudo));

            if (user != null) {
                if (user.Pseudo == data.Pseudo)
                    return BadRequest(new ValidationErrors().Add("Pseudo already taken", "Pseudo"));
                if (user.Email == data.Email)
                    return BadRequest(new ValidationErrors().Add("Email already linked to another account", "Email"));
            }

            var newUser = new User() { 
                Pseudo = data.Pseudo,
                Password = hasher.Hash(data.Password).Substring(0, 10),
                PasswordConfirm = hasher.Hash(data.PasswordConfirm).Substring(0, 10),
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                BirthDate = data.BirthDate,
                Role = data.Role,
            };

            _context.Users.Add(newUser);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            TokenGen(newUser);

            newUser = new User(){Id = newUser.Id, Pseudo = newUser.Pseudo, Token = newUser.Token};

            return Ok(newUser);
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpPut("{Id}")]
        public async Task<ActionResult<User>> PutUser(int Id, UserDTO data) {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            if (user == null)
                return NotFound();


            user.FirstName = data.FirstName;
            user.LastName = data.LastName;

            _context.Users.Update(user);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            User newUser = new User(){Id = user.Id, Pseudo = user.Pseudo};

            return Ok(user);
        }

        [Authorized(Role.Admin)]
        [HttpDelete("{pseudo}")]
        public async Task<IActionResult> DeleteUser(string pseudo) {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Pseudo.Equals(pseudo));

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> Authenticate(UserDTO data) {
            Hasher hasher = new Hasher();
            var user = await Authenticate(data.Pseudo, hasher.Hash(data.Password).Substring(0, 10));

            if (user == null)
                return BadRequest(new ValidationErrors().Add("User not found", "Pseudo"));
            if (user.Token == null)
                return BadRequest(new ValidationErrors().Add("Incorrect password", "Password"));

            user = new User(){Id = user.Id, Pseudo = user.Pseudo, Token = user.Token, Role=user.Role};
            
            return Ok(user);
        }

        private async Task<User> Authenticate(string pseudo, string password) {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Pseudo.Equals(pseudo));
            // return null if member not found
            if (user == null)
                return null;

            if (user.Password == password) {
                // authentication successful so generate jwt token
                TokenGen(user);
            }

            // remove password before returning
            user.Password = null;

            return user;
        }

        private void TokenGen(User user) {
            var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("my-super-secret-key");
                var tokenDescriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity(new Claim[]
                                                {
                                                    new Claim(ClaimTypes.Name, user.Pseudo),
                                                    new Claim(ClaimTypes.Role, user.Role.ToString())
                                                }),
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
        }

        public User getUserByPseudo(string pseudo)
        {
            var user = _context.Users.FirstOrDefault(m => m.Pseudo.Equals(pseudo));
            return (user);
        }

        public User getCurrentUser()
        {
            var user = new User();
            user = getUserByPseudo(User.Identity.Name);
            if (user == null)
                return null;
            return (user);
        }
    }
}
