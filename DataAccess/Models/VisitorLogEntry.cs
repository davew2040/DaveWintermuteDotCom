using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class VisitorLogEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public DateTime VisitTime { get; set; }
    }
}
