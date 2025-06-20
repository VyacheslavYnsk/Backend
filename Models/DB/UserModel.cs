using Domain.Abstractions;
using Microsoft.VisualBasic;
using Domain.Enums;
namespace Domain.Entities;

public class UserModel : Entity
{
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly Birthday { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }

    public void Update (string firstName, string middleName , string lastName) 
    {
       FirstName = firstName;
       MiddleName = middleName;
       LastName = lastName;

    }
}