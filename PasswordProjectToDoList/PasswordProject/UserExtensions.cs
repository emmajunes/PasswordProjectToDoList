using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordProject
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this User user)
        {
            return user.Access.ToLower() == "admin";
        }
    }
}
