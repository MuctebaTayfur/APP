
using APP.Base.Model.Entity;

namespace APP.Auth.Model.Entity
{
    public class UserRegions : BaseEntity
    {
        public long  RegionId { get; set; }
        public string UserName { get; set; }
    }
}
