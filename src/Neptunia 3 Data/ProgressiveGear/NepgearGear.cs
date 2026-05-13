using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear
{
    internal class NepgearGear:ProgressiveGear
    {
        public NepgearGear()
        {
            GearList.Add(0, new List<short>() { 1216 });
            GearList.Add(1, new List<short>() { 1217, 1223 });
            GearList.Add(2, new List<short>() { 1224, 1227 });
            GearList.Add(3, new List<short>() { 1228, 1229 });
            GearList.Add(4, new List<short>() { 1230 });
            GearList.Add(5, new List<short>() { 1231, 1232, 1233 });
        }
    }
}
