using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using Terraria.ID;

namespace HJScarletRework.Items.Pets
{
    public class WatcherItem : HJScarletPetItem
    {
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<WatcherProj>(), BuffType<WatcherBuff>());
        }

        public override void ExSD()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuPetItem);
        }
    }
}
