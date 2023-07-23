using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FavouriteFeedAJAX.Pages
{
    public class FavoritesModel : PageModel
    {
        private const int DefaultPageSize = 8;

        public List<FeedItem> FavList { get; set; } = new List<FeedItem>();
        public IActionResult OnGet(int? pageNumber, int? pageSize)
        {
            // Recieve favorites from Index pages
            string[] links = TempData["linksArray"] as string[];
            string[] titles = TempData["titlesArray"] as string[];

            // Handling the case of recieving no favorites
            if (links == null)
                links = new string[0];

            // Clear old favorites list, and add the new items to it
            FavList.Clear();
            for (int i = 0; i < links.Length; i++)
            {
                FeedItem feedObject = new FeedItem();
                feedObject.Link = links[i];
                feedObject.Title = titles[i];
                FavList.Add(feedObject);
            }

            // Retrieve pagination parameters from query string or use default values
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? DefaultPageSize;

            // Determine the number of pages based on the current page size (pagination)
            int totalItems = FavList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Ensure currentPageNumber is within valid range
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            // Calculate the start and end index for the current page
            int startIndex = (currentPageNumber - 1) * currentPageSize;
            int endIndex = Math.Min(startIndex + currentPageSize, totalItems);

            // Create a sublist of FavList for the current page
            List<FeedItem> currentPageItems = FavList.GetRange(startIndex, endIndex - startIndex);

            // Pass pagination information and the current page items to the view
            ViewData["PageNumber"] = currentPageNumber;
            ViewData["PageSize"] = currentPageSize;
            ViewData["TotalPages"] = totalPages;
            ViewData["favList"] = currentPageItems;

            // Store the favorites array again in temp data, to be available after refresh (re-direct)
            TempData["linksArray"] = links;
            TempData["titlesArray"] = titles;

            return Page();
        }
    }
}