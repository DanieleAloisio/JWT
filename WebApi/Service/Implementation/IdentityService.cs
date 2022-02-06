using Common;
using Email;
using JWT.Areas.Identity.Data;
using JWT.Data;
using JWT.Dto;
using JWT.Service.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Dto.Requests;

namespace JWT.Service.Implementation
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JWTLocalDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;
        private readonly IEmail _emailSending;

        public IdentityService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole<Guid>> roleManager,
             JWTLocalDbContext context,
             IOptions<AppSettings> appSettings,
             IHttpContextAccessor httpContextAccessor,
             IEmail emailSending)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor!.HttpContext!;
            _emailSending = emailSending;
        }

        #region Public interface methods
        public async Task<BaseResponse> RegisterAsync(RegisterRequest request)
        {
            return await RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);
        }

        public async Task<BaseResponse> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            var response = new BaseResponse();
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                response.Errors.Add("User with this email address already exists");
                return response;
            }

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);
            if (!createdUser.Succeeded)
            {
                response.Errors.AddRange(createdUser.Errors.Select(x => x.Description));
                return response;
            }

            var request = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host.Value}";
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmEmailLink = $"{request}/confirm-email?token={WebUtility.UrlEncode(token)}&userId={newUser.Id}";

            //TODO
            //await _emailSending.SendAsync(null, newUser.Email, "Confirm Email .NET Auth", $"<a href=\"{confirmEmailLink}\">Click here to confirm your email</a>");

            response.Success = true;
            return response;
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var errorResponse = new AuthResponse();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                errorResponse.Errors.Add("Wrong email and password");
                return errorResponse;
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                errorResponse.Errors.Add("Wrong email and password");
                return errorResponse;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
            if (!result.Succeeded)
            {
                errorResponse.Errors.Add("Wrong email and password");
                return errorResponse;
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        #endregion
        private async Task<AuthResponse> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = await GetTokenClaimsAsync(user);
            var token = GenerateJwtToken(tokenHandler, claims);
            var refreshToken = await GenerateJwtRefreshTokenAsync(user.Id, token.Id);

            return new AuthResponse
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                ExpireDate = token.ValidTo,
                UserName = user.UserName,
                Roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList()
            };
        }

        private SecurityToken GenerateJwtToken(JwtSecurityTokenHandler tokenHandler, IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.SecretKey));
            var UtcNow = DateTime.UtcNow;

            //Create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = UtcNow.AddMinutes(_appSettings.JWT.TokenExpiresIn),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Audience = _appSettings.JWT.ValidAudience,
                Issuer = _appSettings.JWT.ValidIssuer
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        private async Task<IEnumerable<Claim>> GetTokenClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("Id", user.Id.ToString())
        };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Take user's roles e map them to Claims. For each role it takes all realted claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                //claims.AddRange(roleClaims.Except(claims));
                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            return claims;
        }

        private async Task<RefreshToken> GenerateJwtRefreshTokenAsync(Guid UserId, string TokenId)
        {
            var refreshToken = new RefreshToken
            {
                JwtId = TokenId,
                UserId = UserId,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_appSettings.JWT.RefreshTokenExpiresIn)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}
