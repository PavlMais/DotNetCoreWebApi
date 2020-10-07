using System.Linq;
using System.Threading.Tasks;
using FastFood.WebApi.Entities;
using FastFood.WebApi.Models;
using FastFood.WebApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly EfContext _context;
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly IJwtTokenService _IJwtTokenService;

        public AccountController(EfContext context,
            UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            IJwtTokenService IJwtTokenService)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _IJwtTokenService = IJwtTokenService;
        }
            
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //var errrors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest("Bad Model");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return BadRequest(new { invalid = "Даний користувач не знайденний" });
            }

            var result = _signInManager
                .PasswordSignInAsync(user, model.Password, false, false).Result;

            if (!result.Succeeded)
            {
                return BadRequest(new { invalid = "Невірно введений пароль" });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(
                new
                {
                    token = _IJwtTokenService.CreateToken(user)
                });
        }
    
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //var errrors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest("Bad Model");
            }

            var user = new DbUser
            {
                Email = model.Email,
                UserName = model.Email,
                Image = "https://cdn.pixabay.com/photo/2017/07/28/23/34/fantasy-picture-2550222_960_720.jpg",
                Age = 30,
                Phone = model.Phone,
                Description = "PHP programmer"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { invalid = "Хюстон у нас проблеми!" });
            }

            return Ok();
        }
    }
}