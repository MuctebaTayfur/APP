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
        public long Id { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public long? DeletedBy { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }     
        public string CompanyAvatar { get; set; }
        public string UserAvatar { get; set; }
        public string Theme { get; set; }

    }
}
