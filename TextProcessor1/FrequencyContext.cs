using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace TextProcessor1
{
    class FrequencyContext : DbContext
    {
        public FrequencyContext()
            : base("DbConnection")
        { }

        public DbSet<Frequency> Frequencies { get; set; }
    }
}
