using APP.Auth.Model.Entity;
using APP.Base.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model.Model
{
    public class ProductModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Packet> Packets { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
