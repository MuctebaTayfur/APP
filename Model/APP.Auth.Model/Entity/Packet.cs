﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model.Entity
{
    public class Packet : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Cost { get; set; }
        public int UserAmount { get; set; }
        public long PacketTime { get; set; }
        public DateTime PacketStartTime { get; set; }
        public DateTime PacketEndTime { get; set; }


    }
}
