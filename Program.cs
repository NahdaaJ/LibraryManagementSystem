using LibraryManagementSystem;
using System.Data.SQLite;

var check = new DatabaseManager();
check.InitialiseDatabase();

var startScreen = new Menu();
startScreen.StartScreen();



