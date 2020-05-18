using System.Threading.Tasks;
using Keeper.WebApi.Models;

namespace Keeper.WebApi.Services
{
    public interface IUsersService
    {
        Task<string> CreateAsync(string secret);
        Task<bool> ExistsAsync(string username);
        Task<string> LoginAsync(LoginForm login);
    }
}