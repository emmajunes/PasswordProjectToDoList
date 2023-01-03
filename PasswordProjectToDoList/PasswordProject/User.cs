using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace PasswordProject
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Access { get; set; }
        public Guid UserId { get; set; }

    }
}
