using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMySQL
{
    public class SimpleInsert
    {
        ConnectionDevice Connection;

        string table_name = "";
        Dictionary<string, object> args = new Dictionary<string, object>();
        string where = "";

        internal SimpleInsert(ConnectionDevice Connection)
        {
            this.Connection = Connection;
        }

        public SimpleInsert SetTable(string table_name)
        {
            this.table_name = table_name;
            return this;
        }

        public SimpleInsert Add(string column, object value)
        {
            args.Add(column, value);
            return this;
        }

        public SimpleInsert SetWhere(string column, object value)
        {
            where = column + "=" + (value is string ? $"'{value.ToString()}'" : value.ToString());
            return this;
        }

        public SimpleInsert SetWhere(string str)
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
            var str = $"INSERT INTO {table_name} (";

            // Set the columns
            foreach (var i in args)
                str += i.Key + ",";

            if (str.EndsWith(","))
                str = str.Substring(0, str.Length - 1); // Remove the last ,

            str += ") VALUES(";

            // Set the values
            foreach (var i in args)
                str += (i.Value is string ? $"'{i.Value}'" : i.Value.ToString()) + ",";

            if (str.EndsWith(","))
                str = str.Substring(0, str.Length - 1); // Remove the last ,

            str += ") ";

            // Set where requeriments
            if (where.Length > 0)
                str += "WHERE " + where;

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
