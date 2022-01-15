using APP.Base.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model.Dto
{
    public class PacketDto
    {
        public long Id { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public long? DeletedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public int UserAmount { get; set; }
        public long PacketTime { get; set; }
        public long ProductId { get; set; }
    }
}
