using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace XMLParser.Pages
{
    public class IndexModel : PageModel
    {
        //Initlizaing ILogger 
        private readonly ILogger<IndexModel> _logger;

        //Initlizaing a new list to carry the items to be renderd
        public List<Item> ItemsDetails { get; set; } = new List<Item>();

        // Injection IHttpClientFactory 
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        public async Task<IActionResult> OnGet()
        {   
            XmlDocument xmlDoc = new XmlDocument();
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://scripting.com/rss.xml");

            if (response.IsSuccessStatusCode)
            {
                var xmlContent = response.Content.ReadAsStringAsync().Result;
                xmlDoc.LoadXml(xmlContent);
                var itemsNode = xmlDoc.SelectNodes("rss/channel/item");
                foreach (XmlNode itemNode in itemsNode)
                {

                    var itemDetails = new Item
                    {
                        Title = itemNode.SelectSingleNode("title")?.InnerText,
                        Description = itemNode.SelectSingleNode("description")?.InnerText,
                        PublishedDate = itemNode.SelectSingleNode("pubDate")?.InnerText,
                        Link = itemNode.SelectSingleNode("link")?.InnerText,
                    };
                    ItemsDetails.Add(itemDetails);
                }
                return Page();
            }

            else 
            {
                _logger.LogError("Error");
                return RedirectToPage("/Error");
            }
        }
    }
    public class Item
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string PublishedDate { get; set; }
            public string Link { get; set; }       
        }
}

