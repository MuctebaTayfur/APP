using APP.Base.Model.Base;
using APP.Base.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Auth.Model.Entity
{
    public class ApplicationUserProduct:BaseClass
    {
          
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }    
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long? CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime ProductStartDate { get; set; }
        public DateTime ProductEndDate { get; set; }
        public long PacketId { get; set; }
    }
}
