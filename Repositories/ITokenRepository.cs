using Microsoft.AspNetCore.Identity;

namespace ThucHanhWEBAPI.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}