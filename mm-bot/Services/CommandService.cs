using mm_bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class CommandService : ICommandService
    {
        public async Task ProcessCommand(string command)
        {
            if (command.Equals("-cleanup"))
            {

            }
        }
    }
}
