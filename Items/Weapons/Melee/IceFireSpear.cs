using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class IceFireSpear : ThrownSpearClass
    {
        public override void ExSD()
        {
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 32;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<IceFireSpearProj>();
            Item.rare = ItemRarityID.Orange;
        }
        public override Color MainTooltipColor => Color.AliceBlue;
        public override void AddRecipes()
        {
            if (!HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient<FierySpear>().
                    AddIngredient<IceSpear>().
                    AddIngredient(ItemID.SoulofFright, 15).
                    AddIngredient(ItemID.SoulofSight, 15).
                    AddIngredient(ItemID.SoulofFright, 15).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient<FierySpear>().
                    AddIngredient<IceSpear>().
                    AddRecipeGroup(HJScarletRecipeGroup.AnyMechBossSoul, 15).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
        }
    }
}
