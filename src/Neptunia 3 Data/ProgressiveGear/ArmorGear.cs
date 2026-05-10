using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.src.Neptunia_3_Data.ProgressiveGear
{
    internal class ArmorGear:ProgressiveGear
    {
        public ArmorGear()
        {
            GearList.Add(0, new List<short>() { 1632, 1633 });
            GearList.Add(1, new List<short>() { 1641, 1642 });
            GearList.Add(2, new List<short>() { 1650, 1651 });
            GearList.Add(3, new List<short>() { 1655, 1656 });
            GearList.Add(4, new List<short>() { 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664, 1665, 1666 });
            GearList.Add(5, new List<short>() { 1667, 1668, 1669, 1670 });
            Amount = 20;
        }
    }
}
