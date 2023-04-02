namespace Backend.Data.Dtos.Auth;

public class RefreshTokenDto
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public RefreshTokenDto(string refreshToken, DateTime refreshTokenExpiration)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiration = refreshTokenExpiration;
    }
}
