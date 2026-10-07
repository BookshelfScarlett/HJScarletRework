using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class ShadowItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<ShadowProj>();
        public override int PetBuffType => BuffType<ShadowBuff>();
    }
}
