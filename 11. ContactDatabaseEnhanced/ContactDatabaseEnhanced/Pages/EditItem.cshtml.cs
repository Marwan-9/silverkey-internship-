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

        public async Task<IActionResult> OnPostAsync(String email)
        {
            await _client.ExecuteAsync($@"
                UPDATE Contact
                FILTER Contact.email = '{email}'
                SET {{
                    first_name := '{NewContact.FirstName}',
                    last_name := '{NewContact.LastName}',
                    email := '{NewContact.Email}',
                    title := '{NewContact.Title}',
                    description := '{NewContact.Description}',
                    birth_date := '{NewContact.BirthDate}',
                    marital_status := <bool>{NewContact.MaritalStatus}
                }}
            ");
            return RedirectToPage("/Dataview");
        }


        public async Task<IActionResult> OnPostSelect(string email)
        {

            var result = await _client.QueryAsync<DBContact>($$"""
                SELECT Contact {first_name, last_name, email, title, description, birth_date, marital_status}
                FILTER Contact.email = <str>"{{email}}";
                """);
   
            DBContact foundContact = result.FirstOrDefault();
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

            ContactToEdit = contact;
            return Page();
        }
    }
}