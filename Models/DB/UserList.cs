using Domain.Entities;

public class UserList
{
    public required UserDto Owner { get; set; }
    public required List<UserDto> Teachers { get; set; }
    public required List<UserDto> Students { get; set; }

}


