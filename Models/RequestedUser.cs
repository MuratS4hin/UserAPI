using System;
using System.Collections.Generic;

namespace UserApi.Models
{
    public class RequestedUser 
    {
        
        public string Name {  get; set; }
        
        public string Password {  get; set; }
        
        public string Email { get; set; }
        
        public string UsersRole { get; set; }
        
        public string Birthday { get; set; }
        
        public User SetRequestedUserToUser()
        {
            var permissionList = new List<string>();
            switch (UsersRole)
            {
                case "User":
                    permissionList.Add("Get");
                    break;
                case "Admin":
                    permissionList.Add("Download");
                    permissionList.Add("Upload");
                    permissionList.Add("Update");
                    break;
            }
            
            var userDto = new User
            {
                UsersRole = UsersRole,
                Name = Name,
                Birthday = Birthday,
                Email = Email,
                Password = Password,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PermissionList = permissionList
            };

            return userDto;
        }
        
    }
}