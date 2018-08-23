using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using ConfigureOneFlag;

namespace Triggers
{
    public abstract class abDatabase
    {
        public virtual SqlConnection Connection { get; set; }
        public virtual SqlCommand Command { get; set; }
    }
}
