using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Interfaces;
using Models.Interfaces;

namespace AppRazor.Pages
{
    public class FriendsByCountryModel : PageModel
    {
        private readonly IFriendsService _friendsService;

        public List<CountryGroupDto> FriendsByCountry { get; set; } = new();
        public IFriend? TestFriend { get; set; }

        public FriendsByCountryModel(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task OnGetAsync()
        {
            var response = await _friendsService.ReadFriendsAsync(true, false, "", 0, 1000);

            FriendsByCountry = response.PageItems
                .Where(f => f.Address != null && !string.IsNullOrEmpty(f.Address.Country))
                .GroupBy(f => f.Address!.Country)
                .Select(g => new CountryGroupDto
                {
                    Country = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Country)
                .ToList();
        }
    }

    public class CountryGroupDto
    {
        public string Country { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
