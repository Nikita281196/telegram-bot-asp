using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services.Traiders
{
    public interface ITraiderService
    {
        IReadOnlyList<Traider> GetTraiders();
        Traider GetTraiderById(int id);
    }
}
