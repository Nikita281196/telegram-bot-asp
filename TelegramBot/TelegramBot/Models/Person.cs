using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string FullName 
        { 
            get
            {
                return $"{LastName} {FirstName} {MiddleName}";
            } 
        }

        public string ShortName 
        { 
            get 
            { 
                return $"{LastName} {FirstName}"; 
            } 
        } 
    }
}
