using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Api.Models;

public class CourseCreateModel
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]

    public required string Chapter { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]

    public required string Subject { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]

    public required string Audience  { get; set; }

}