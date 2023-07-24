using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using EdgeDB;
using static ContactDatabaseEnhanced.Pages.DataviewModel;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ContactDatabaseEnhanced.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public Credentials LoginInput { get; set; }
        private readonly EdgeDBClient _client;

        public IndexModel(EdgeDBClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnPost()
        {
            string username = LoginInput.Username;
            string password = LoginInput.Password;
            string role;

            Console.WriteLine(string.IsNullOrEmpty(username));

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Please enter username and password");
                return Page();

            }

            string query = "SELECT Contact {user_name, password, salt, user_role} " +
                           "FILTER Contact.user_name = <str>$username;";
            var parameters = new Dictionary<string, object>
            {
                { "username", username }
            };
            // Execute the EdgeDB query and retrieve the result
            var result = await _client.QueryAsync<DBContact>(query, parameters);
            if (result.Count != 0)
            {
                DBContact foundContact = result.FirstOrDefault();
                role = Authenticator(foundContact, password);
            }
            else
            {
                ModelState.AddModelError("", "User does not exist");
                return Page();
            }

            if (role=="user")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToPage("/Homepage");
            }

            else if (role == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToPage("/Homepage");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return Page();
            }
        }


        public static string Authenticator(DBContact foundContact, string password) {

            var contact = new Contact
            {
                UserName = foundContact.user_name,
                Password = foundContact.password,
                UserRole = foundContact.user_role,
                Salt = foundContact.salt,
            };
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: contact.Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            if (foundContact.password == hashed)
            {
                return foundContact.user_role;

            }
            else
            {
                return "invalid";
            }

        }


    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}