using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class Login : DatabaseManager
    {
        private readonly string LibrarianUsername = "User";
        private readonly string LibrarianPassword = "Pass" ;
        internal bool UserLogin(string firstName, string lastName, int pin)
        {
            var count = ValidateUser(firstName, lastName, pin);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        internal bool LibrarianLogin(string username, string password)
        {
            if (username == LibrarianUsername && password == LibrarianPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
