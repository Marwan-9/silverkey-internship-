using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;


namespace ContactDatabaseEnhanced.Pages
{
    public class InsertItemModel : PageModel
    {
        [BindProperty]
        public Contact NewContact { get; set; } = new();
        private readonly EdgeDBClient _client;

        public InsertItemModel(EdgeDBClient client)
        {
            _client = client;
        }
        public void OnGet()
        {
            var client = new EdgeDBClient();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            Console.WriteLine(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: NewContact.Password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            var query = "INSERT Contact {first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, " +
                "title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status, " +
                "user_name := <str>$user_name, password := <str>$password, user_role := <str>$user_role, salt :=<bytes>$salt }";
            await _client.ExecuteAsync(query, new Dictionary<string, object?>
                {
                    {"first_name", NewContact.FirstName},
                    {"last_name", NewContact.LastName},
                    {"email", NewContact.Email},
                    {"title", NewContact.Title},
                    {"description", NewContact.Description},
                    {"birth_date", NewContact.BirthDate},
                    {"marital_status", NewContact.MaritalStatus},
                    {"user_name", NewContact.UserName},
                    {"password", hashed},
                    {"user_role", NewContact.UserRole},
                    {"salt", salt}
                });
            return RedirectToPage("/Dataview");
        }
    }

    public class Contact
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String BirthDate { get; set; }
        public bool MaritalStatus { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String UserRole { get; set; }

        public byte[]? Salt { get; set; }

        public Contact()
        {
        }
        public Contact(string firstName, string lastName, string email, string title, string description, string birthDate, bool maritalStatus, string userName, string password, string userType, byte[] salt)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Title = title;
            Description = description;
            BirthDate = birthDate;
            MaritalStatus = maritalStatus;
            UserName = userName;
            Password = password;
            UserRole = userType;
            Salt = salt;
        }
    }
}