using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.Neptunia_3_Data
{
    public enum Nation : byte
    {
        BlackScreenNation,
        Planeptun,
        Lowee,
        Laststation,
        Leanbox,
        Eden,
        PCMasterRace,
        HelloContinent,
        Hyperdimension
    }
    internal static class DungeonToNation
    {
        internal static readonly Dictionary<short,Nation> link = new Dictionary<short, Nation>() {
            {1, Nation.Hyperdimension},{2,Nation.Hyperdimension},{3,Nation.Planeptun},{4,Nation.Planeptun},{5,Nation.Laststation},{6,Nation.Laststation},{8,Nation.Laststation},{9,Nation.Laststation},{11,Nation.Laststation},
            {12,Nation.Lowee},{13,Nation.Lowee},{16,Nation.Lowee},{17,Nation.Laststation},{19,Nation.Leanbox},{20,Nation.Laststation},{21,Nation.Lowee},{22,Nation.Leanbox},{23,Nation.Planeptun},{25,Nation.Lowee},{27,Nation.Lowee},
            {28,Nation.Planeptun},{29,Nation.Laststation},{31,Nation.Eden},{33,Nation.Eden},{34,Nation.Hyperdimension},{35,Nation.Planeptun},{36,Nation.Planeptun},{37,Nation.Planeptun},{39,Nation.Laststation},{40,Nation.Laststation},
            {41,Nation.Laststation},{42,Nation.Lowee},{43,Nation.Lowee},{44,Nation.Lowee},{46,Nation.Leanbox},{47,Nation.Leanbox},{48,Nation.Leanbox},{50,Nation.Eden},{52,Nation.Eden},{53,Nation.Eden},{54,Nation.Hyperdimension},
            {56,Nation.Hyperdimension},{58,Nation.Hyperdimension},{59,Nation.HelloContinent},{61,Nation.HelloContinent},{62,Nation.HelloContinent},{63,Nation.PCMasterRace},{64,Nation.PCMasterRace},{65,Nation.PCMasterRace},
            {67,Nation.Planeptun},{302,Nation.Planeptun }
        };
        internal static Nation GetNation(short dungeon)
        {
            if (link.ContainsKey(dungeon))
                return link[dungeon];
            else
                return Nation.BlackScreenNation;
        }
    }
}
