using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMySQL
{
    public class SimpleSelect
    {
        ConnectionDevice Connection;

        List<string> tables = new List<string>();
        List<string> columns = new List<string>();
        string where = "";

        internal SimpleSelect(ConnectionDevice Connection)
        {
            this.Connection = Connection;
        }

        public SimpleSelect AddTable(string table)
        {
            tables.Add(table);
            return this;
        }

        public SimpleSelect AddColunm(string column)
        {
            columns.Add(column);
            return this;
        }

        public SimpleSelect AddColunm(int tableIndex, string column)
        {
            columns.Add("t" + tableIndex + "." + column);
            return this;
        }

        public SimpleSelect SetWhere(string colunm, object value)
        {
            where = colunm + "=" + (value is string ? $"'{value.ToString()}'" : value.ToString());
            return this;
        }

        public SimpleSelect SetWhere(string str)
        {
            where = str;
            return this;
        }

        public MySqlDataReader Execute()
        {

            var str = "SELECT ";

            if (columns.Count > 0)
            {
                // Set the columns and values
                foreach (var i in columns)
                    str += i + ",";

                if (str.EndsWith(","))
                    str = str.Substring(0, str.Length - 1); // Remove the last ,
            }
            else
                str += "*";

            str += " ";

            if (tables.Count > 0)
            {
                str += "FROM ";
                foreach (var i in tables)
                    str += i + " AS t" + tables.IndexOf(i) + ",";

                if (str.EndsWith(","))
                    str = str.Substring(0, str.Length - 1); // Remove the last ,

                str += " ";
            }

            // Set where requeriments
            if (where.Length > 0)
                str += "WHERE " + where;

            str += ";";

            try
            {
                var command = Connection.Connection.CreateCommand();
                command.CommandText = str;
                var reader = command.ExecuteReader();

                // Close the connection
                //Connection.Close();

                return reader;
            }
            catch
            {                
                return null;
            }
        }
    }
}
