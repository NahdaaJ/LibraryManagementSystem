using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    internal class UserManager : DatabaseManager
    {
        private readonly string _tableName = "Users";
        internal void AddUser(User user)
        {
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var email = user.Email;
            var pin = user.GeneratePin();

            string addString = $"INSERT INTO {_tableName} (First_Name, Last_Name, Email, Pin) VALUES (@firstName,@lastName,@email,@pin);";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(addString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@pin", pin);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal void RemoveUser(string id)
        {
            string deleteString = @$"DELETE FROM {_tableName} WHERE ID = @id;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(deleteString, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal void EditUser(string firstName, string lastName, string pin) // Will use this in future update.
        {
            string editString = @$"";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(editString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@pin", pin);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal (MySqlDataReader, MySqlConnection) SearchUser(string searchTerm)
        {
            var connection = GetConnection();
            
            connection.Open();
            string searchString = $"SELECT * FROM {_tableName} WHERE First_Name LIKE @searchTerm OR Last_Name LIKE @searchTerm OR Email LIKE @searchTerm;";

            var command = new MySqlCommand(searchString, connection);
            
            command.Parameters.AddWithValue("@searchTerm", searchTerm);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);                      
        }
        internal (MySqlDataReader, MySqlConnection) GetUserInfo(string id)
        {
            var connection = GetConnection();

            connection.Open();
            string searchString = $"SELECT * FROM {_tableName} WHERE ID = @id;";

            var command = new MySqlCommand(searchString, connection);

            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
        }
        internal (MySqlDataReader, MySqlConnection) GetPin(string firstName, string lastName, string email)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = $"SELECT Pin FROM {_tableName} WHERE First_Name = @firstName AND Last_Name = @lastName AND Email = @email;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);
            command.Parameters.AddWithValue("@email", email);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr,connection);
        }
        internal (MySqlDataReader, MySqlConnection) GetID(int pin)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = $"SELECT ID FROM {_tableName} WHERE Pin = @pin;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@pin", pin);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
        }
        internal (MySqlDataReader, MySqlConnection) ViewAllUsers()
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = $"SELECT * FROM {_tableName} ORDER BY First_Name ASC;";

            var command = new MySqlCommand(searchString, connection);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
        }

    }
}
