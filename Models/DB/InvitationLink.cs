namespace Domain.Entities;
using Domain.Enums;
public class InvitationLink
{
    public required Guid Id { get; set; }
    public required Role Role { get; set; }

}