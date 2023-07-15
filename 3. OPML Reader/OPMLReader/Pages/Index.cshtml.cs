using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace OPMLReader.Pages
{
    public class IndexModel : PageModel
    {
        //Initlizaing a new list to carry the items to be renderd
        public List<FeedItem> RSSList { get; set; } = new List<FeedItem>();

        // Injection IHttpClientFactory 
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;


        public async Task OnGet(int pageNumber = 1, int pageSize = 8)
        {
            //Creating a cleint, wait till the get request is resolved  
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://blue.feedland.org/opml?screenname=dave");

            if (response.IsSuccessStatusCode)
            {
                //wait till the get request is resolved and load it as XML file
                var responseData = await response.Content.ReadAsStringAsync();
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.LoadXml(responseData);

                //Just taking the needed nodes (tags) from the XML
                XmlNodeList? nodes = XMLDoc.DocumentElement?.GetElementsByTagName("outline");

                //Determining the number of pages based of size od page (pagination)
                int totalItems = nodes?.Count ?? 0;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                //Create a list to carry the items
                List<FeedItem> rssList = new List<FeedItem>();

                if (nodes != null)
                {
                    //Setting the start and the end depending on pagination conditions
                    int startIndex = (pageNumber - 1) * pageSize;
                    int endIndex = Math.Min(startIndex + pageSize, nodes.Count);

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        //Extract needed info from XML and pass it to the feedObject object
                        XmlNode node = nodes[i];
                        FeedItem feedObject = new FeedItem();
                        feedObject.Title = node.Attributes?["text"]?.Value;
                        feedObject.Link = node.Attributes?["xmlUrl"]?.Value;
                        // Pass itemDetails to the list of items
                        rssList.Add(feedObject);
                    }
                }

                // Pass pagination information to the view
                ViewData["PageNumber"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalItems"] = totalItems;
                ViewData["TotalPages"] = totalPages;
                ViewData["RSSList"] = rssList;
            }
        }


    }
    public class FeedItem
    {
        public string? Title { get; set; }
        public string? Link{get; set; }
    }
}

