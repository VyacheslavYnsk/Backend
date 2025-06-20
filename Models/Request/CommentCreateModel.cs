using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class CommentCreateModel : Entity
{

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string Text { get; set; }

}