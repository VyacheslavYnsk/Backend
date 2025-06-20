using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class UserLoginModel
{
    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }


}