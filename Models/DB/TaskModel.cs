using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class TaskModel : Entity
{
    public required string Name { get; set; }

    public required string Description { get; set; }


    public required Guid AuthorId { get; set; }

    public required DateTime CreateTime { get; set; }

    public required List<CommentModel> Comments { get; set; }

}