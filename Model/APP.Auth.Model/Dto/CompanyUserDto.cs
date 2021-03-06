using APP.Base.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Auth.Model.Dto
{
    public class CompanyUserDto
    {   
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }     
        public string Avatar { get; set; }
        public string Theme { get; set; }
        public string AdminUserName { get; set; }

    }
}
