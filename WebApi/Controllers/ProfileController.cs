using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastFood.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<DbUser> _userManager;

        public ProfileController(UserManager<DbUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var userName = User.Claims.FirstOrDefault(x=>x.Type=="name").Value;
            var user = await _userManager.FindByNameAsync(userName);
            return Ok(user);
        }
    }
}
