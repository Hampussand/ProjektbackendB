using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Models.DTO;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectB_GoodFriends.Pages
{
    public class FriendEditModel : PageModel
    {
        private readonly IFriendsService _friendsService;
        private readonly IAddressesService _addressesService;
        private readonly IPetsService _petsService;
        private readonly IQuotesService _quotesService;

        public List<IFriend> Friends { get; set; } = new List<IFriend>();
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public FriendEditModel(IFriendsService friendsService, IAddressesService addressesService, IPetsService petsService, IQuotesService quotesService)
        {
            _friendsService = friendsService;
            _addressesService = addressesService;
            _petsService = petsService;
            _quotesService = quotesService;
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

        public async Task<IActionResult> OnPostUpdateFriendAsync(Guid friendId, string firstName, string lastName, string email, string birthday, string streetAddress, string city, string zipCode, string country)
        {
            try
            {
                var friendResponse = await _friendsService.ReadFriendAsync(friendId, false);
                var existingFriend = friendResponse?.Item;

                if (existingFriend == null)
                {
                    ErrorMessage = "Friend not found.";
                    await OnGetAsync();
                    return Page();
                }
                DateTime? birthdayDate = null;
                if (!string.IsNullOrEmpty(birthday))
                {
                    if (DateTime.TryParse(birthday, out var parsedDate))
                    {
                        birthdayDate = parsedDate;
                    }
                    else
                    {
                        ErrorMessage = "Invalid birthday format.";
                        await OnGetAsync();
                        return Page();
                    }
                }
                var updateDto = new FriendCuDto
                {
                    FriendId = friendId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Birthday = birthdayDate,
                    AddressId = existingFriend.Address?.AddressId,
                    PetsId = existingFriend.Pets?.Select(p => p.PetId).ToList(),
                    QuotesId = existingFriend.Quotes?.Select(q => q.QuoteId).ToList()
                };
                updateDto.EnsureValidity();
                if (existingFriend.Address != null && (!string.IsNullOrEmpty(streetAddress) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(country) || !string.IsNullOrEmpty(zipCode)))
                {
                    if (!int.TryParse(zipCode, out int parsedZipCode))
                    {
                        ErrorMessage = "Invalid zip code format.";
                        await OnGetAsync();
                        return Page();
                    }
                    var addressDto = new AddressCuDto
                    {
                        AddressId = existingFriend.Address.AddressId,
                        StreetAddress = streetAddress,
                        City = city,
                        ZipCode = parsedZipCode,
                        Country = country,
                        FriendsId = existingFriend.Address.Friends?.Select(f => f.FriendId).ToList()
                    };
                    addressDto.EnsureValidity();
                    await _addressesService.UpdateAddressAsync(addressDto);
                }
                await _friendsService.UpdateFriendAsync(updateDto);

                SuccessMessage = "Friend updated successfully!";
                await OnGetAsync();
                return Page();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                await OnGetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeletePetAsync(Guid petId)
        {
            try
            {
                await _petsService.DeletePetAsync(petId);
                SuccessMessage = "Pet deleted successfully!";
                await OnGetAsync();
                return Page();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                await OnGetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while deleting the pet: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteQuoteAsync(Guid quoteId)
        {
            try
            {
                await _quotesService.DeleteQuoteAsync(quoteId);
                SuccessMessage = "Quote deleted successfully!";
                await OnGetAsync();
                return Page();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                await OnGetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while deleting the quote: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }
    }
}
