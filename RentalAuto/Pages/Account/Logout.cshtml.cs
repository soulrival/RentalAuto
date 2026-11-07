using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalAuto.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public string ReturnUrl { get; set; } = "/";

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync("CookieAuth");

            returnUrl = returnUrl ?? Url.Content("~/");
            return LocalRedirect(returnUrl);
        }
    }
}