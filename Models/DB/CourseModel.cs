using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class CourseModel : Entity
{
    public required string TeachersCode { get; set; }

    public required string StudentsCode { get; set; }

    public required string Name { get; set; }

    public required string Chapter { get; set; }

    public required string Subject { get; set; }

    public required string Audience { get; set; }


    public required DateTime CreateTime { get; set; }

    public virtual ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();

}