namespace LibraryManagementSystem
{
    internal class Loan
    {
        public int UserID { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public DateTime DateBorrowed { get; set; }

        public string DueDate(DateTime dateBorrowed)
        {
            var dueDate = dateBorrowed.AddDays(14).ToString("yyyy-MM-dd");
            return dueDate;
        }
    }
}
