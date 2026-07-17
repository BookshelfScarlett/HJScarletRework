using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class EndlessWar : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionProgress => 15;
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override void ExSSD()
        {
            HJScarletList.MiscRarityDrawDictionary.Add(Type, MatterRarity.DrawRarity);
        }
        public override void ExSD()
        {
            Item.width = Item.height = 120;
            Item.damage = 1547;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 12f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shootSpeed = 16;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ExModifyTooltips(tooltips);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BinaryStars>().
                AddIngredient<DeathTolls>().
                AddIngredient<GaiaStriker>().
                AddIngredient<FinalBar>(5).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
