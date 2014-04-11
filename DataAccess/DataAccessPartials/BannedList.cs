using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class DataAccessProxy : IDataAccessProxy
    {
        public IEnumerable<Models.BannedEntry> GetBanList()
        {
            using (var context = new DaveAppContext())
            {
                return context.BannedList.ToList();
            }
        }

        public void AddBannedEntry(Models.BannedEntry newEntry)
        {
            using (var context = new DaveAppContext())
            {
                if (context.BannedList.Where(x => x.IpAddress == newEntry.IpAddress).Any())
                {
                    throw new Exception("Banned entry already exists with IP address " + newEntry.IpAddress);
                }

                context.BannedList.Add(newEntry);
            }
        }

        public Models.BannedEntry GetBannedEntryByHost(string hostName)
        {
            using (var context = new DaveAppContext())
            {
                return context.BannedList.Where(x => x.IpAddress == hostName).FirstOrDefault();
            }
        }

        public Models.BannedEntry GetBannedEntryById(int id)
        {
            using (var context = new DaveAppContext())
            {
                return context.BannedList.Where(x => x.Id == id).FirstOrDefault();
            }
        }
    }
}
