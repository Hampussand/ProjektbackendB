using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Interfaces;


namespace AppRazor.Pages;

public class SeedModel : PageModel
{
    private readonly ILogger<SeedModel> _logger;
    readonly IAdminService _admin_service = null;
    public SeedModel(ILogger<SeedModel> logger, IAdminService admin_service)
    {
        _logger = logger;
        _admin_service = admin_service;
    }

    public void OnGet()
    {
        
    }
    public async Task<IActionResult> OnPost()
        {
            await _admin_service.RemoveSeedAsync(true);
            await _admin_service.SeedAsync(100);

            return Redirect($"/Index");
        }
}