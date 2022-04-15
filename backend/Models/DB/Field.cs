using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public string Polycentr { get; set; }
        public bool deleted { get; set; }

    }

}
