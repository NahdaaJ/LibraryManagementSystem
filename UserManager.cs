using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

            string addString = $"INSERT INTO {_tableName} (First_Name, Last_Name, Email, Pin) VALUES ('@firstName','@lastName','@email',@pin);";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(addString, connection))
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
        internal void RemoveUser(string firstName, string lastName, int pin)
        {
            string deleteString = @$"DELETE FROM {_tableName} WHERE First_Name COLLATE NOCASE = @firstName AND Last_Name COLLATE NOCASE = @Last_Name AND Pin = @pin;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(deleteString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@pin", pin);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal void EditUser(string firstName, string lastName, string pin)
        {
            string editString = @$"";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(editString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@pin", pin);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal SQLiteDataReader SearchUser(string firstName, string lastName)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string searchString = $"SELECT * FROM {_tableName} WHERE First_Name = @firstName AND Last_Name = @lastName;";

                using (var command = new SQLiteCommand(searchString, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);

                    SQLiteDataReader rdr = command.ExecuteReader();
                    return rdr;
                }
            }
        }
    }
}
