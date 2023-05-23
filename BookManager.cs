using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class BookManager : DatabaseManager
    {
        private readonly string _tableName = "Books";
        internal void AddBook(Book book)
        {
            var title = book.Title;
            var author = book.Author;
            var description = book.Description;
            var genre = book.Genre;
            var publicationDate = book.PublicationDate;

            string addString = @$"INSERT OR IGNORE INTO {_tableName} (Title, Author, Description, Genre, Publication_Date, Available) 
                                    VALUES ('@title','@author','@description', '@genre', '@publicationDate', 'Yes');";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(addString, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@genre", genre);
                    command.Parameters.AddWithValue("@publicationDate", publicationDate);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal void RemoveBook(string title, string author)
        {
            string deleteString = @$"DELETE FROM {_tableName} WHERE Title COLLATE NOCASE = @title AND Author COLLATE NOCASE = @author;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(deleteString, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal void EditAvailability(string title, string author, string availability)
        {
            string editString = @$"UPDATE {_tableName} SET Available = @availability WHERE Title = @title AND Author = @author;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(editString, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@availability", availability);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal (SQLiteDataReader,SQLiteConnection) SearchBook(string searchTerm)
        {
            var connection = GetConnection();
            connection.Open();
            string insertString = $"SELECT * FROM {_tableName} WHERE Title LIKE @searchTerm OR Author LIKE @searchTerm OR Genre LIKE @searchTerm OR Available LIKE @searchTerm;";

            var command = new SQLiteCommand(insertString, connection);
            command.Parameters.AddWithValue("@searchTerm", searchTerm);

            SQLiteDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
            
        }
        internal (SQLiteDataReader, SQLiteConnection) GetBookInfo(string id)
        {
            var connection = GetConnection();            
            connection.Open();

            string searchString = $@"SELECT * FROM {_tableName} WHERE ID = @id;";
            var command = new SQLiteCommand(searchString, connection);
                
            command.Parameters.AddWithValue("@id", id);
            var rdr = command.ExecuteReader();
            return (rdr, connection);
            
        }
        internal (SQLiteDataReader, SQLiteConnection) GetBookInfo(string title, string author)
        {
            var connection = GetConnection();            
            connection.Open();

            string searchString = $"SELECT * FROM {_tableName} WHERE Title = @title AND Author = @author;";
            var command = new SQLiteCommand(searchString, connection);

            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@author", author);

            var rdr = command.ExecuteReader();
            return (rdr, connection);

        }
        internal int CheckBookExists(string id, string available) // new close
        {
            var connection = GetConnection();
            connection.Open();

            string searchString = $@"SELECT * FROM {_tableName} WHERE ID = @id AND Available = @available;";
            var command = new SQLiteCommand(searchString, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@available", available);
            var count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }
       
    }
}
