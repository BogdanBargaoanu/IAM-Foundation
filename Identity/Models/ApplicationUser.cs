using Microsoft.AspNetCore.Identity;

namespace Identity.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? EmployeeId { get; set; }
    public string? PinHash { get; set; }
}
