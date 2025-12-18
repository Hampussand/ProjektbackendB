using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ProjectB_GoodFriends.Pages
{
    public class FriendsWithPetsModel : PageModel
    {
        private readonly IFriendsService _friendsService;

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; }

        public List<IFriend> Friends { get; set; } = new List<IFriend>();

        public IEnumerable<IGrouping<string, IFriend>> FriendsGroupedByCity { get; set; } = Enumerable.Empty<IGrouping<string, IFriend>>();

        public FriendsWithPetsModel(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task OnGetAsync()
        {
            var response = await _friendsService.ReadFriendsAsync(true, false, "", 0, 1000);
            var items = response?.PageItems ?? new List<IFriend>();

            var countryFilter = (Country ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(countryFilter))
            {
                items = items.Where(f => f?.Address?.Country != null && f.Address.Country.IndexOf(countryFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            Friends = items.ToList();

            FriendsGroupedByCity = Friends
                .Where(f => !string.IsNullOrWhiteSpace(f?.Address?.City))
                .GroupBy(f => f.Address.City)
                .OrderBy(g => g.Key)
                .ToList();
        }
    }
}