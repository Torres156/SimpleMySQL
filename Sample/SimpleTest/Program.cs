using SimpleMySQL;
using System;

namespace SimpleTest
{
    class Program
    {
        static ConnectionDevice Device;
        const string TABLE_TEST = "simpletesting";

        static void Main(string[] args)
        {
            Console.WriteLine("Initialize MySQL Device.");
            Device = new ConnectionDevice("Test", "localhost", 3306, "root", "156156");
            Device.Open();
            Console.WriteLine($"Verify table <{TABLE_TEST}>");
            Device.ExecuteNonQuery(@$"CREATE TABLE IF NOT EXISTS {TABLE_TEST}(
arg1 TEXT, arg2 BOOL, arg3 INT
)");

            Device.Close();

            Console.WriteLine("");
            Console.WriteLine("Commands:");
            Console.WriteLine("update <arg1:TEXT> <arg2:false or true> <arg3:INT>");
            Console.WriteLine("select optional<argName:arg1|arg2|arg3> optional<WHERE arg1Value>");
            Console.WriteLine("insert <arg1:TEXT> <arg2:false or true> <arg3:INT>");
            Console.WriteLine("");

            string cmd = "";
            while ((cmd = Console.ReadLine()) != "exit")
            {
                var cmdSplit = cmd.Split();

                switch (cmdSplit[0].ToLower())
                {
                    case "insert":
                        if (cmdSplit.Length < 4)
                        {
                            Console.WriteLine("insert <arg1:TEXT> <arg2:false or true> <arg3:INT>");
                            Console.WriteLine("");
                            break;
                        }

                        var insert = Device.CreateInsertCommand();
                        insert.SetTable(TABLE_TEST)
                            .Add("arg1", cmdSplit[1])
                            .Add("arg2", bool.Parse(cmdSplit[2]))
                            .Add("arg3", int.Parse(cmdSplit[3]));

                        Device.Open();
                        try
                        {
                            insert.Execute();
                            Console.WriteLine($"The value {cmdSplit[1]} has been inserted!");
                            Console.WriteLine("");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("");
                        }
                        Device.Close();
                        break;

                    case "update":
                        if (cmdSplit.Length < 4)
                        {
                            Console.WriteLine("update where:<arg1:TEXT> <arg2:false or true> <arg3:INT>");
                            Console.WriteLine("");
                            break;
                        }

                        var update = Device.CreateUpdateCommand();
                        update.SetTable(TABLE_TEST)
                            .Add("arg2", bool.Parse(cmdSplit[2]))
                            .Add("arg3", int.Parse(cmdSplit[3]))
                            .SetWhere("arg1", cmdSplit[1]);
                        Device.Open();
                        try
                        {
                            update.Execute();
                            Console.WriteLine($"The value {cmdSplit[1]} has been updated!");
                            Console.WriteLine("");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("");
                        }
                        Device.Close();
                        break;

                    case "select":
                        var select = Device.CreateSelectCommand();
                        select.AddTable(TABLE_TEST);

                        if (cmd.Contains("where", StringComparison.OrdinalIgnoreCase))
                        {
                            var str = cmd.Remove(0, "select".Length);
                            str = str.Substring(0, str.IndexOf("where"));
                            str = str.Trim();

                            if (str.Length > 0)
                            {
                                var s = str.Split();
                                foreach (var i in s)
                                    if (i == "arg1" || i == "arg2" || i == "arg3")
                                        select.AddColunm(i);
                                    else
                                        Console.WriteLine(i + " is not valid! Use 'arg1', 'arg2', 'arg3'");
                            }
                            select.SetWhere("arg1", cmd.Substring(cmd.IndexOf("where") + "where".Length).Trim());
                        }
                        else
                        {
                            var str = cmd.Remove(0, "select".Length);
                            str = str.Trim();

                            if (str.Length > 0)
                            {
                                var s = str.Split();
                                foreach (var i in s)
                                    if (i == "arg1" || i == "arg2" || i == "arg3")
                                        select.AddColunm(i);
                                    else
                                        Console.WriteLine(i + " is not valid! Use 'arg1', 'arg2', 'arg3'");
                            }
                        }

                        Device.Open();
                        var read = select.Execute();
                        while(read.Read())
                        {
                            for (int i = 0; i < read.FieldCount; i++)
                                Console.WriteLine(read.GetName(i) + ": " + read.GetValue(i));

                            Console.WriteLine("");
                        }
                        Device.Close();
                        break;
                }
            }
        }
    }
}
