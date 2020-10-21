using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<DbUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var userName = User.Claims.FirstOrDefault(x=>x.Type=="name")?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            return Ok(user);
        }
        [HttpPut]
        public async Task<ActionResult> Update(UserProfileModel model)
        {
            var userName = User.Claims.FirstOrDefault(x=>x.Type=="name")?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            user.Email = model.Email; //TODO use ChangeEmail()
            user.Age = model.Age;
            user.Phone = model.Phone;
            
            var res = await _userManager.UpdateAsync(user);
            
            if (res.Succeeded)
                return Ok();
            
            return BadRequest(res.Errors.First().Description);
        }

        [HttpPut("changePhoto")]
        public async Task<ActionResult> ChangePhoto(string base64)
        {
            var userName = User.Claims.FirstOrDefault(x=>x.Type=="name")?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            
            
            if (base64.Contains(","))
            {
                base64 = base64.Split(',')[1];
            }
            var bmp = base64.FromBase64StringToImage();


            var serverPath = _env.ContentRootPath;
            
            var path = Path.Combine(serverPath, "Uploads"); //
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
          
            var fileName = Path.GetRandomFileName() + ".jpg";

            var filePathSave = Path.Combine(path, fileName);

            bmp.Save(filePathSave, ImageFormat.Jpeg);

            user.Image = fileName;
            
            var res = await _userManager.UpdateAsync(user);
            
            if (res.Succeeded)
                return Ok();
            
            return BadRequest(res.Errors.First().Description);
        }
    }
}
