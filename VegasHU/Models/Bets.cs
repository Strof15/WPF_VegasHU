using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegasHU.Models
{
    public class Bets
    {
        public int BetsID { get; set; }
        public DateTime BetDate { get; set; }
        public float Odds { get; set; }
        public int Amount { get; set; }
        public int BettorsID { get; set; }
        public int EventID { get; set; }
        public int Status { get; set; }

        public Bettors Bettor { get; set; }
        public Event Event { get; set; }
    }
}
