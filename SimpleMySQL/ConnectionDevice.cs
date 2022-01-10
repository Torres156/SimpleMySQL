using MySql.Data.MySqlClient;
using System;

namespace SimpleMySQL
{
    public class ConnectionDevice
    {
        string Ip = "";
        int Port = 3306;
        string User = "";
        string Password = "";
        string Database = "";

        bool connectionInitialize = false;
        bool checkDatabase = false;
        string connStr = "";

        public MySqlConnection Connection { get; private set; }

        public ConnectionDevice(string Database, string Ip, int Port, string User, string Password, string args = "")
        {
            this.Database = Database;
            this.Ip = Ip;
            this.Port = Port;
            this.User = User;
            this.Password = Password;

            connStr = $"server={Ip};port={Port};user id={User};pwd={Password};" + args;
            InitializeConnection();
            CheckDatabase();
        }

        public void CheckDatabase()
        {
            Open();
            ExecuteNonQuery($"CREATE DATABASE IF NOT EXISTS {Database};");            
            checkDatabase = true;
            Close();
        }


        public bool InitializeConnection()
        {
            try
            {
                Connection = new MySqlConnection(connStr);
                connectionInitialize = true;
            }
            catch
            {
                connectionInitialize = false;
                return false;
            }
            return true;
        }

        public void Open()
        {
            if (!connectionInitialize && !InitializeConnection())
                return;

            Connection.Open();

            if (checkDatabase)
                ExecuteNonQuery($"USE {Database}");
        }

        public void Close()
        {
            if (!connectionInitialize)
                return;

            Connection.Close();
        }

        public SimpleUpdate CreateUpdateCommand()
            => new SimpleUpdate(this);

        public SimpleInsert CreateInsertCommand()
            => new SimpleInsert(this);

        public SimpleSelect CreateSelectCommand()
            => new SimpleSelect(this);

        public int ExecuteNonQuery(string str)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = str;
            var result = cmd.ExecuteNonQuery();    
            return result;
        }
    }
}
