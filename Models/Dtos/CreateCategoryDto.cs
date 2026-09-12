using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    [MinLength(3, ErrorMessage = "Name cannot be at least 3 characters long")]
    public string Name { get; set; } = string.Empty;
}
