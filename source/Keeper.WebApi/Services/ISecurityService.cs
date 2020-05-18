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
    public interface ISecurityService
    {
        string GetRandomString(int minLength, int maxLength);
        char GetRandomSymbol();
        int GetRandomInt(int min, int max);
        byte GetRandomByte(byte max);
        string GenerateJwtToken(IdentityUser user);
    }
}