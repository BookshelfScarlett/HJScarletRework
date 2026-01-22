using HJScarletRework.Assets.Registers;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SodomsDisaster : ThrownSpearClass
    {
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 75;
            Item.useTime = Item.useAnimation = 24;
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 14;
            Item.shoot = ProjectileType<SodomsDisasterProj>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = HJScarletSounds.SodomsDisaster_Toss with { MaxInstances = 0, Pitch= 0.21f, Volume = 0.74f};
        }
        public override Color MainTooltipColor => Color.Crimson;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FierySpear>().
                AddIngredient<LightBiteThrown>().
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
