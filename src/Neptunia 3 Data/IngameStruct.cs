using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Memory;

namespace Nep3ArchipelagoClient.src.Neptunia_3_Data
{
    internal struct IngameStruct
    {
        public readonly nuint Offset;
        public readonly int ArraySize;
        public readonly Type Type;
    }
}
