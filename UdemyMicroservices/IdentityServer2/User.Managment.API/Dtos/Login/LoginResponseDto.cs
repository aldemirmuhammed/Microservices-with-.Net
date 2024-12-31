using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Management.API.Dtos.Authentication.User;

namespace User.Management.API.Dtos.Login
{
    public class LoginResponseDto
    {
        public TokenType AccessToken { get; set; }
        public TokenType RefreshToken { get; set; }

    }
}
