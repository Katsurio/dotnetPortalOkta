using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okta.Sdk;
using dotnetPortalOkta.Models;
using Okta.Sdk.Internal;
using System;

namespace dotnetPortalOkta.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOktaClient _oktaClient;

        public AccountController(IOktaClient oktaClient = null)
        {
            _oktaClient = oktaClient;
        }

        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Apps", "Account");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Me()
        {
            var username = User.Claims
                .FirstOrDefault(x => x.Type == "preferred_username")
                ?.Value.ToString();

            var viewModel = new MeViewModel
            {
                Username = username,
                SdkAvailable = _oktaClient != null
            };

            if (!viewModel.SdkAvailable)
            {
                return View(viewModel);
            }

            if (!string.IsNullOrEmpty(username))
            {
                var user = await _oktaClient.Users.GetUserAsync(username);
                var vader = await _oktaClient.Users.GetUserAsync("jfisher.jobs");
                var appList = (await vader.AppLinks.ToList()).Select( g => new 
                    { 
                        AppAssignmentId = g.AppAssignmentId,
                        AppInstanceId = g.AppInstanceId,
                        Name = g.AppName,
                        Id = g.Id,
                        Label = g.Label,
                        Link = g.LinkUrl,
                        Logo = g.LogoUrl
                }).ToArray();
                var factors = await vader.Factors.ToArray();


                //await _oktaClient.GetAsync<T>("/api/v1/apps?filter=user.id+eq+%22%22");


                dynamic userInfoWrapper = new ExpandoObject();
                userInfoWrapper.Profile = user.Profile;
                userInfoWrapper.PasswordChanged = user.PasswordChanged;
                userInfoWrapper.LastLogin = user.LastLogin;
                userInfoWrapper.Status = user.Status.ToString();
                viewModel.UserInfo = userInfoWrapper;

                viewModel.Groups = (await user.Groups.ToList()).Select(g => g.Profile.Name).ToArray();
            }

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Apps()
        {
            var username = User.Claims
                .FirstOrDefault(x => x.Type == "preferred_username")
                ?.Value.ToString();

            var viewModel = new MeViewModel
            {
                Username = username,
                SdkAvailable = _oktaClient != null
            };

            if (!viewModel.SdkAvailable)
            {
                return View(viewModel);
            }

            if (!string.IsNullOrEmpty(username))
            {
                var user = await _oktaClient.Users.GetUserAsync(username);
                dynamic userInfoWrapper = new ExpandoObject();
                userInfoWrapper.Profile = user.Profile;
                userInfoWrapper.PasswordChanged = user.PasswordChanged;
                userInfoWrapper.LastLogin = user.LastLogin;
                userInfoWrapper.Status = user.Status.ToString();
                viewModel.UserInfo = userInfoWrapper;

                viewModel.Groups = (await user.Groups.ToList()).Select(g => g.Profile.Name).ToArray();
            }

            return View(viewModel);
        }
    }
}
