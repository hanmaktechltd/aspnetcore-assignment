using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class ServiceProviderViewModel
{
    public int ServiceProviderId { get; set; }

    [Required]
    public string Username { get; set; }

    public string Role { get; set; }

    public List<SelectListItem>? Roles { get; set; }

    [AtLeastOneElement(ErrorMessage = "Please select at least one service point.")]
    public List<int> SelectedServicePointIds { get; set; }

    public List<SelectListItem>? AllServicePoints { get; set; }
}
