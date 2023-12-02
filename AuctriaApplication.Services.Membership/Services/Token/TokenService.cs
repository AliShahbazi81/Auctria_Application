using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuctriaApplication.DataAccess.Entities;
using AuctriaApplication.DataAccess.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AuctriaApplication.Services.Membership.Services.Token;

public class TokenService : ITokenService
{
    private readonly TokenConfig _jwtOptions;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        TokenConfig jwtOptions, 
        SignInManager<User> signInManager, 
        ILogger<TokenService> logger)
    {
        _jwtOptions = jwtOptions;
        _logger = logger;
        _signInManager = signInManager;
    }

    public async Task<string> GenerateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var claimsPrincipal = await _signInManager.ClaimsFactory.CreateAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.TokenKey)), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(claimsPrincipal.Claims),
            Expires = DateTime.UtcNow.AddDays(7)
        });

        var accessToken = tokenHandler.WriteToken(securityToken);
        return accessToken;
    }
}