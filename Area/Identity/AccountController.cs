using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lamsura.DTOs.Request;
using Lamsura.DTOs.Response;
using Lamsura.Models;
using Lamsura.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Lamsura.Area.identity
    {
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
        {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController ( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService )
            {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            }



        [HttpPost("Login")]
        public async Task<IActionResult> Login ( [FromBody] loginRequest loginRequest )
            {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if ( user is null )
                {
                return NotFound(new ErrorModelResponse
                    {
                    Code = "Invalid Cred.",
                    Description = "Invalid Email OR Password"
                    });
                }

            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);

            if ( !result.Succeeded )
                {
                if ( result.IsLockedOut )
                    return BadRequest(new ErrorModelResponse
                        {
                        Code = "Too many attemps",
                        Description = "Too many attemps, try again after 5 min"
                        });
                else
                    return NotFound(new ErrorModelResponse
                        {
                        Code = "Invalid Cred.",
                        Description = "Invalid Email OR Password"
                        });
                }


            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach ( var role in userRoles )
                {
                claims.Add(new Claim(ClaimTypes.Role, role));
                }

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new
                {
                accessToken = accessToken,
                ValidTo = "15 Min",
                refreshToken = refreshToken,
                RefreshTokenExpiration = "7 day"
                });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh ( [FromBody] TokenApiRequest tokenApiRequest )
        {
            if ( tokenApiRequest?.AccessToken == null || tokenApiRequest.RefreshToken == null )
                return BadRequest("Invalid client request");

            ClaimsPrincipal principal;
            try {
                principal = _tokenService.GetPrincipalFromExpiredToken(tokenApiRequest.AccessToken);
            }
            catch {
                return BadRequest("Invalid access token");
            }
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if ( string.IsNullOrEmpty(userId) )
                return BadRequest("Invalid token claims");

            var user = await _userManager.FindByIdAsync(userId);

            if ( user == null )
                return BadRequest("User not found");

            if ( user.RefreshToken != tokenApiRequest.RefreshToken )
                return BadRequest("Refresh token mismatch");

            if ( user.RefreshTokenExpiryTime <= DateTime.UtcNow )
                return BadRequest("Refresh token expired");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                accessToken = newAccessToken,
                ValidTo = "15 Min",
                refreshToken = newRefreshToken
            });

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke ()
            {
            var username = User.Identity.Name;

            var user = _userManager.Users.FirstOrDefault(e => e.UserName == username);
            if ( user == null ) return BadRequest();

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
            }
        }
    }



