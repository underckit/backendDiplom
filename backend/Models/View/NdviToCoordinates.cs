using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class NdviToCoordinates
    {
        public string coordinates { get; set; }
        public string ndvimap { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate{ get; set; }
        public string type { get; set; }
    }
}
