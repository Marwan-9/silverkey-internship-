using EdgeDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ContactDatabaseEnhanced.Pages.DataviewModel;

namespace ContactDatabaseEnhanced.Pages
{

    public class EditItemModel : PageModel
    {
        [BindProperty]
        public Contact NewContact { get; set; } = new();
        public Contact ContactToEdit { get; set; } = new();
        public List<Contact> ContactsList { get; private set; } = new();

        private readonly EdgeDBClient _client;

        public EditItemModel(EdgeDBClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {

            Console.WriteLine(NewContact.UserRole);
            var query = @"
            UPDATE Contact
            FILTER Contact.email = <str>$email_toget
            SET {
                first_name := <str>$firstName,
                last_name := <str>$lastName,
                email := <str>$email,
                title := <str>$title,
                description := <str>$description,
                birth_date := <str>$birthDate,
                marital_status := <bool>$maritalStatus,
                user_role := <str>$userRole
            }";

            await _client.ExecuteAsync(query, new Dictionary<string, object?>
            {
                {"email_toget", email },
                { "firstName" , NewContact.FirstName },
                { "lastName", NewContact.LastName },
                { "email" , NewContact.Email },
                { "title" , NewContact.Title },
                { "description" , NewContact.Description },
                { "birthDate" , NewContact.BirthDate },
                { "maritalStatus" , NewContact.MaritalStatus },
                { "userRole", NewContact.UserRole }
            });

            return RedirectToPage("/Dataview");
        }


        public async Task<IActionResult> OnPostSelect(string email)
        {

            string query = "SELECT Contact {first_name, last_name, email, title, description, birth_date, marital_status, user_role} " +
                                       "FILTER Contact.email = <str>$email;";

            var parameters = new Dictionary<string, object>
            {
                { "email", email }
            };
            // Execute the EdgeDB query and retrieve the result
            var result = await _client.QueryAsync<DBContact>(query, parameters);
                
            DBContact foundContact = result.FirstOrDefault();
            var contact = new Contact
            {
                FirstName = foundContact.first_name,
                LastName = foundContact.last_name,
                Email = foundContact.email,
                Title = foundContact.title,
                Description = foundContact.description,
                BirthDate = foundContact.birth_date,
                MaritalStatus = foundContact.marital_status,
                UserRole = foundContact.user_role
            };

            ContactToEdit = contact;
            return Page();
        }
    }
}