using HJScarletRework.Projs.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Buffs.Pets

{
        public class DiverBuff: PetsBuff
    {
        public override int PetProjType => ProjectileType<DiverProj>();
    }
}
