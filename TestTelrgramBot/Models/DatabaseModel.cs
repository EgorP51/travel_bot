using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelrgramBot.Models
{
    public class DatabaseModel
    {
        public string? UserId { get; set; }
        public string? City { get; set; }
        public string[]? NewValue { get; set; }
    }
}
