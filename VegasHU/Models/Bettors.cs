using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegasHU.Models
{
    public class Bettors
    {
        public int BettorsId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public int Balance { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Bets> Bets { get; set; }
    }

}
