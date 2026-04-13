using ContinentOfJourney.Items.Material;
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
    public class BinaryStars : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionTime => 20;
        public override void ExSD()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.width = Item.height = 86;
            Item.damage = 321;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.rare = RarityType<NebulaRarity>();
            Item.SetUpNoUseGraphicItem();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<BinaryStarsProj>();
            Item.knockBack = 12f;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.White with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            return false;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                NebulaRarity.DrawRarityReverse(line);
                return false;
            }
            if (line.Mod == "Terraria" && line.Name == "CritChance")
            {
                NebulaRarity.DrawMisc(line);
                return false;
            }

            if (line.Mod == "Terraria" && line.Name == "Damage")
            {
                NebulaRarity.DrawMisc(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        /// <summary>
        /// 双子星不再以微光作为前置。
        /// 现在双子星正常10个锭与下位的两个锤子
        /// 火山锤目前是个占位符，后续应该是要变成泰拉物品的
        /// </summary>
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AetherfireSmasher>().
                AddIngredient<DeathTolls>().
                AddIngredient<FinalBar>(10).
                AddTile<ContinentOfJourney.Tiles.FinalAnvil>().
                Register();
        }
    }
}