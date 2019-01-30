using System.IdentityModel.Tokens.Jwt;

namespace FirstCateringAPI.BusinessLogic.Contracts
{
    public interface IAuthLogic
    {
        string[] GetUsernameAndPassword(string authHeader);

        JwtSecurityToken CreateSecurityToken(string username);

        bool AuthorizedUser(string username, string password);
    }
}