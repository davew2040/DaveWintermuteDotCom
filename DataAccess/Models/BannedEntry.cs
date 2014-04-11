using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class BannedEntry
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }
        [Required]
        public string IpAddress { get; set; }

        public string Description { get; set; }
    }
}
