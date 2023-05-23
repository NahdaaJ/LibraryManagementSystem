using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class LoanManager : DatabaseManager
    {
        private readonly string _tableName = "Loans";
        internal void LoanBook(Loan loan)
        {
            var pin = loan.Pin;
            var title = loan.BookTitle;
            var author = loan.Author;
            var loanDate = loan.DateBorrowed;
            var loanDateString = loanDate.ToString("yyyy-MM-dd");
            var dueDate = loan.DueDate(loanDate);

            string loanString = $@"INSERT INTO {_tableName} (Pin, Title, Author, Date_Loaned, Date_Due) VALUES (@pin, @title, @author, @loanDateString, @dueDate);";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(loanString, connection))
                {
                    command.Parameters.AddWithValue("@pin", pin);
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@loanDateString", loanDateString);
                    command.Parameters.AddWithValue("@dueDate", dueDate);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            var editAvailability = new BookManager();
            editAvailability.EditAvailability(title, author, "No");
        }
        internal void ReturnBook(string id)
        {
            var dateReturned = DateTime.Now.ToString("yyyy-MM-dd");
            var title = "";
            var author = "";
           
            string returnString = @$"UPDATE {_tableName} SET Date_Returned = @dateReturned WHERE ID = @id;";
            string searchString = $"SELECT * FROM {_tableName} WHERE ID = @id;";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(returnString, connection))
                {
                    command.Parameters.AddWithValue("@dateReturned", dateReturned);
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(searchString, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    var rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        title = rdr.GetString(2);
                        author = rdr.GetString(3);
                    }
                }
                connection.Close();
            }
            var editAvailability = new BookManager();
            editAvailability.EditAvailability(title, author, "Yes");
        }
        internal void RenewBook(int pin, string title, string author)
        {
            var loanID = 0;
            var dueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-DD");

            using (var connection = GetConnection())
            {
                connection.Open();
                string insertString = @$"SELECT ID FROM {_tableName}
                                        WHERE Pin = @pin AND Title = @title AND Author = @author 
                                        ORDER BY Date_Loaned DESC
                                        LIMIT 1;";

                using (var command = new SQLiteCommand(insertString, connection))
                {
                    command.Parameters.AddWithValue("@pin", pin);
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@author", author);

                    SQLiteDataReader rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        loanID = rdr.GetInt32(0);
                    }
                }
                connection.Close();
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                string renewString = $@"UPDATE {_tableName} SET Due_Date = @dueDate WHERE ID = @id;";

                using (var command =  new SQLiteCommand(renewString, connection))
                {
                    command.Parameters.AddWithValue("@dueDate",dueDate);
                    command.Parameters.AddWithValue("@id", loanID);
                }
            }
        }
        internal (SQLiteDataReader, SQLiteConnection) SearchUserLoans(int pin)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT * FROM {_tableName} WHERE Pin = @pin AND Date_Returned IS NULL;";

            var command = new SQLiteCommand(searchString, connection);
            command.Parameters.AddWithValue("@pin", pin);

            SQLiteDataReader rdr = command.ExecuteReader();
            return (rdr,connection);
        }
        internal int GetLoan(string id) // new close
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT * FROM {_tableName} WHERE ID = @id;";

            var command = new SQLiteCommand(searchString, connection);
            command.Parameters.AddWithValue("@id", id);

            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }
        internal int LoanLimit(int pin)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT COUNT(*) FROM {_tableName} WHERE Pin = @pin AND Date_Returned IS NULL;";

            var command = new SQLiteCommand(searchString, connection);
            command.Parameters.AddWithValue("@pin", pin);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count;
        }
       
    }
}
