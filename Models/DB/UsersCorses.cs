using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;
using Domain.Entities;

using Domain.Enums;

namespace Api.Models;

public class UserCorse: Entity
{
    public required Guid UserId { get; set; }

    public required Guid CourseId { get; set; }

    public required Role? Role { get; set; }


}