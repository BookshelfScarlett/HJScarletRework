using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class CryoblazeHymn : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.damage = 74;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<CryoblazeHymnProj>();
            Item.rare = ItemRarityID.Orange;
        }
        public override Color MainTooltipColor => Color.AliceBlue;
        public override void AddRecipes()
        {
            if (!HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient<FierySpear>().
                    AddIngredient<AzureFrostmark>().
                    AddIngredient(ItemID.SoulofFright, 15).
                    AddIngredient(ItemID.SoulofSight, 15).
                    AddIngredient(ItemID.SoulofMight, 15).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient<FierySpear>().
                    AddIngredient<AzureFrostmark>().
                    AddRecipeGroup(HJScarletRecipeGroup.AnyMechBossSoul, 15).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
        }
    }
}
