using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Pets
{
    public class SquidItem : HJScarletPetItem
    {
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<SquidProj>(), BuffType<SquidBuff>());
        }

        public override void ExSD()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
        }
    }
}
