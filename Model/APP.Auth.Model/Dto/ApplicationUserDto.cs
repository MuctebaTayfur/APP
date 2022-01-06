using System;
using System.Collections.Generic;
using APP.Base.Model.Enum;


namespace APP.Auth.Model.Dto
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Status? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public string UserName { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<AuthorizedFolderDto>? AuthorizedFolders { get; set; }
        public string Avatar { get; set; }
        public string Theme { get; set; }

    }
}
