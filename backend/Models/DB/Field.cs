using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class field
    {
        public int id { get; set; }
        public string name { get; set; }
        public string coordinates { get; set; }
        public string polycentr { get; set; }
        public bool deleted { get; set; }

    }

}
