using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelrgramBot.Constant
{
    public class Constants
    {
        //using environment variables
        // 5583909669:AAEUHGYg7NEA9VrWMz0gLiTmnn-zvt8qQwU
        public static readonly string BotKey = Environment.GetEnvironmentVariable("BotKey");
        
    }
}
