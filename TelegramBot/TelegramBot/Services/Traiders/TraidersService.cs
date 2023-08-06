using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services.Traiders
{
    public class TraidersService: ITraiderService
    {
        private static readonly List<Traider> Traiders = new() { 
                new Traider() { Id = 1, FirstName = "Иван", LastName = "Иванов", MiddleName = "Иванович", Rating = "10%" },
                new Traider() { Id = 2, FirstName = "Петров", LastName = "Владимир", MiddleName = "Иванович", Rating = "24%" },
                new Traider() { Id = 3, FirstName = "Сидоров", LastName = "Кирилл", MiddleName = "Иванович", Rating = "13%" },
                new Traider() { Id = 4, FirstName = "Качан", LastName = "Александр", MiddleName = "Иванович", Rating = "87%" },
                new Traider() { Id = 5, FirstName = "Саетов", LastName = "Даниил", MiddleName = "Иванович", Rating = "11%" }
        };
        public IReadOnlyList<Traider> GetTraiders()
        {
            return Traiders;
        }

        public Traider GetTraiderById(int id)
        {
            return Traiders.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
