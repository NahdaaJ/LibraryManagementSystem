using MySql.Data.MySqlClient;

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

            string addString = @$"INSERT IGNORE INTO {_tableName} (Title, Author, Description, Genre, Publication_Date, Available) 
                                    VALUES (@title,@author,@description, @genre, @publicationDate, 'Yes');";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(addString, connection))
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
        internal void RemoveBook(string id)
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
        internal void EditAvailability(string title, string author, string availability)
        {
            string editString = @$"UPDATE {_tableName} SET Available = @availability WHERE Title = @title AND Author = @author;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(editString, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@availability", availability);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal (MySqlDataReader, MySqlConnection) SearchBook(string searchTerm)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = $"SELECT * FROM {_tableName} WHERE Title LIKE @searchTerm OR Author LIKE @searchTerm OR Genre LIKE @searchTerm OR Available LIKE @searchTerm ORDER BY Title ASC;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@searchTerm", searchTerm);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
            
        }
        internal (MySqlDataReader, MySqlConnection) ViewAllBooks()
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = $"SELECT * FROM {_tableName} ORDER BY Title ASC;";

            var command = new MySqlCommand(searchString, connection);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
        }
        internal (MySqlDataReader, MySqlConnection) GetBookInfo(string id)
        {
            var connection = GetConnection();            
            connection.Open();

            string searchString = $@"SELECT * FROM {_tableName} WHERE ID = @id;";
            var command = new MySqlCommand(searchString, connection);
                
            command.Parameters.AddWithValue("@id", id);
            var rdr = command.ExecuteReader();
            return (rdr, connection);
            
        }
        internal int CheckBookExists(string id, string available) // new close
        {
            var connection = GetConnection();
            connection.Open();

            string searchString = $@"SELECT COUNT(*) FROM {_tableName} WHERE ID = @id AND Available = @available;";
            var command = new MySqlCommand(searchString, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@available", available);
            var count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            connection.Dispose();
            return count;
        }
       
    }
}
