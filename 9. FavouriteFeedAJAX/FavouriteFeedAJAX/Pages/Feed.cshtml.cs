using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace FavouriteFeedAJAX.Pages
{
    public class FeedModel : PageModel
    {
        //Initlizaing a new list to carry the items to be renderd
        public List<Item> ItemsDetails { get; private set; } = new List<Item>();

        // Injection IHttpClientFactory 
        private readonly IHttpClientFactory _httpClientFactory;

        public FeedModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        public async Task<List<Item>> XMLParser(string feed)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //Creating a cleint, wait till the get request is resolved  
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(feed);

            if (response.IsSuccessStatusCode)
            {
                //wait till the get request is resolved and load it as XML file
                var xmlContent = await response.Content.ReadAsStringAsync();
                xmlDoc.LoadXml(xmlContent);

                //Just taking the needed nodes (tags) from the XML
                var itemsNode = xmlDoc.SelectNodes("rss/channel/item");
                foreach (XmlNode itemNode in itemsNode)
                {

                    //Extract needed info from XML and pass it to the itemDetails object
                    var itemDetails = new Item
                    {
                        Title = itemNode.SelectSingleNode("title")?.InnerText,
                        Description = itemNode.SelectSingleNode("description")?.InnerText,
                        PublishedDate = itemNode.SelectSingleNode("pubDate")?.InnerText,
                        Link = itemNode.SelectSingleNode("link")?.InnerText,
                    };
                    // Pass itemDetails to the list of items
                    ItemsDetails.Add(itemDetails);
                }
            }
            return ItemsDetails;

        }
        //Method to be called on get request
        public async Task<IActionResult> OnGetAsync(string feedLink)
        {
            string feed = Request.Query["message"];
            var List = await XMLParser(feed);
            ItemsDetails = List;
            return Page();
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
