using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Buffs.Pets
{
    public class LifeWormBuff : PetsBuff
    {
        public override int PetProjType => ProjectileType<LifeWormProj>();
    }
}
