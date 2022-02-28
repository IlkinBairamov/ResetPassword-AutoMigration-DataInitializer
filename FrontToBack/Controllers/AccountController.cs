using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)  
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public   IActionResult Register()
        {
            return View();
        }  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
                return View();

            var exsistUser = await _userManager.FindByNameAsync(registerModel.UserName);
            if (exsistUser!=null)
            {
                ModelState.AddModelError("UserName","This username already exsist");
                return View();
            }
            var user = new User
            {
                Email = registerModel.Email,
                UserName = registerModel.UserName,
                FullName = registerModel.FullName
            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(Verify), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
            msg.To.Add(user.Email);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/template/verify.html"))
            {
                body = reader.ReadToEnd();
            }
            msg.Body = body.Replace("{{link}}", link);
            msg.Subject = "Verify";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);
            TempData["confirm"] = true;

           await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index),"Home");
        }

        public async Task<IActionResult> Verify(string email, string token)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            TempData["confirmed"] = true;

            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)

                return View();
            var existUser = await _userManager.FindByNameAsync(loginModel.Username);
            if (existUser == null)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View();
            }
       
            
            var result = await _signInManager.PasswordSignInAsync(existUser, loginModel.Password, loginModel.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "This user is locked out");
                return View();
            }
            if (!result.Succeeded)
            {                                                                            
                ModelState.AddModelError("","Invalid Credentials");
                return View();
            }

            return RedirectToAction(nameof(Index), "Home");
            
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ForgetPassword(AccountViewModel account)
        {
            var user = await _userManager.FindByEmailAsync(account.user.Email);

            if (user==null)
            {
                return BadRequest();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string link = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());


            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
            msg.To.Add(user.Email);


            msg.Body = $"<a href=\"{link}\">Please click here for reset password</a>";
            msg.Subject = "ResetPassword";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult>ResetPassword(string Email,string token)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest();
            }

            var account = new AccountViewModel()
            {
                Token = token,
                user = user
            };
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ResetPassword(AccountViewModel account)    
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }
            var user = await _userManager.FindByEmailAsync(account.user.Email);
            if (user == null)
            {
                return BadRequest();
            }
            var accountModel = new AccountViewModel()
            {
                Token = account.Token,
                user = user
            };
            var result = await _userManager.ResetPasswordAsync(user, account.Token, account.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(accountModel);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
