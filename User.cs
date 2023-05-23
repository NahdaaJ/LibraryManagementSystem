﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    internal class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public int GeneratePin()
        {
            var database = new DatabaseManager();
            int pin = 0;
            bool pinDistinct = false;
            
            while (!pinDistinct)
            {
                var random = new Random();
                string temp = "";
                for (int i = 0; i < 8; i++)
                {
                    temp += random.Next(0, 9).ToString();
                }
                pin = int.Parse(temp);
                if (database.CheckPinDistinct(pin) == 0)
                {
                    break;
                }
            }
            return pin;
        }

    }
}
