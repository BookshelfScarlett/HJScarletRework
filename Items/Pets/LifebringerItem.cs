using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Items.Pets
{
    internal class LifeWormItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<LifeWormProj>();
        public override int PetBuffType => BuffType<LifeWormBuff>();
    }
}
