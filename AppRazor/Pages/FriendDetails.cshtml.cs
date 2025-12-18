using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectB_GoodFriends.Pages
{
    public class FriendDetailsModel : PageModel
    {
        private readonly IFriendsService _friendsService;

        public List<IFriend> Friends { get; set; } = new List<IFriend>();

        public FriendDetailsModel(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task OnGetAsync()
        {
            var response = await _friendsService.ReadFriendsAsync(true, false, "", 0, 1000);
            var items = response?.PageItems ?? new List<IFriend>();
            Friends = items
                .OrderBy(f => f?.FirstName ?? "")
                .ThenBy(f => f?.LastName ?? "")
                .ToList();
        }
    }
}
