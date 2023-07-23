using EdgeDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            var mutation = @"
            UPDATE Contact
            FILTER Contact.email = <str>$email_toget
            SET {
                first_name := <str>$firstName,
                last_name := <str>$lastName,
                email := <str>$email,
                title := <str>$title,
                description := <str>$description,
                birth_date := <str>$birthDate,
                marital_status := <bool>$maritalStatus
            }";

            await _client.ExecuteAsync(mutation, new Dictionary<string, object?>
            {
                {"email_toget", email },
                { "firstName" , NewContact.FirstName },
                { "lastName", NewContact.LastName },
                { "email" , NewContact.Email },
                { "title" , NewContact.Title },
                { "description" , NewContact.Description },
                { "birthDate" , NewContact.BirthDate },
                { "maritalStatus" , NewContact.MaritalStatus }
            });

            return Page();
 
        }


        public async Task<IActionResult> OnPostSelect(string email)
        {
            string query = "SELECT Contact {first_name, last_name, email, title, description, birth_date, marital_status} " +
                           "FILTER Contact.email = <str>$email;";

            var parameters = new Dictionary<string, object>
            {
                { "email", email }
            };
            // Execute the EdgeDB query and retrieve the result
            var result = await _client.QueryAsync(query, parameters);

   
            dynamic foundContact = result.FirstOrDefault();
            var contact = new Contact
            {
                FirstName = foundContact.first_name,
                LastName = foundContact.last_name,
                Email = foundContact.email,
                Title = foundContact.title,
                Description = foundContact.description,
                BirthDate = foundContact.birth_date,
                MaritalStatus = foundContact.marital_status
            };

            Console.WriteLine(foundContact.marital_status);
            ContactToEdit = contact;
            return Page();
        }
    }
}