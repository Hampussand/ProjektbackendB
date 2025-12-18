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
    public class FriendsInCountryModel : PageModel
    {
        private readonly IAddressesService _addressesService;

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchBy { get; set; } = "Country";

        public List<IFriend> Friends { get; set; } = new List<IFriend>();

        public FriendsInCountryModel(IAddressesService addressesService)
        {
            _addressesService = addressesService;
        }

        public async Task OnGetAsync()
        {
            var response = await _addressesService.ReadAddressesAsync(true, true, "", 0, 1000);
            var addresses = response?.PageItems ?? new List<Models.Interfaces.IAddress>();

            var q = (Query ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(q))
            {
                if (string.Equals(SearchBy, "City", StringComparison.OrdinalIgnoreCase))
                {
                    addresses = addresses.Where(a => a?.City != null && a.City.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
                else
                {
                    addresses = addresses.Where(a => a?.Country != null && a.Country.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
            }
            Friends = addresses
                .Where(a => a.Friends != null)
                .SelectMany(a => a.Friends)
                .ToList();
        }
    }
}
