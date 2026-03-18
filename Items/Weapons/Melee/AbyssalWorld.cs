using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class AbyssalWorld : ThrownSpearClass
    {
        public override bool HasLegendary => false;
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.width = Item.height = 72;
            Item.damage = 108;
            Item.useTime = Item.useAnimation = 35;
            Item.rare = ItemRarityID.Yellow;
            Item.shootSpeed = 18f;
            Item.shoot = ProjectileType<AbyssalWorldProj>();
        }
        public override Color MainTooltipColor => Color.Aqua;
    }
}
