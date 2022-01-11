using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model
{
   public class BaseContext:DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options):base(options)
        {

        }
    }
}
