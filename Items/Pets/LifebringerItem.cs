using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    internal class LifeWormItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<LifeWormProj>();
        public override int PetBuffType => BuffType<LifeWormBuff>();
    }
}
