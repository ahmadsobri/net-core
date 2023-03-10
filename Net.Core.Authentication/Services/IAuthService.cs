using Net.Core.Authentication.Entities;
using Net.Core.Authentication.Models;
using System.Threading.Tasks;

namespace Net.Core.Authentication.Services
{
    public interface IAuthService
    {
        Task<User> FindByUserName(string username);
        Task<Response> CreateUser(Register user);
        Task<User> FindById(int id);
        Task<User> FindByAccessToken(string token);
        Task<User> ValidateRefreshToken(RefreshToken request);
        Task<User> ValidateAccessToken(AccessToken request);
        Task<User> ValidateBasicToken(string accountNumber, string key);
        Task<TokenResponse> CreateToken(User user);
        Task<Response> Auth(Request request);
    }
}
