using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class ShadowItem : HJScarletPetItem
    {
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<ShadowProj>(), BuffType<ShadowBuff>());
        }
    }
}
