using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matcha.API.Dtos
{
    public class UserForResetDto
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
