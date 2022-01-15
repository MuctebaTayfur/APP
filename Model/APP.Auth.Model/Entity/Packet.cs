using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model.Entity
{
    public class Packet : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public int UserAmount { get; set; }
        public int PacketTime { get; set; }            
        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public Product Product { get; set; }


    }
}
