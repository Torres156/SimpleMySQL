using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMySQL
{
    public class SimpleUpdate
    {
        ConnectionDevice Connection;

        string table_name = "";
        Dictionary<string, object> args = new Dictionary<string, object>();
        string where = "";        

        internal SimpleUpdate(ConnectionDevice Connection)
        {
            this.Connection = Connection;
        }

        public SimpleUpdate SetTable(string table_name)
        {
            this.table_name = table_name;
            return this;
        }

        public SimpleUpdate Add(string column, object value)
        {
            args.Add(column, value);
            return this;
        }

        public SimpleUpdate SetWhere(string column, object value)
        {
            where = column + "=" + (value is string ?  $"'{value.ToString()}'" : value.ToString());            
            return this;
        }

        public SimpleUpdate SetWhere(string str)
        {
            where = str;
            return this;
        }

        public void Clear()
        {
            table_name = "";
            args.Clear();
            where = "";
        }

        public void Execute()
        {
            // Set the table that will be updated
            var str = $"UPDATE {table_name} SET ";

            // Set the columns and values
            foreach (var i in args)
                str += i.Key + "=" + (i.Value is string ? $"'{i.Value.ToString()}'" : i.Value.ToString()) + ",";

            if (str.EndsWith(","))
                str = str.Substring(0, str.Length - 1); // Remove the last ,

            // Set where requeriments
            if (where.Length > 0)
                str += " WHERE " + where;

            try
            {
                var command = Connection.Connection.CreateCommand();
                command.CommandText = str;
                command.ExecuteNonQuery();
            }
            catch
            { }

      
        }
    }
}
