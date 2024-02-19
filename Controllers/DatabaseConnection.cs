using MySql.Data.MySqlClient;
using System;

namespace Flashcards_api.Controllers
{
    public class DatabaseConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DatabaseConnection()
        {
            server = "localhost";
            database = "flashcards_api";
            uid = "flashcards_api";
            password = "test1234";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(MySqlException ex)
            {
                switch(ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch(MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<string>> Select(string query)
        {
            if(this.OpenConnection() == true)
            {
                List<string> results = new List<string>();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                    while (dataReader.Read())
                    {
                        string row = "";
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            if (dataReader.GetFieldType(i) == typeof(string))
                            {
                                row += dataReader.GetString(i) + ",";
                            }
                            else
                            {
                                row += dataReader.GetValue(i).ToString() + ",";
                            }
                        }
                        row = row.TrimEnd(',');
                        
                        results.Add(row);
                    }

                    dataReader.Close();

                    this.CloseConnection();
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return results;
                }

                return results;
            }
            else
            {
                return new List<string>();
            }
        }

        public async Task<int> Insert(string query)
        {
            if (this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Rows affected: " + rowsAffected);
                    }
                    else
                    {
                        Console.WriteLine("No rows affected");
                    }

                    this.CloseConnection();
                    return rowsAffected;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.Number == 1062)
                    {
                        Console.WriteLine("Duplicate entry");
                        return -2;
                    }
                    else
                    {
                        Console.WriteLine("Insert failed");
                        return -1;
                    }
                }

            }

            return -3;
        }

        public async Task<int> Update(string query)
        {
            if (this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Rows affected: " + rowsAffected);
                    }
                    else
                    {
                        Console.WriteLine("No rows affected");
                    }

                    this.CloseConnection();
                    return rowsAffected;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.Number == 1062)
                    {
                        Console.WriteLine("Duplicate entry");
                        return -2;
                    }
                    else
                    {
                        Console.WriteLine("Update failed");
                        return -1;
                    }
                }

            }

            return -3;
        }

        public async Task<int> Delete(string query)
        {
            if (this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Rows affected: " + rowsAffected);
                    }
                    else
                    {
                        Console.WriteLine("No rows affected");
                    }

                    this.CloseConnection();
                    return rowsAffected;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.Number == 1062)
                    {                        
                        Console.WriteLine("Duplicate entry");
                        return -2;
                    }
                    else
                    {
                        Console.WriteLine("Delete failed");
                        return -1;
                    }
                }

            }

            return -3;
        }

        public async Task<int> CreateTable(string query)
        {
            if(this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if(rowsAffected > 0)
                    {
                        Console.WriteLine("Rows affected: " + rowsAffected);
                    }
                    else
                    {
                        Console.WriteLine("No rows affected");
                    }

                    this.CloseConnection();
                    return rowsAffected;
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.Number == 1050)
                    {
                        Console.WriteLine("Table already exists");
                        return -2;
                    }
                    else
                    {
                        Console.WriteLine("Table creation failed");
                        return -1;
                    }
                }

            }

            return -3;
        }

        public async Task<int> DropTable(string query)
        {
            if (this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine("Rows affected: " + rowsAffected);

                    if (rowsAffected == -1)
                    {
                        Console.WriteLine("Rows affected: " + rowsAffected);
                    }
                    else
                    {
                        Console.WriteLine("No rows affected");
                    }

                    this.CloseConnection();
                    return rowsAffected;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                    if (ex.Number == 1051)
                    {
                        Console.WriteLine("Table does not exist");
                        return -2;
                    }
                    else
                    {
                        Console.WriteLine("Table deletion failed");
                        return -1;
                    }
                }
            }

            return -3;
        }
        
        public void AlterTable(string query)
        {
            if(this.OpenConnection() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    this.CloseConnection();
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
