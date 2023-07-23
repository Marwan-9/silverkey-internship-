using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;

namespace FavouriteFeedAJAX.Pages
{
    public class IndexModel : PageModel
    {
        //Initlizaing a new list to carry the items to be renderd
        public List<FeedItem> RSSList { get; set; } = new List<FeedItem>();
        // Array to hold the values for favorite links 
        public string[] linksArray;
        public string[] titlesArray;

        // Injection IHttpClientFactory 
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        [HttpPost]
        public IActionResult OnPost(string feedLink, string feedTitle, bool star)
        {
            var starredLinks = Request.Cookies["StarredLinks"] ?? "";
            var starredTitles = Request.Cookies["StarredTitles"] ?? "";

            if (star)
            {
                // Unstar the item
                if (starredLinks != null && starredTitles != null)
                {
                    string updatedLinks = starredLinks.Replace($"{feedLink},", "");
                    string updatedTitles = starredTitles.Replace($"{feedTitle},", "");
                    Response.Cookies.Append("StarredLinks", updatedLinks);
                    Response.Cookies.Append("StarredTitles", updatedTitles);
                }
            }
            else
            {
                // Star the item
                Response.Cookies.Append("StarredLinks", $"{starredLinks}{feedLink},");
                Response.Cookies.Append("StarredTitles", $"{starredTitles}{feedTitle},");
            }

            // Return an empty result (status code 200) to indicate success
            return new EmptyResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnPostStarItem(StarItemModel model)
        {
            var feedLink = model.FeedLink;
            var feedTitle = model.FeedTitle;
            var star = model.Star;

            var starredLinks = Request.Cookies["StarredLinks"] ?? "";
            var starredTitles = Request.Cookies["StarredTitles"] ?? "";

            if (starredLinks != null && starredTitles != null)
            {
                // Convert the values in cookies into array for easier operations 
                linksArray = starredLinks.Split(',');
                titlesArray = starredTitles.Split(',');

                // If the item is already in the cookies (array), so delete it and update cookies.
                if (linksArray.Contains(feedLink))
                {
                    linksArray = Array.FindAll(linksArray, value => value != feedLink);
                    titlesArray = Array.FindAll(titlesArray, value => value != feedTitle);
                    string updatedCookieLinks = string.Join(",", linksArray);
                    string updatedCookieTitles = string.Join(",", titlesArray);
                    Console.WriteLine(updatedCookieLinks);
                    Console.WriteLine(updatedCookieTitles);
                    Response.Cookies.Append("StarredLinks", updatedCookieLinks);
                    Response.Cookies.Append("StarredTitles", updatedCookieTitles);
                }

                // If not in the cookies (array), add it t the cookies then convert it again to array
                else
                {
                    Response.Cookies.Append("StarredLinks", $"{starredLinks}{feedLink},");
                    Response.Cookies.Append("StarredTitles", $"{starredTitles}{feedTitle},");
                    linksArray = starredLinks.Split(',');
                    titlesArray = starredTitles.Split(',');
                }
            }

            // Saving arrays in tempData; to pass it from Index controller into Favorites controller
            TempData["linksArray"] = linksArray;
            TempData["titlesArray"] = titlesArray;

            // Return a JSON result (status code 200) to indicate success
            return RedirectToPage("/Index");
        }


        // Get request handler of Index page 
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

                // Ensure pageNumber is within valid range
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

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

                // Check if any favorites in cookies in order to determine the action (star or unstar)
                var starredLinks = Request.Cookies["StarredLinks"] ?? "";
                var starredTitles = Request.Cookies["StarredTitles"] ?? "";
                linksArray = starredLinks.Split(',');
                titlesArray = starredTitles.Split(',');
                TempData["linksArray"] = linksArray;
                TempData["titlesArray"] = titlesArray;

                // Pass pagination information to the view
                ViewData["PageNumber"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalItems"] = totalItems;
                ViewData["TotalPages"] = totalPages;
                ViewData["RSSList"] = rssList;
                ViewData["LinksArray"] = linksArray;
                ViewData["TitlesArray"] = titlesArray;
            }
        }
    }
    public class StarItemModel
    {
        public string FeedLink { get; set; }
        public string FeedTitle { get; set; }
        public bool Star { get; set; }
    }
    public class FeedItem
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
    }
}