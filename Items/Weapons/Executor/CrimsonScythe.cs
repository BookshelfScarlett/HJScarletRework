using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class CrimsonScythe : ExecutorWeaponClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ExecutionTime => 30;
        public int CurSwingTime = 0;
        public override void ExSD()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.width = 88;
            Item.height = 82;
            Item.damage = 456;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.rare = RarityType<DisasterRarity>();
            Item.shoot = ProjectileType<CrimsonScytheProj>();
            Item.shootSpeed = 10;
            Item.knockBack = 5;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2+1, flavorTooltips);

        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();

            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(tex, position + ToRadians((i * 5f) + 5f).ToRotationVector2() * 2.5f, null, Color.DarkRed with { A = 100}, 0f, tex.Size() / 2, scale * 1.02f, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            return false;
        }
        public override void PostUpdate()
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 pos = Item.position + tex.Size() / 2;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && (line.Name == "ItemName"))
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            if (line.Mod == "Terraria" && (line.Name == "CritChance"))
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }

            if (line.Mod == "Terraria" && (line.Name == "Damage"))
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }
            if (line.Mod == Mod.Name && (line.Name == "FlavorTooltipsName"))
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((CrimsonScytheProj)proj.ModProjectile).SwingType = (CrimsonScytheProj.SwingState)CurSwingTime;
            CurSwingTime++;
            if (CurSwingTime > 2)
                CurSwingTime = 0;
            return false;
        }
    }
}
