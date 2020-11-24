using System;
using System.Collections.Generic;

namespace UserApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Password { get; set; }
        
        public string Email { get; set; }
        
        public string Birthday { get; set; }
        
        public string UsersRole { get; set; }

        public DateTime UpdatedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public List<string> PermissionList { get; set; }
    }
}