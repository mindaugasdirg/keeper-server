using System.Threading.Tasks;
using Keeper.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace Keeper.WebApi.Services
{
    public class UsersService : IUsersService
    {
        private DatabaseContext context;
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> usersManager;
        private ISecurityService securityService;

        public UsersService(DatabaseContext _context, SignInManager<IdentityUser> _signInManager, 
                            UserManager<IdentityUser> _usersManager, ISecurityService _securityService)
        {
            context = _context;
            signInManager = _signInManager;
            usersManager = _usersManager;
            securityService = _securityService;
        }

        public async Task<string> CreateAsync(string secret)
        {
            var username = string.Empty;
            var foundNew = false;
            while(!foundNew)
            {
                username = securityService.GetRandomString(10, 15);
                foundNew = await usersManager.FindByNameAsync(username) is null;
            }

            var user = new IdentityUser() { UserName = username };
            var result = await usersManager.CreateAsync(user, secret);

            if(result.Succeeded)
                return username;
            return string.Empty;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            var found = await usersManager.FindByNameAsync(username);
            return !(found is null);
        }

        public async Task<string> LoginAsync(LoginForm login)
        {
            var result = await signInManager.PasswordSignInAsync(login.Key, login.Secret, false, false);

            if(!result.Succeeded)
                return "";

            var user = await usersManager.FindByNameAsync(login.Key);
            return securityService.GenerateJwtToken(user);
        }
    }
}