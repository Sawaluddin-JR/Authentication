using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authentication.Dtos
{
    public class RegisterDto
    {
        public string Name { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
    }
}