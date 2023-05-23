﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class Loan
    {
        public int Pin { get; set; }
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
