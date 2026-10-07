using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Models;

//test
public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}
