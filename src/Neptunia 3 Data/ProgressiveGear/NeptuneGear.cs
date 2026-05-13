using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear

{
    internal class NeptuneGear :ProgressiveGear
    {
        public NeptuneGear()
        {
            GearList.Add(0,new List<short>(){1101 });
            GearList.Add(1,new List<short>(){1102, 1110 });
            GearList.Add(2,new List<short>(){1111, 1113 });
            GearList.Add(3,new List<short>(){1114, 1119 });
            GearList.Add(4, new List<short>(){1120, 1122 });
            GearList.Add(5, new List<short>(){1123, 1124 });
            GearList.Add(6, new List<short>(){1125 });
        }

    }
}
