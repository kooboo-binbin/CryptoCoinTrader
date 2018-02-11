using CryptoCoinTrader.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Data
{
    public class CoinContext : DbContext
    {
        public CoinContext(DbContextOptions<CoinContext> options) : base(options)
        {
        }

        public DbSet<Arbitrage> Arbitrages { get; set; }
        public DbSet<Observation> Observations { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasIndex(nameof(Order.ArbitrageId));
            modelBuilder.Entity<Order>().HasIndex(nameof(Order.ObservationId));
            modelBuilder.Entity<Arbitrage>().HasIndex(nameof(Order.ObservationId));
            // modelBuilder.Entity<Observation>().Property(it=>it.SpreadPercentage).
        }
    }
}