using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Net.NetworkInformation;

namespace LibraryManagementSystem
{
    internal class DatabaseManager
    {
        private static readonly string _databaseName = "LibraryManager.db";
        private static readonly string _connectionString = $@"Data Source={_databaseName};Pooling=True;";
        internal void InitialiseDatabase()
        {
            if (!File.Exists(_databaseName))
            {
                SQLiteConnection.CreateFile(_databaseName);
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var CreateBookTable = @"CREATE TABLE IF NOT EXISTS Books (ID INTEGER PRIMARY KEY AUTOINCREMENT, Title TINYTEXT, Author TINYTEXT, 
                                       Description MEDIUMTEXT, Genre TINYTEXT,Publication_Date DATE, Available VARCHAR(5));";

                var CreateUserTable = @"CREATE TABLE IF NOT EXISTS Users (ID INTEGER PRIMARY KEY AUTOINCREMENT, First_Name TINYTEXT, 
                                        Last_Name TINYTEXT, Email TINYTEXT, Pin INTEGER);";

                var CreateLoanTable = @"CREATE TABLE IF NOT EXISTS Loans (ID INTEGER PRIMARY KEY AUTOINCREMENT, pin INTEGER,
                                        Title TINYTEXT, Author TINYTEXT, Date_Loaned DATE, Date_Due DATE, Date_Returned DATE);";

                using (var command = new SQLiteCommand(CreateBookTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(CreateUserTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(CreateLoanTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        protected SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            return connection;
        }

        protected int ValidateUser(string firstName, string lastName, int pin)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string searchString = $"SELECT COUNT(*) FROM Users WHERE First_Name COLLATE NOCASE = @firstName AND Last_Name COLLATE NOCASE = @lastName AND Pin = @pin;";
                using (var command = new SQLiteCommand(searchString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@pin", pin);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
        }
        internal int CheckPinDistinct(int pin)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string searchString = $"SELECT COUNT(*) FROM Users WHERE Pin = {pin};";
                using (var command = new SQLiteCommand(searchString, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }

            }
        }
    }
}
