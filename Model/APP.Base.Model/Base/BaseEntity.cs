using APP.Base.Model.Base;
using APP.Base.Model.Enum;
using System;


namespace APP.Base.Model.Entity
{
   public class BaseEntity: BaseClass
    {
        public long Id { get; set; }

        public Status Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public long? DeletedBy { get; set; }
    }
}
