using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear
{
    internal class RomGear:ProgressiveGear
    {
        public RomGear()
        {
            GearList.Add(0, new List<short>() { 1252 });
            GearList.Add(1, new List<short>() { 1253 });
            GearList.Add(2, new List<short>() { 1254 });
            GearList.Add(3, new List<short>() { 1255 });
            GearList.Add(4, new List<short>() { 1257 });
        }
    }
}
