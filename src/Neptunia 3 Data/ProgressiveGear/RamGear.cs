using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear
{
    internal class RamGear:ProgressiveGear
    {
        public RamGear()
        {
            GearList.Add(0, new List<short>() { 1259 });
            GearList.Add(1, new List<short>() { 1260 });
            GearList.Add(2, new List<short>() { 1261 });
            GearList.Add(3, new List<short>() { 1262 });
            GearList.Add(4, new List<short>() { 1264 });
        }
        
    }
}
