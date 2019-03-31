using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessor1
{
    class Frequency
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public int Amount { get; set; }

        public static explicit operator Frequency(DbSet<Frequency> v)
        {
            throw new NotImplementedException();
        }
    }
}
