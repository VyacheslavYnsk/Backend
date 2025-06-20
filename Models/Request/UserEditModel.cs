using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class UserEditModel
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string MiddleName { get; set; }

     [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string LastName { get; set; }
  
}