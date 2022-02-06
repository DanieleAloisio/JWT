using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using JWT.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using JWT.Service.Abstraction;
using JWT.Dto;
using System.Net;
using Microsoft.AspNetCore.Http;
using WebApi.Dto.Requests;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _identityService = identityService;

        }

        #region TEST
        //[HttpPost]
        //[Route("createToken")]
        //public async Task<IActionResult> CreateToken([FromBody] JwtTokenViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);
        //        var signInResult = await _userManager.CheckPasswordAsync(user, model.Password);

        //        if (signInResult)
        //        {
        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCostants.Key));
        //            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //            var claims = new[]
        //            {
        //                new Claim(JwtRegisteredClaimNames.Sub, model.UserName),
        //                new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
        //                new Claim(JwtRegisteredClaimNames.UniqueName, model.UserName)
        //            };

        //            var token = new JwtSecurityToken(
        //                JwtCostants.Issuer,
        //                JwtCostants.Audience,
        //                claims,
        //                expires: DateTime.UtcNow.AddMinutes(30),
        //                signingCredentials: creds
        //                );

        //            var results = new
        //            {
        //                token = new JwtSecurityTokenHandler().WriteToken(token),
        //                exipiration = token.ValidTo
        //            };

        //            return Created("", results);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }

        //    return BadRequest();
        //}
        #endregion

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var resp = new BaseResponse();

            try
            {
                if (!ModelState.IsValid)
                {
                    //resp.Errors.AddRange(ModelState.GetErrors());
                    return BadRequest(resp);
                }

                resp = await _identityService.RegisterAsync(request);
                if (!resp.Success)
                {
                    return BadRequest(resp);
                }

                return Ok(resp);
            }
            catch (Exception e)
            {
                resp.Errors.Add(e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var resp = new AuthResponse();

            try
            {
                if (!ModelState.IsValid)
                {
                    //resp.Errors.AddRange(ModelState.GetErrors());
                    return BadRequest(resp);
                }

                resp = await _identityService.LoginAsync(request.Email, request.Password);
                if (!resp.Success)
                {
                    return Unauthorized(resp);
                }

                SetRefreshTokenCookie(resp.RefreshToken);
                resp.HideRefreshToken();
                return Ok(resp);
            }
            catch (Exception e)
            {
                resp.Errors.Add(e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, resp);
            }
        }

        private string SetRefreshTokenCookie(string refreshToken)
        {
            var rtCoookie = new Dto.Cookie("rToken", new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                //Path = "/",
                //Domain = "localhost"
            });

            rtCoookie.SetCookieValue(refreshToken, Response);

            return null;
        }

    }
}
