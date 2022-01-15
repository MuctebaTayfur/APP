using APP.Base.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Auth.Model.Dto
{
    public class ProductUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long ProductId { get; set; }        
        public long PacketId { get; set; }
        public DateTime? PacketStartDate { get; set; }
      
    }
}
