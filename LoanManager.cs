using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    internal class LoanManager : DatabaseManager
    {
        private readonly string _tableName = "Loans";
        internal void LoanBook(Loan loan)
        {
            var userID = loan.UserID;
            var title = loan.BookTitle;
            var author = loan.Author;
            var loanDate = loan.DateBorrowed;
            var loanDateString = loanDate.ToString("yyyy-MM-dd");
            var dueDate = loan.DueDate(loanDate);

            string loanString = $@"INSERT INTO {_tableName} (User_ID, Title, Author, Date_Loaned, Date_Due) VALUES (@userID, @title, @author, @loanDateString, @dueDate);";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(loanString, connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);
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
                using (var command = new MySqlCommand(returnString, connection))
                {
                    command.Parameters.AddWithValue("@dateReturned", dateReturned);
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
                using (var command = new MySqlCommand(searchString, connection))
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
        internal void RenewBook(string loanID)
        {
            var dueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
           
            using (var connection = GetConnection())
            {
                connection.Open();

                string renewString = $@"UPDATE {_tableName} SET Date_Due = @dueDate WHERE ID = @loanID;";

                using (var command =  new MySqlCommand(renewString, connection))
                {
                    command.Parameters.AddWithValue("@dueDate",dueDate);
                    command.Parameters.AddWithValue("@loanID", loanID);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        internal (MySqlDataReader, MySqlConnection) SearchUserLoans(int userID)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT * FROM {_tableName} WHERE User_ID = @userID AND Date_Returned IS NULL;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@UserID", userID);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr,connection);
        }
        internal (MySqlDataReader, MySqlConnection) ViewAllLoans()
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT * FROM {_tableName} WHERE Date_Returned IS NULL;";

            var command = new MySqlCommand(searchString, connection);

            MySqlDataReader rdr = command.ExecuteReader();
            return (rdr, connection);
        }
        internal int GetLoan(string id)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT * FROM {_tableName} WHERE ID = @id;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@id", id);

            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            connection.Dispose();
            return count;
        }
        internal int LoanLimit(int userID)
        {
            var connection = GetConnection();
            connection.Open();
            string searchString = @$"SELECT COUNT(*) FROM {_tableName} WHERE User_ID = @userID AND Date_Returned IS NULL;";

            var command = new MySqlCommand(searchString, connection);
            command.Parameters.AddWithValue("@userID", userID);

            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            connection.Dispose();
            return count;
        }
        internal int CheckLoanExists(string loanID)
        {
            var connection = GetConnection();
            connection.Open();

            string searchString = $@"SELECT COUNT(*) FROM {_tableName} WHERE ID = @id;";
            var command = new MySqlCommand(searchString, connection);

            command.Parameters.AddWithValue("@id", loanID);
            var count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            connection.Dispose();
            return count;
        }
       
    }
}
