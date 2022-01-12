using APP.Base.Model.Entity;
using APP.Base.Model.Enum;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APP.Auth.Model.Entity
{
    public class ApplicationUser : IdentityUser<int>
    {
    
        [StringLength(250)]
        public string FirstName { get; set; }
        
        [StringLength(250)]
        public string LastName { get; set; }
        
        [NotMapped]
        public string Name => $"{this.FirstName} {this.LastName}";

        public Status Status { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        [DataType(DataType.DateTime)]
        public int CreatedBy { get; set; }
        public string Avatar { get; set; }
        public string Theme { get; set; }
        
        [ForeignKey("Company")]
        public long CompanyId { get; set; }
        public Company Company { get; set; }
        public virtual ICollection<ApplicationUserProduct> ApplicationUserProducts { get; set; }
    }
}
