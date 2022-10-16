using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface ICleanUpService
    {
        public Task<bool> StartCleanUpAsync();
    }
}
