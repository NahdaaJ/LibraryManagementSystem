using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    internal class DatabaseManager
    {
        private static readonly string _databaseName = "LibraryManager";
        private static readonly string _connectionString = Helper.ConnVal("DBExists");
        private static readonly string _initialisationString = Helper.ConnVal("DBDoesNotExist");

        internal void InitialiseDatabase()
        {
            using (var connection = new MySqlConnection(_initialisationString))
            {
                connection.Open();

                var createDatabaseQuery = $"CREATE DATABASE IF NOT EXISTS {_databaseName};";

                using (var command = new MySqlCommand(createDatabaseQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var createBookTable = @"CREATE TABLE IF NOT EXISTS Books (
                                        ID INT AUTO_INCREMENT PRIMARY KEY,
                                        Title VARCHAR(255),
                                        Author VARCHAR(255),
                                        Description TEXT,
                                        Genre VARCHAR(255),
                                        Publication_Date DATE,
                                        Available VARCHAR(5)
                                        );";

                var createUserTable = @"CREATE TABLE IF NOT EXISTS Users (
                                        ID INT AUTO_INCREMENT PRIMARY KEY,
                                        First_Name VARCHAR(255),
                                        Last_Name VARCHAR(255),
                                        Email VARCHAR(255),
                                        Pin INT
                                        );";

                var createLoanTable = @"CREATE TABLE IF NOT EXISTS Loans (
                                        ID INT AUTO_INCREMENT PRIMARY KEY,
                                        User_ID INT,
                                        Title VARCHAR(255),
                                        Author VARCHAR(255),
                                        Date_Loaned DATE,
                                        Date_Due DATE,
                                        Date_Returned DATE
                                        );";

                using (var command = new MySqlCommand(createBookTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new MySqlCommand(createUserTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new MySqlCommand(createLoanTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        protected MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            return connection;
        }

        protected int ValidateUser(string firstName, string lastName, int pin)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string validateString = $"SELECT COUNT(*) FROM Users WHERE First_Name COLLATE utf8mb4_general_ci = @firstName AND Last_Name COLLATE utf8mb4_general_ci = @lastName AND Pin = @pin;";
                using (var command = new MySqlCommand(validateString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@pin", pin);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return count;
                }
            }
        }
        internal int CheckPinDistinct(int pin)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string searchString = $"SELECT COUNT(*) FROM Users WHERE Pin = {pin};";
                using (var command = new MySqlCommand(searchString, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return count;
                }

            }
        }
    }
}
