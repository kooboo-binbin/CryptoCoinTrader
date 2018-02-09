using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CryptoCoinTrader.Core.Data.Entities
{
    public class Exchange
    {
        [Key]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// we save json on here
        /// </summary>
        public string Settings { get; set; }
    }
}
