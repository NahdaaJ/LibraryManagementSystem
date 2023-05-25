namespace LibraryManagementSystem
{
    internal class Menu
    {
        public void StartScreen()
        {
            var check = new DatabaseManager();
            check.InitialiseDatabase();

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
            var userManager = new UserManager();

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
4 - View All Books
5 - Logout");
                var userInput = Console.ReadLine().Trim();

                var (read, connection) = userManager.GetID(pin);
                read.Read();
                var userID = read.GetInt32(0);
                
                read.Close();
                read.Dispose();
                connection.Close();
                connection.Dispose();                

                switch (userInput)
                {
                    case "1":
                        var validID = false;
                        string loanTitle = "";
                        string loanAuthor = "";

                        if (loanManager.LoanLimit(userID) >= 3)
                        {
                            Console.Clear();
                            Console.WriteLine("You currently have 3 outstanding loans. You cannot loan anymore books.\nPress enter to return to Main Menu.");
                            Console.ReadLine();
                            break;
                        }

                        while (!validID)
                        {
                            Console.Clear();                            
                            Console.WriteLine("ID        Book Title                                                  Author");
                            Console.WriteLine("----     ------------------------------------------------------      ---------------------");

                            var (reader, conn) = bookManager.SearchBook("Yes");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader.GetInt32(0),-10}{reader.GetString(1),-60}{reader.GetString(2)}");
                            }
                                                       
                            reader.Close();
                            reader.Dispose();
                            conn.Close();
                            conn.Dispose();

                            Console.WriteLine("------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please input the ID of one of the above books, or press (C) to cancel.");
                            userInput = Console.ReadLine().Trim().ToLower();

                            if (userInput == "c")
                            {
                                Console.Clear();
                                Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                Console.ReadLine();
                                UserMenu(firstName, lastName, pin);
                            }

                            if (bookManager.CheckBookExists(userInput, "Yes") == 0)
                            {
                                Console.WriteLine("Please enter a valid ID.");
                                Console.ReadLine();
                                continue;
                            }
                            break;
                        }

                        var (rdr1, connection1) = bookManager.GetBookInfo(userInput);
                        while (rdr1.Read())
                        {
                            loanTitle = rdr1.GetString(1);
                            loanAuthor = rdr1.GetString(2);
                        }              
                        
                        rdr1.Close();
                        rdr1.Dispose();
                        connection1.Close();
                        connection1.Dispose();

                        var loan = new Loan();
                        loan.UserID = userID;
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
                            Console.WriteLine("ID        Book Title                              Author                 Date Loaned          Due Date");
                            Console.WriteLine("----     ------------------------------------    --------------------    ------------        ------------");

                            var (rdr2,connection2) = loanManager.SearchUserLoans(userID);
                            while (rdr2.Read())
                            {
                                var titleString = rdr2.GetString(2);
                                var authorString = rdr2.GetString(3);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length > 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr2.GetInt32(0),-10}{titleString,-40}{authorString,-24}{rdr2.GetString(4).Substring(0, 10),-20}{rdr2.GetString(5).Substring(0, 10)}");
                            }

                            rdr2.Close();
                            rdr2.Dispose();
                            connection2.Close();
                            connection2.Dispose();

                            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please enter the ID of the book you would like to return, or type (C) to cancel.");
                            var userInput2 = Console.ReadLine().Trim();

                            if (userInput2 == "c" || userInput2 == "C")
                            {
                                Console.Clear();
                                Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                Console.ReadLine();
                                UserMenu(formatFirstName, formatLastName, pin);
                            }
                            if (loanManager.GetLoan(userInput2) == 0) 
                            {
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

                            Console.WriteLine("ID        Title                                             Author                   Genre                 Available?");
                            Console.WriteLine("-----    ----------------------------------------------    --------------------     --------------        ------------");

                            var (rdr3,connection3) = bookManager.SearchBook(userSearchTerm);
                            while (rdr3.Read())
                            {
                                var titleString = rdr3.GetString(1);
                                var authorString = rdr3.GetString(2);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length > 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr3.GetInt32(0),-10}{titleString,-50}{authorString,-25}{rdr3.GetString(4),-25}{rdr3.GetString(6)}"); // display id, title, author, genre, users can select to read in depth or exit 01246
                            }

                            rdr3.Close();
                            rdr3.Dispose();
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
                                Console.WriteLine(@$"Title: {rdr4.GetString(1),-40}Published: {rdr4.GetString(5).Substring(0, 10)}
Author: {rdr4.GetString(2),-40}Genre: {rdr4.GetString(4)}

Description:
{rdr4.GetString(3)}

Currently Available: {rdr4.GetString(6)}");
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();

                                rdr4.Close();
                                rdr4.Dispose();
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
                        validID = false;

                        while (!validID)
                        {
                            Console.Clear();
                            Console.WriteLine("ID        Title                                             Author                   Genre                 Available?");
                            Console.WriteLine("-----    ----------------------------------------------    --------------------     --------------        ------------");

                            var (rdr5, connection5) = bookManager.ViewAllBooks();
                            while (rdr5.Read())
                            {
                                var titleString = rdr5.GetString(1);
                                var authorString = rdr5.GetString(2);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length > 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr5.GetInt32(0),-10}{titleString,-50}{authorString,-25}{rdr5.GetString(4),-25}{rdr5.GetString(6)}"); // display id, title, author, genre, users can select to read in depth or exit 01246
                            }

                            rdr5.Close();
                            rdr5.Dispose();
                            connection5.Close();
                            connection5.Dispose();

                            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("If you would like to see details about a book, please enter the ID.\nIf you would like to return to the main menu, please enter (M).");
                            var userInput4 = Console.ReadLine().Trim().ToLower();

                            if (userInput4 == "m")
                            {
                                UserMenu(formatFirstName, formatLastName, pin);
                            }

                            var (rdr6, connection6) = bookManager.GetBookInfo(userInput4);
                            if (rdr6.HasRows)
                            {
                                rdr6.Read();
                                Console.Clear();
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine(@$"Title: {rdr6.GetString(1),-40}Published: {rdr6.GetString(5).Substring(0, 10)}
Author: {rdr6.GetString(2),-40}Genre: {rdr6.GetString(4)}

Description:
{rdr6.GetString(3)}

Currently Available: {rdr6.GetString(6)}");
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();

                                rdr6.Close();
                                rdr6.Dispose();
                                connection6.Close();
                                connection6.Dispose();
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

                    case "5":
                        Console.Clear();
                        Console.WriteLine("Goodbye! Press enter to return to the start screen.");
                        Console.ReadLine();
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
            var bookManager = new BookManager();
            var userManager = new UserManager();
            var loanManager = new LoanManager();

            var validInput = false;

            while (!validInput)
            {
                Console.Clear();
                Console.WriteLine(@"Welcome! Please choose a service:

Book Inventory Options                          User Options                    Loan Options
-------------------------------------          ---------------------           ----------------------
1 - Add a book to the inventory.                5 - Add a new user.             9 - Search all outstanding loans.
2 - Remove a book from the inventory.           6 - Delete a user.             10 - Renew user loan.
3 - Search a book.                              7 - Search a user.             11 - Logout.   
4 - View all books.                             8 - View all users.
");
                var userInput = Console.ReadLine().Trim();

                switch (userInput)
                {
                    case "1":
                        var inputCorrect = false;

                        while (!inputCorrect)
                        {
                            Console.Clear();
                            Console.Write("Please enter the title of the book: ");
                            var title = Console.ReadLine().Trim().Replace("'","\'").Replace('"','\"').Replace("@","@@");

                            Console.Clear();
                            Console.Write("Please enter the author of the book: ");
                            var author = Console.ReadLine().Trim().Replace("'", "\'").Replace('"', '\"').Replace("@", "@@");

                            Console.Clear();
                            Console.WriteLine("Please enter a description of the book: ");
                            var description = Console.ReadLine().Trim().Replace("'", "\'").Replace('"', '\"').Replace("@", "@@");

                            Console.Clear();
                            Console.WriteLine(@"Fiction              Mystery             Thriller            Romance
Science Fiction      Fantasy             Historical Fiction  Biography
Non-Fiction          Self-Help           Horror              Adventure
Young Adult          Poetry              Drama               Crime
Humor                Political           Philosophy          Reference
");


                            Console.WriteLine("----------------------------------------------------------------------");
                            Console.Write("Please enter the genre of the book, use one of the keywords listed above: ");
                            var genre = Console.ReadLine().Trim().Replace("'", "\'").Replace('"', '\"').Replace("@", "@@");

                            Console.Clear();
                            Console.Write("Please enter the publication date of the book (YYYY-MM-DD format) : ");
                            var published = Console.ReadLine().Trim().Replace("'", "\'").Replace('"', '\"').Replace("@", "@@");

                            var newBook = new Book();
                            newBook.Title = title;
                            newBook.Author = author;
                            newBook.Description = description;
                            newBook.Genre = genre;
                            newBook.PublicationDate = published;

                            Console.Clear();
                            Console.WriteLine(@$"Title: {title,-40}Published: {published}
Author: {author,-39}Genre: {genre}

Description:
{description}");
                            Console.WriteLine("-------------------------------------------------------------------------------------");
                            Console.WriteLine("Is this correct? (Y/N) Or would you like to cancel? (C)");
                            var correctBook = Console.ReadLine().Trim().ToLower();

                            if (correctBook == "y")
                            {
                                Console.Clear();
                                bookManager.AddBook(newBook);
                                Console.WriteLine("New book added! Please press enter to return to the Main Menu.");
                                Console.ReadLine();
                                break;
                            }
                            else if (correctBook == "n")
                            {
                                continue;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                Console.ReadLine();
                                break;
                            }
                        }                 
                        break;

                    case "2":
                        var validID = false;

                        while (!validID)
                        {
                            Console.Clear();
                            Console.Write("Please enter your search term: ");
                            var removeBookSearch = Console.ReadLine();

                            Console.Clear();
                            Console.WriteLine($"Results for '{removeBookSearch}' :\n");

                            Console.WriteLine("ID        Title                                             Author                   Genre                 Available?");
                            Console.WriteLine("-----    ----------------------------------------------    --------------------     --------------        ------------");

                            var (rdr, connection) = bookManager.SearchBook(removeBookSearch);
                            while (rdr.Read())
                            {
                                var titleString = rdr.GetString(1);
                                var authorString = rdr.GetString(2);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length > 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr.GetInt32(0),-10}{titleString,-50}{authorString,-25}{rdr.GetString(4),-25}{rdr.GetString(6)}");
                            }

                            rdr.Close();
                            rdr.Dispose();
                            connection.Close();
                            connection.Dispose();

                            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please enter the ID of the book you would like to remove.\nIf you would like to return to the main menu, please enter (M).");
                            var bookID = Console.ReadLine().Trim().ToLower();

                            if (bookID == "m")
                            {
                                StaffMenu();
                            }

                            var (rdr2, connection2) = bookManager.GetBookInfo(bookID);
                            if (rdr2.HasRows)
                            {
                                rdr2.Read();
                                Console.Clear();
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine(@$"Title: {rdr2.GetString(1),-40}Published: {rdr2.GetString(5)}
Author: {rdr2.GetString(2),-40}Genre: {rdr2.GetString(4)}

Description:
{rdr2.GetString(3)}

Currently Available: {rdr2.GetString(6)}");
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Is this the book you would like to remove (Y/N) or would you like to cancel (C)?");
                                var correctBook = Console.ReadLine().Trim().ToLower();

                                rdr2.Close();
                                rdr2.Dispose();
                                connection2.Close();
                                connection2.Dispose();

                                if (correctBook == "y")
                                {
                                    bookManager.RemoveBook(bookID);
                                    Console.Clear();
                                    Console.WriteLine("Book removed from inventory! Press enter to return to the Main Menu.");
                                    Console.ReadLine();
                                }
                                else if (correctBook == "n")
                                {
                                    continue;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                    Console.ReadLine();
                                    StaffMenu();
                                }          
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
                    case "3":
                        validID = false;

                        Console.Clear();
                        Console.Write("Please enter your search term: ");
                        var userSearchTerm = Console.ReadLine();

                        while (!validID)
                        {
                            Console.Clear();
                            Console.WriteLine($"Results for '{userSearchTerm}' :\n");

                            Console.WriteLine("ID        Title                                             Author                   Genre                 Available?");
                            Console.WriteLine("-----    ----------------------------------------------    --------------------     --------------        ------------");

                            var (rdr3, connection3) = bookManager.SearchBook(userSearchTerm);
                            while (rdr3.Read())
                            {
                                var titleString = rdr3.GetString(1);
                                var authorString = rdr3.GetString(2);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length > 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr3.GetInt32(0),-10}{titleString,-50}{authorString,-25}{rdr3.GetString(4),-25}{rdr3.GetString(6)}");
                            }

                            rdr3.Close();
                            rdr3.Dispose();
                            connection3.Close();
                            connection3.Dispose();

                            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("If you would like to see details about a book, please enter the ID.\nIf you would like to return to the main menu, please enter (M).");
                            var userInput3 = Console.ReadLine().Trim().ToLower();

                            if (userInput3 == "m")
                            {
                                StaffMenu();
                            }

                            var (rdr4, connection4) = bookManager.GetBookInfo(userInput3);
                            if (rdr4.HasRows)
                            {
                                rdr4.Read();
                                Console.Clear();
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine(@$"Title: {rdr4.GetString(1),-40}Published: {rdr4.GetString(5).Substring(0, 10)}
Author: {rdr4.GetString(2),-40}Genre: {rdr4.GetString(4)}

Description:
{rdr4.GetString(3)}

Currently Available: {rdr4.GetString(6)}");

                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();

                                rdr4.Close();
                                rdr4.Dispose();
                                connection4.Close();
                                connection4.Dispose();

                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                        }
                        break;
                    case "4":
                        validID = false;

                        while (!validID)
                        {
                            Console.Clear();
                            Console.WriteLine("ID        Title                                             Author                   Genre                 Available?");
                            Console.WriteLine("-----    ----------------------------------------------    --------------------     --------------        ------------");

                            var (rdr5, connection5) = bookManager.ViewAllBooks();
                            while (rdr5.Read())
                            {
                                var titleString = rdr5.GetString(1);
                                var authorString = rdr5.GetString(2);

                                if (titleString.Length > 30)
                                {
                                    titleString = titleString.Substring(0, 30) + "...";
                                }
                                if (authorString.Length >= 15)
                                {
                                    authorString = authorString.Substring(0, 15) + "...";
                                }
                                Console.WriteLine($"{rdr5.GetInt32(0),-10}{titleString,-50}{authorString,-25}{rdr5.GetString(4),-25}{rdr5.GetString(6)}");
                            }

                            rdr5.Close();
                            rdr5.Dispose();
                            connection5.Close();
                            connection5.Dispose();

                            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("If you would like to see details about a book, please enter the ID.\nIf you would like to return to the main menu, please enter (M).");
                            var userInput4 = Console.ReadLine().Trim().ToLower();

                            if (userInput4 == "m")
                            {
                                StaffMenu();
                            }

                            var (rdr6, connection6) = bookManager.GetBookInfo(userInput4);
                            if (rdr6.HasRows)
                            {
                                rdr6.Read();

                                Console.Clear();
                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine(@$"Title: {rdr6.GetString(1),-40}Published: {rdr6.GetString(5).Substring(0, 10)}
Author: {rdr6.GetString(2),-40}Genre: {rdr6.GetString(4)}

Description:
{rdr6.GetString(3)}

Currently Available: {rdr6.GetString(6)}");

                                Console.WriteLine("------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();

                                rdr6.Close();
                                rdr6.Dispose();
                                connection6.Close();
                                connection6.Dispose();
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                            break;
                        };
                        break;
                    case "5":
                        var userDetailsCorrect = false;

                        while(!userDetailsCorrect)
                        {
                            Console.Clear();
                            Console.Write("Please enter the users first name: ");
                            var firstName = Console.ReadLine().Trim();

                            Console.Write("Please enter the users last name: ");
                            var lastName = Console.ReadLine().Trim();

                            Console.Write("Please enter the users email address: ");
                            var email = Console.ReadLine().Trim();

                            Console.Clear();
                            Console.WriteLine($"Is this correct? (Y/N) Or would you like to cancel? (C)\n\nFirst Name: {firstName,-14}\tLast Name: {lastName,-15}\tEmail: {email}");
                            var detailsCorrect = Console.ReadLine().Trim().ToLower();

                            if (detailsCorrect == "y")
                            {
                                var newUser = new User();
                                newUser.FirstName = firstName;
                                newUser.LastName = lastName;
                                newUser.Email = email;

                                userManager.AddUser(newUser);
                                var (rdr5, connection5) = userManager.GetPin(firstName, lastName, email);
                                rdr5.Read();
                                var pin = rdr5.GetInt32(0);

                                rdr5.Close();
                                rdr5.Dispose();
                                connection5.Close();
                                connection5.Dispose();

                                Console.Clear();
                                Console.WriteLine($@"-------------------------- New User Details --------------------------
First Name: {firstName,-35}Last Name: {lastName}
Email: {email,-40}Unique Pin: {pin}
");
                                Console.WriteLine("----------------------------------------------------------------------");
                                Console.WriteLine("Please press enter to return to the Main Menu.");
                                Console.ReadLine();
                                StaffMenu();
                            }
                            else if (detailsCorrect == "n")
                            {
                                continue;
                            }
                            else if (detailsCorrect == "c")
                            {
                                Console.Clear();
                                Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                Console.ReadLine();
                                StaffMenu();
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }                              
                        }
                        break;
                    case "6":
                        validID = false;

                        while (!validID)
                        {
                            Console.Clear();
                            Console.Write("Please enter your user search term: ");
                            var removeUserSearch = Console.ReadLine();

                            Console.Clear();
                            Console.WriteLine($"Results for '{removeUserSearch}' :\n");

                            Console.WriteLine("ID        First Name          Last Name           Email                                   Pin");
                            Console.WriteLine("-----    ---------------     ---------------     -----------------------------------     ----------");

                            var (rdr6, connection6) = userManager.SearchUser(removeUserSearch);
                            while (rdr6.Read())
                            {
                                Console.WriteLine($"{rdr6.GetInt32(0),-10}{rdr6.GetString(1),-20}{rdr6.GetString(2),-20}{rdr6.GetString(3),-40} {rdr6.GetInt32(4)}");
                            }

                            rdr6.Close();
                            rdr6.Dispose();
                            connection6.Close();
                            connection6.Dispose();

                            Console.WriteLine("---------------------------------------------------------------------------------------------------");
                            Console.WriteLine("Please enter the ID of the user you would like to remove.\nIf you would like to return to the Main Menu, please enter (M).");
                            var userRemoveID = Console.ReadLine().Trim().ToLower();

                            if (userRemoveID == "m")
                            {
                                StaffMenu();
                            }

                            (rdr6, connection6) = userManager.GetUserInfo(userRemoveID);
                            if (rdr6.HasRows)
                            {
                                rdr6.Read();
                                Console.Clear();
                                Console.WriteLine("---------------------------------------------------------------------------------------------------");
                                Console.WriteLine($"{rdr6.GetInt32(0),-10}{rdr6.GetString(1),-20}{rdr6.GetString(2),-20}{rdr6.GetString(3),-40} {rdr6.GetInt32(4)}");
                                Console.WriteLine("---------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Is this the user you would like to remove (Y/N) or would you like to cancel (C)?");
                                var correctBook = Console.ReadLine().Trim().ToLower();

                                rdr6.Close();
                                rdr6.Dispose();
                                connection6.Close();
                                connection6.Dispose();

                                if (correctBook == "y")
                                {
                                    userManager.RemoveUser(userRemoveID);
                                    Console.Clear();
                                    Console.WriteLine("User removed! Press enter to return to the Main Menu.");
                                    Console.ReadLine();
                                    break;
                                }
                                else if (correctBook == "n")
                                {
                                    continue;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Action cancelled. Please press enter to return to Main Menu");
                                    Console.ReadLine();
                                    StaffMenu();
                                }                                
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                            break;
                        };
                        break;
                    case "7":
                        validID = false;

                        Console.Clear();
                        Console.Write("Please enter your user search term: ");
                        var userSearch = Console.ReadLine();

                        Console.Clear();
                        Console.WriteLine($"Results for '{userSearch}' :\n");

                        Console.WriteLine("ID        First Name          Last Name           Email                                   Pin");
                        Console.WriteLine("-----    ---------------     ---------------     -----------------------------------     ----------");

                        var (rdr7, connection7) = userManager.SearchUser(userSearch);
                        while (rdr7.Read())
                        {
                            Console.WriteLine($"{rdr7.GetInt32(0),-10}{rdr7.GetString(1),-20}{rdr7.GetString(2),-20}{rdr7.GetString(3),-40}{rdr7.GetInt32(4)}");
                        }

                        rdr7.Close();
                        rdr7.Dispose();
                        connection7.Close();
                        connection7.Dispose();

                        Console.WriteLine("---------------------------------------------------------------------------------------------------");
                        Console.WriteLine("Please press enter to return to the Main Menu.");
                        Console.ReadLine();
                        break;

                    case "8":                        
                        Console.Clear();                        
                        Console.WriteLine("ID        First Name          Last Name           Email                                   Pin");
                        Console.WriteLine("-----    ---------------     ---------------     -----------------------------------     ----------");

                        var (rdr8, connection8) = userManager.ViewAllUsers();
                        while (rdr8.Read())
                        {
                            Console.WriteLine($"{rdr8.GetInt32(0),-10}{rdr8.GetString(1),-20}{rdr8.GetString(2),-20}{rdr8.GetString(3),-40}{rdr8.GetInt32(4)}");
                        }

                        rdr8.Close();
                        rdr8.Dispose();
                        connection8.Close();
                        connection8.Dispose();

                        Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("Please press enter to return to the Main Menu.");
                        Console.ReadLine();
                        break;

                    case "9":
                        Console.Clear();                        
                        Console.WriteLine("ID        User ID        Book Title                              Author                   Date Loaned    Due Date");
                        Console.WriteLine("-----    ---------      -----------------------------------     ---------------------    ------------   ------------");
                        
                        var (rdr9, connection9) = loanManager.ViewAllLoans();
                        while (rdr9.Read())
                        {
                            var titleString = rdr9.GetString(2);
                            var authorString = rdr9.GetString(3);

                            if (titleString.Length > 30)
                            {
                                titleString = titleString.Substring(0, 30)+"...";
                            }
                            if (authorString.Length > 15)
                            {
                                authorString = authorString.Substring(0, 15) + "...";
                            }
                            Console.WriteLine($"{rdr9.GetInt32(0),-10}{rdr9.GetInt32(1),-15}{titleString,-40}{authorString,-25}{rdr9.GetString(4).Substring(0,10),-15}{rdr9.GetString(5).Substring(0, 10)}");
                        }

                        rdr9.Close();
                        rdr9.Dispose();
                        connection9.Close();
                        connection9.Dispose();

                        Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("Please press enter to return to the Main Menu.");
                        Console.ReadLine();                        
                        break;

                    case "10":
                        var pinIsNum = false;
                        validID = false;
                        int userID = 0;

                        while (!pinIsNum)
                        {
                            Console.Clear();
                            Console.Write("Please enter the users pin, or enter (C) to cancel: ");
                            var userPinString = Console.ReadLine().Trim().ToLower();

                            if (userPinString == "c")
                            {
                                Console.Clear();
                                Console.WriteLine("Action cancelled. Please press enter to return to the Main Menu.");
                                Console.ReadLine();
                                StaffMenu();
                            }

                            if (!int.TryParse(userPinString, out _))
                            {
                                Console.WriteLine("Pin must be an 8-digit number. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }

                            var userPin = int.Parse(userPinString);
                            var (rdr11, connection11) = userManager.GetID(userPin);

                            if (rdr11.HasRows)
                            {
                                rdr11.Read();
                                userID = rdr11.GetInt32(0);


                                rdr11.Close();
                                rdr11.Dispose();
                                connection11.Close();
                                connection11.Dispose();
                                break;
                            }
                            else
                            {
                                rdr11.Close();
                                rdr11.Dispose();
                                connection11.Close();
                                connection11.Dispose();
                                Console.WriteLine("Incorrect pin. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                        }

                        while (!validID) 
                        { 
                            var (rdr10, connection10) = loanManager.SearchUserLoans(userID);

                            if (rdr10.HasRows)
                            {
                                while (rdr10.Read())
                                {
                                    Console.Clear();
                                    Console.WriteLine("ID        Book Title                              Author                   Date Loaned         Due Date");
                                    Console.WriteLine("-----    -----------------------------------     ---------------------    ------------        -----------");

                                    var titleString = rdr10.GetString(2);
                                    var authorString = rdr10.GetString(3);

                                    if (titleString.Length > 30)
                                    {
                                        titleString = titleString.Substring(0, 30) + "...";
                                    }
                                    if (authorString.Length > 15)
                                    {
                                        authorString = authorString.Substring(0, 15) + "...";
                                    }
                                    Console.WriteLine($"{rdr10.GetInt32(0),-10}{titleString,-40}{authorString,-25}{rdr10.GetString(4).Substring(0,10),-20}{rdr10.GetString(5).Substring(0, 10)}");
                                }

                                rdr10.Close();
                                rdr10.Dispose();
                                connection10.Close();
                                connection10.Dispose();

                                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                                Console.WriteLine("Please enter the ID of which loan to renew, or enter (C) to cancel.");
                                var loanID = Console.ReadLine().Trim().ToLower();

                                if (loanID == "c")
                                {
                                    Console.Clear();
                                    Console.WriteLine("Action cancelled. Please press enter to return to the Main Menu.");
                                    Console.ReadLine();
                                    StaffMenu();
                                }

                                if (loanManager.CheckLoanExists(loanID) == 0)
                                {
                                    Console.WriteLine("Please enter a valid ID.");
                                    Console.ReadLine();
                                    continue;
                                }

                                loanManager.RenewBook(loanID);

                                Console.WriteLine("Loan renewed! Please press enter to return to the Main Menu.");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {

                                rdr10.Close();
                                rdr10.Dispose();
                                connection10.Close();
                                connection10.Dispose();

                                Console.WriteLine("Invalid pin. Please press enter and try again.");
                                Console.ReadLine();
                                continue;
                            }
                        }
                        break;

                    case "11":
                        Console.Clear();
                        Console.WriteLine("Goodbye! Press enter to return to the start screen.");
                        Console.ReadLine();
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
