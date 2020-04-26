using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Constants;
using IdentityServer.Infrastructure.Data.Identity;
using IdentityServer.Models;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AccountController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationService _authenticationService;


        public AccountController(UserManager<AppUser> userManager, IEventService events, IIdentityServerInteractionService interactionService, IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _events = events;
            _interaction = interactionService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterRequestViewModel model)
        {
            //var aVal = 0; var blowUp = 1 / aVal;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", Roles.Consumer));

            return Ok(new RegisterResponseViewModel(user));
        }

        [HttpPost("{[action]}")]
        public async Task<IActionResult> Login([FromBody] LoginRequestViewModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            string accessToken = string.Empty;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.Name));

                AuthenticationProperties props = null;
                await HttpContext.SignInAsync(user.Id, user.UserName, props);

                //accessToken = await HttpContext.GetTokenAsync("access_token"); Will come back to fix

                // Now you can use them. For more info on when and how to use the 
                // access_token and id_token, see https://auth0.com/docs/tokens


                Dictionary<string, string> list = new Dictionary<string, string>();
                foreach (var prop in (await HttpContext.AuthenticateAsync()).Properties.Items)
                {
                    list.Add(prop.Key, prop.Value);
                }

                //return Redirect(model.ReturnUrl);
                return Ok(accessToken);

            }
            await _events.RaiseAsync(new UserLoginFailureEvent(model.Email, "invalid credentials"));
            //ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            return BadRequest(ModelState);
        }
    }
}