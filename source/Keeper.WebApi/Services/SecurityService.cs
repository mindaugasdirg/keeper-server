using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Keeper.WebApi.Services
{
    public class SecurityService : ISecurityService
    {
        private const string usernameSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
        private IConfiguration configuration;

        public SecurityService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public string GetRandomString(int minLength, int maxLength)
        {
            var length = GetRandomInt(minLength, maxLength);
            var str = new StringBuilder();
            for(int i = 0; i < length; ++i)
            {
                str.Append(GetRandomSymbol());
            }
            return str.ToString();
        }

        public char GetRandomSymbol() => usernameSymbols[GetRandomByte(Convert.ToByte(usernameSymbols.Length))];

        public int GetRandomInt(int min, int max)
        {
            var boundries = BitConverter.GetBytes(max - min);
            var value = new byte[boundries.Length];
            for(int i = 0; i < value.Length; ++i)
            {
                value[i] = GetRandomByte(boundries[i]);
            }
            return BitConverter.ToInt32(value) + min;
        }

        public byte GetRandomByte(byte max)
        {
            if(max == 0) // interval is [0; 0]
                return 0;
            var value = new byte[1];
            var fair = false;
            while(!fair)
            {
                rand.GetBytes(value);
                fair = IsFair(value[0], max);
            }
            return (byte)((int)value[0] % (int)max);
        }

        public string GenerateJwtToken(IdentityUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(configuration["Issuer"], configuration["Issuer"], claims, DateTime.Now, DateTime.Now.Add(TimeSpan.FromHours(2)), creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsFair(byte value, byte maxValue)
        {
            int sets = byte.MaxValue / maxValue;
            return value < sets * maxValue;
        }
    }
}