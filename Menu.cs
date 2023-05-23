using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class Menu
    {
        public void StartScreen()
        {
            var login = new Login();
            var inputCorrect = false;

            var userInputStartScreen = "";

            while (!inputCorrect)
            {
                Console.Clear();
                Console.WriteLine(@"Welcome to Enfield Library! Would you like to: 
1 - Login as a User
2 - Login as Staff
3 - Exit");
                userInputStartScreen = Console.ReadLine().Trim();
                switch (userInputStartScreen)
                {
                    case "1":
                        var pinIsNum = false;

                        var firstName = "";
                        var lastName = "";
                        var pin = 0;

                        while (!pinIsNum)
                        {
                            Console.Clear();
                            Console.Write("Please enter your first name:\t");
                            firstName = Console.ReadLine().Trim();

                            Console.Write("\nPlease enter your first name:\t");
                            lastName = Console.ReadLine().Trim();

                            Console.Write("\nPlease enter your 8-digit pin:\t");
                            var pinString = "";

                            ConsoleKeyInfo keyInfo;

                            do
                            {
                                keyInfo = Console.ReadKey(true);

                                if (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    pinString += keyInfo.KeyChar;

                                    Console.Write("*");
                                }

                            } while (keyInfo.Key != ConsoleKey.Enter);

                            if (!int.TryParse(pinString, out _))
                            {
                                Console.Write("\nYour pin will consist of 8 numbers and no spaces. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                            else
                            {
                                pin = int.Parse(pinString);
                            }
                            var validateUserLogin = login.UserLogin(firstName, lastName, pin);

                            if (!validateUserLogin)
                            {
                                Console.WriteLine("\nLogin incorrect. Please press enter and try again, or press (1) to exit.");
                                var userInput = Console.ReadLine();
                                if (userInput == "1")
                                {
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                UserMenu(firstName, lastName, pin);
                                break;
                            }
                        }
                        break;

                    case "2":
                        var staffLoginCorrect = false;

                        while (!staffLoginCorrect)
                        {
                            Console.Clear();
                            Console.Write("Username: ");
                            var username = Console.ReadLine();

                            Console.Write("Password: ");

                            var password = "";

                            ConsoleKeyInfo keyInfo;

                            do
                            {
                                keyInfo = Console.ReadKey(true);

                                if (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    password += keyInfo.KeyChar;

                                    Console.Write("*");
                                }

                            } while (keyInfo.Key != ConsoleKey.Enter);

                            var validateStaffLogin = login.LibrarianLogin(username, password);

                            if (!validateStaffLogin)
                            {
                                Console.WriteLine("\nUsername or password incorrect. Press enter and try again or press (1) to return to the start screen.");
                                var userInput = Console.ReadLine();
                                if (userInput == "1")
                                {
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                StaffMenu();
                                break;
                            }
                        }
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("Thank you for visiting Enfield Library!\nExiting...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Incorrect input. Please press enter and try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }
        public void UserMenu(string firstName, string lastName, int pin)
        {
            var bookManager = new BookManager();
            var loanManager = new LoanManager();

            var temp1 = firstName[0].ToString();
            var formatFirstName = temp1.ToUpper() + firstName.Substring(1);
            var temp2 = lastName[0].ToString();
            var formatLastName = temp2.ToUpper() + lastName.Substring(1);

            var validInput = false;

            while (!validInput)
            {
                Console.Clear();
                Console.WriteLine($@"Welcome {formatFirstName}! What would you like to do today?

1 - Loan a Book
2 - Return a Book
3 - Search Books
4 - Logout");
                var userInput = Console.ReadLine().Trim();

                switch (userInput)
                {
                    case "1":
                        var validID = false;
                        SQLiteDataReader rdr;
                        string loanTitle = "";
                        string loanAuthor = "";

                        if (loanManager.LoanLimit(pin) >= 3)
                        {
                            Console.Clear();
                            Console.WriteLine("You currently have 3 outstanding loans. You cannot loan anymore books.\nPress enter to return to Main Menu.");
                            Console.ReadLine();
                            break;
                        }

                        while (!validID)
                        {
                            Console.Clear();
                            var (reader, conn) = bookManager.SearchBook("Yes");
                            Console.WriteLine("ID        Book Title                                                  Author");
                            Console.WriteLine("----     ------------------------------------------------------      ---------------------");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader.GetInt32(0),-10}{reader.GetString(1),-60}{reader.GetString(2)}");
                            }
                            Console.WriteLine("------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please input the ID of one of the above books, or press (C) to cancel.");
                            userInput = Console.ReadLine().Trim().ToLower();

                            if (userInput == "c")
                            {
                                UserMenu(firstName, lastName, pin);
                            }

                            if (bookManager.CheckBookExists(userInput, "Yes") == 0)
                            {
                                Console.WriteLine("Please enter a valid ID.");
                                Console.ReadLine();
                                continue;
                            }
                            conn.Close();
                            break;
                        }
                        Console.Clear();
                        var (rdr1, connection1) = bookManager.GetBookInfo(userInput);
                        while (rdr1.Read())
                        {
                            loanTitle = rdr1.GetString(1);
                            loanAuthor = rdr1.GetString(2);
                        }
                        connection1.Close();
                        connection1.Dispose();

                        var loan = new Loan();
                        loan.Pin = pin;
                        loan.BookTitle = loanTitle.Replace("'", "\'");
                        loan.Author = loanAuthor.Replace("'", "\'");
                        loan.DateBorrowed = DateTime.Now;

                        loanManager.LoanBook(loan);
                        Console.Clear();
                        Console.WriteLine("Enjoy your book! Please press enter to return to Main Menu.");
                        Console.ReadLine();
                        break;

                    case "2":
                        var invalidInput = false;

                        while (!invalidInput)
                        {
                            Console.Clear();
                            Console.WriteLine("ID        Book Title                              Author             Date Loaned          Due Date");
                            Console.WriteLine("----     ------------------------------------    ----------------    ------------        ------------");

                            var (rdr2,connection2) = loanManager.SearchUserLoans(pin);
                            while (rdr2.Read())
                            {
                                Console.WriteLine($"{rdr2.GetInt32(0),-10}{rdr2.GetString(2),-40}{rdr2.GetString(3),-20}{rdr2.GetString(4),-20}{rdr2.GetString(5)}");
                            }
                            connection2.Close();
                            connection2.Dispose();
                            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please enter the ID of the book you would like to return, or type (C) to cancel.");
                            var userInput2 = Console.ReadLine().Trim();

                            if (userInput2 == "c" || userInput2 == "C")
                            {
                                UserMenu(formatFirstName, formatLastName, pin);
                            }
                            if (loanManager.GetLoan(userInput2) == 0) {
                                Console.WriteLine("Please enter a valid ID.");
                                Console.ReadLine();
                                continue;
                            }
                            loanManager.ReturnBook(userInput2);
                            Console.Clear();
                            Console.WriteLine("We hope you enjoyed your book! Please press enter to return to Main Menu.");
                            Console.ReadLine();
                            break;
                        }
                        break;

                    case "3":
                        validID = false;
                        Console.Clear();
                        Console.Write("Please enter your search term: ");
                        var userSearchTerm = Console.ReadLine();

                        while (!validID)
                        {
                            Console.Clear();
                            Console.WriteLine($"Results for '{userSearchTerm}' :\n");
                            var (rdr3,connection3) = bookManager.SearchBook(userSearchTerm);
                            Console.WriteLine("ID        Title                                             Author              Genre            Available?");
                            Console.WriteLine("-----    ----------------------------------------------    ---------------     ---------        ------------");
                            while (rdr3.Read())
                            {
                                Console.WriteLine($"{rdr3.GetInt32(0),-10}{rdr3.GetString(1),-50}{rdr3.GetString(2),-20}{rdr3.GetString(4),-20}{rdr3.GetString(6)}"); // display id, title, author, genre, users can select to read in depth or exit 01246
                            }
                            connection3.Close();
                            connection3.Dispose();

                            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("If you would like to see details about a book, please enter the ID.\nIf you would like to return to the main menu, please enter (M).");
                            var userInput3 = Console.ReadLine().Trim().ToLower();
                            if (userInput3 == "m")
                            {
                                UserMenu(formatFirstName, formatLastName, pin);
                            }

                            var (rdr4,connection4) = bookManager.GetBookInfo(userInput3);
                            if(rdr4.HasRows)
                            {
                                rdr4.Read();
                                Console.Clear();
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine(@$"Title: {rdr4.GetString(1),-40}Published: {rdr4.GetString(5)}
Author: {rdr4.GetString(2),-40}Genre: {rdr4.GetString(4)}

Description:
{rdr4.GetString(3)}

Currently Available: {rdr4.GetString(6)}");
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();
                                connection4.Close();
                                connection4.Dispose();
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }


                            break;            
                        }   
                        break;

                    case "4":
                        StartScreen();
                        break;

                    default:
                        Console.WriteLine("Incorrect input, please press enter and try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }
        public void StaffMenu()
        {
            var validInput = false;
            // add books, delete books, search books, add users, delete users, search users, search loans
            while (!validInput)
            {
                Console.Clear();
                Console.WriteLine(@"Welcome! Please choose a service:

Book Inventory Options                          User Options                    Loan Options
-------------------------------------          ---------------------           ----------------------
1 - Add a book to the inventory.                4 - Add a new user.             7 - Search all loans.
2 - Remove a book from the inventory.           5 - Delete a user.              8 - Renew user loan.
3 - Search a book.                              6 - Search a user.              9 - Logout.                        
");
                var userInput = Console.ReadLine().Trim();

                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("Book added to inventory!");
                        break;
                    case "2":
                        Console.WriteLine("Book removed from inventory!");
                        break;
                    case "3":
                        Console.WriteLine("Books are great!");
                        break;
                    case "4":
                        Console.WriteLine("So many loans!");
                        break;
                    case "5":
                        Console.WriteLine("User added!");
                        break;
                    case "6":
                        Console.WriteLine("User deleted!");
                        break;
                    case "7":
                        Console.WriteLine("So many readers!!");
                        break;
                    case "8":
                        StartScreen();
                        break;
                    default:
                        Console.WriteLine("Incorrect input, please press enter and try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
