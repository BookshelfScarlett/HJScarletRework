using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class TheJudgement : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 30;
        public override float ExecutionStrikeDamageMult => 1f;
        public override void ExSD()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<TheJudgementProj>();
            Item.knockBack = 8f;
            Item.width = Item.height = 58;
            Item.damage = 54;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem();
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                HallowedRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ThePunishment>().
                AddIngredient(ItemID.ChlorophyteBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
