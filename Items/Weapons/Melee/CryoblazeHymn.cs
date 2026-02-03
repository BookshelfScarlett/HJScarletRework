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
        public override void ExSD()
        {
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 32;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<CryoblazeHymnProj>();
            Item.rare = ItemRarityID.Orange;
        }
        public override Color MainTooltipColor => Color.AliceBlue;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.Name == "ItemName" && line.Mod == "Terraria")
            {
                RarePets.DrawCustomTooltipLine(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void AddRecipes()
        {
            if (!HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient<FierySpear>().
                    AddIngredient<AzureFrostmark>().
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
                    AddIngredient<AzureFrostmark>().
                    AddRecipeGroup(HJScarletRecipeGroup.AnyMechBossSoul, 15).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
        }
    }
}
