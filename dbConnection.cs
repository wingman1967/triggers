using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using ConfigureOneFlag;
using System.Data.SqlClient;
using System.Configuration;
using ConfigureOneFlag;

namespace Triggers
{
    public class DbConnection : abDatabase
    {
        private SqlConnection _connection = null;
        private SqlCommand _command = null;

        public override SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(DatabaseFactory.connectionString);
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }
        public override SqlCommand Command
        {
            get
            {
                if (_command == null)
                {
                    _command = new SqlCommand();
                    _command.Connection = Connection;
                }
                return _command;
            }
            set
            {
                _command = value;
            }
        }
    }
}
