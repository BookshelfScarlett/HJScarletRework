using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using Terraria;

namespace HJScarletRework.Items.Pets
{
    public class NoneItem : HJScarletPetItem
    {
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<NoneProj>(), BuffType<NoneBuff>());
        }
    }
}
