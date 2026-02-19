using HJScarletRework.Projs.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class TheMoss : ThrownHammerItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int NeedFocusStrikeTime => 10;
        public override float FocusDamageAddictive => 0f;
        public override int ShootProjID => ProjectileType<TheMossMainProj>();
        public override void ExSSD()
        {
            base.ExSSD();
        }
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.damage = 20;
            Item.useAnimation = Item.useTime = 28;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 14f;
            Item.knockBack = 2f;
        }
    }
}
