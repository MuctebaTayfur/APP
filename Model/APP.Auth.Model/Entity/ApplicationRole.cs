using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace APP.Auth.Model.Entity
{
    public class ApplicationRole : IdentityRole<int>
    {
        [StringLength(250)]
        public string Description { get; set; }

        public string RoleUniqueId { get; set; }
    }
}
