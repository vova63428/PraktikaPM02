using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AgroSintez.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private AgroSintezEntities db;

        public AuthController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public class LoginDto
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        public class RegisterDto
        {
            public string Login { get; set; }
            public string FullName { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Department { get; set; }
        }

        public class AuthResponseDto
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Token { get; set; }
        }

        [HttpPost]
        [Route("debug-login")]
        public IHttpActionResult DebugLogin([FromBody] dynamic data)
        {
            try
            {
                string login = data?.login;
                string password = data?.password;

                return Ok(new
                {
                    receivedLogin = login,
                    receivedPassword = password,
                    loginIsNull = login == null,
                    passwordIsNull = password == null,
                    contentType = Request.Headers.Accept?.FirstOrDefault()?.MediaType,
                    method = Request.Method.Method
                });
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPost]
        [Route("debug-register")]
        public IHttpActionResult DebugRegister(RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null || string.IsNullOrEmpty(registerDto.Login))
                {
                    return BadRequest("Invalid data");
                }

                var user = new users
                {
                    login = registerDto.Login,
                    full_name = registerDto.FullName ?? registerDto.Login,
                    role = registerDto.Role ?? "operator",
                    department = registerDto.Department,
                    is_active = 1,
                    created_date = DateTime.Now,
                    password = registerDto.Password
                };

                db.users.Add(user);
                db.SaveChanges();

                return Ok(new { message = "User created", id = user.id });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors.SelectMany(v => v.ValidationErrors)
                             .Select(e => new { e.PropertyName, e.ErrorMessage });
                return Ok(new { error = "Validation failed", details = errors });
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    return BadRequest("No data received");
                }

                if (string.IsNullOrEmpty(loginDto.Login))
                {
                    return BadRequest("Login is required");
                }

                if (string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest("Password is required");
                }

                // Ищем пользователя с таким логином и паролем
                var user = db.users.FirstOrDefault(u => u.login == loginDto.Login
                                                      && u.password == loginDto.Password
                                                      && u.is_active == 1);

                if (user == null)
                {
                    return Unauthorized();
                }

                var token = GenerateJwtToken(user);

                return Ok(new AuthResponseDto
                {
                    Id = user.id,
                    Login = user.login,
                    FullName = user.full_name,
                    Role = user.role,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null || string.IsNullOrEmpty(registerDto.Login))
                {
                    return BadRequest("Invalid data");
                }

                if (string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Password is required");
                }

                if (db.users.Any(u => u.login == registerDto.Login))
                {
                    return BadRequest("Login already exists");
                }

                var user = new users
                {
                    login = registerDto.Login,
                    full_name = registerDto.FullName ?? registerDto.Login,
                    role = registerDto.Role ?? "operator",
                    department = registerDto.Department,
                    is_active = 1,
                    created_date = DateTime.Now,
                    password = registerDto.Password  // Просто сохраняем пароль
                };

                db.users.Add(user);
                db.SaveChanges();

                var token = GenerateJwtToken(user);

                return Ok(new AuthResponseDto
                {
                    Id = user.id,
                    Login = user.login,
                    FullName = user.full_name,
                    Role = user.role,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string GenerateJwtToken(users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-super-secret-key-min-32-chars-long-1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Name, user.login ?? ""),
                    new Claim(ClaimTypes.Role, user.role ?? "user")
                },
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("test")]
        public IHttpActionResult Test()
        {
            try
            {
                var count = db.users.Count();
                return Ok(new
                {
                    message = "AuthController работает!",
                    timestamp = DateTime.Now,
                    usersCount = count,
                    dbConnected = count > 0
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("simple")]
        public IHttpActionResult Simple()
        {
            return Ok(new { message = "Server works!", time = DateTime.Now });
        }

        [HttpGet]
        [Route("users-list")]
        public IHttpActionResult UsersList()
        {
            var users = db.users.Select(u => new { u.id, u.login, u.is_active, u.role, u.password }).ToList();
            return Ok(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}