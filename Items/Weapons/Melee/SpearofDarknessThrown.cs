using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SpearofDarknessThrown : ThrownSpearClass
    {
        public override string Texture => HJScarletItemProj.Item_SpearofDarknessThrown.Path;
        public override bool HasLegendary => false;
        public int StrikeTime = 0;
        public override void ExSD()
        {
            Item.damage = 43;
            Item.useTime = Item.useAnimation = 29;
            Item.UseSound = SoundID.Item103;
            Item.knockBack = 4f;
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ProjectileType<SpearofDarknessThrownProj>();
        }
        public override Color MainTooltipColor => Color.MediumPurple;
    }
}
