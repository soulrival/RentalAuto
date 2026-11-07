using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RentalAuto.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public LoginModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Требуется имя пользователя")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно содержать от 3 до 50 символов")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Требуется пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка учетных данных
            var role = await AuthenticateUserAsync(Input.Username, Input.Password);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(ReturnUrl);
        }

        private async Task<string?> AuthenticateUserAsync(string username, string password)
        {
            // Для клиента: проверяем по email из базы данных
            if (_context.Clients.Any(c => c.Email == username))
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == username);
                if (client != null && password == "client123") // Упрощенный пароль для всех клиентов
                {
                    return "rental_client";
                }
            }

            // Для ролей PostgreSQL (инспектор и админ)
            var users = new Dictionary<string, (string password, string role)>
            {
                { "rental_inspector", ("inspector_pass", "rental_inspector") },
                { "rental_admin", ("admin_pass", "rental_admin") }
            };

            if (users.TryGetValue(username, out var user) && user.password == password)
            {
                return user.role;
            }

            return null;
        }
    }
}