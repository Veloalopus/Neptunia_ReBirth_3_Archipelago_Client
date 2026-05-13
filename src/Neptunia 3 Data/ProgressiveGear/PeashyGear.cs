using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear
{
    internal class PeashyGear:ProgressiveGear
    {
        public PeashyGear()
        {
            GearList.Add(0, new List<short>() { 1235 });
            GearList.Add(1, new List<short>() { 1236, 1237 });
            GearList.Add(2, new List<short>() { 1238, 1239 });
            GearList.Add(3, new List<short>() { 1241 });
            GearList.Add(4, new List<short>() { 1242 });
            GearList.Add(5, new List<short>() { 1243 });
        }
    }
}
