using System.ComponentModel.DataAnnotations;
using Domain.Enums;
public class TokenResponseModel {

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string Token {get; set;}


}


public interface ITokenRevocationService
{
    bool RevokeToken(string token);
    bool IsTokenRevoked(string token);
}

public class TokenRevocationService : ITokenRevocationService
{
    private readonly List<RevokedToken> _revokedTokens = new();

    public bool RevokeToken(string token)
    {
        if (IsTokenRevoked(token))
        {
            return false;
        }
        
        _revokedTokens.Add(new RevokedToken { Token = token, RevokedAt = DateTime.UtcNow });
        return true; 
    }

    public bool IsTokenRevoked(string token)
    {
        return _revokedTokens.Any(rt => rt.Token == token);
    }
}