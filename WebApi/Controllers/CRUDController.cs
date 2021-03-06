using JWT.Areas.Identity.Data;
using JWT.Data;
using JWT.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebApi.Service.Abstraction;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly IUserService _userService;

        public CRUDController(IUserService userService)   
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("getUsers")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                return Ok(user);

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }

        }
    }
}
