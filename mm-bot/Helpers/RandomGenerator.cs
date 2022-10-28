using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Helpers
{
    public class RandomGenerator
    {
        private Random rand;

        public RandomGenerator()
        {
            rand = new Random();
        }

        public decimal NextDecimal(decimal minValue, decimal maxValue)
        {
            return (decimal)rand.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
