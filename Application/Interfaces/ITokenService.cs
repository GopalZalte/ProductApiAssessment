using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username);

        string GenerateRefreshToken();

        void SaveRefreshToken(string username, string refreshToken);

        bool ValidateRefreshToken(string refreshToken);

        string? GetUsernameFromRefreshToken(string refreshToken);

        void RemoveRefreshToken(string refreshToken);
    }
}