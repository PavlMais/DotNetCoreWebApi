using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using FastFood.WebApi.Entities;
using FastFood.WebApi.Models;
using FastFood.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly EfContext _context;
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly IJwtTokenService _IJwtTokenService;
        private readonly IWebHostEnvironment _env;

        public AccountController(EfContext context,
            UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            IJwtTokenService IJwtTokenService,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _IJwtTokenService = IJwtTokenService;
            _env = env;
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
            
            
            var base64 = model.ImageBase64;
            if (base64.Contains(","))
            {
                base64 = base64.Split(',')[1];
            }
            var bmp = FromBase64StringToImage(base64);

            var serverPath = _env.ContentRootPath; //Directory.GetCurrentDirectory(); //_env.WebRootPath;
            
            var path = Path.Combine(serverPath, "Uploads"); //
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
          
            var fileName = Path.GetRandomFileName() + ".jpg";

            var filePathSave = Path.Combine(path, fileName);

            bmp.Save(filePathSave, ImageFormat.Jpeg);
            
            
            
            

            var user = new DbUser
            {
                Email = model.Email,
                UserName = model.Email,
                Image = filePathSave,
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
        
        public static Bitmap FromBase64StringToImage(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    using (Image imgReturn = Image.FromStream(memoryStream))
                    {
                        memoryStream.Close();
                        byteBuffer = null;
                        return new Bitmap(imgReturn);
                    }
                }
            }
            catch { return null; }

        }
    }
}