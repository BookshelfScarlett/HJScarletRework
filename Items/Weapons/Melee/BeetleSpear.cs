using HJScarletRework.Assets.Registers;
using HJScarletRework.Projs.Melee;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class BeetleSpear : ThrownSpearClass
    {
        public override void ExSD()
        {
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 32;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.Evolution_Thrown with { MaxInstances = 0 };
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<FierySpearProj>();
            Item.rare = ItemRarityID.Orange;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Spear).
                AddIngredient(ItemID.BeetleHusk, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
