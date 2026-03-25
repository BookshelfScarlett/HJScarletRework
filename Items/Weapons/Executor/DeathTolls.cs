using ContinentOfJourney;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DeathTolls: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionTime => 20;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<DeathTollsProj>();
            Item.knockBack = 12f;
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.width = 88;
            Item.height = 94;
            Item.damage = 197;
            Item.useTime = 18;
            //这里的UseTime是有意改的很慢的
            Item.useAnimation = 18;
            Item.shootSpeed = 24f;
            Item.rare = RarityType<NightRarity>();
            //这里不会给音效，因为要考虑一些射弹的联动
            //实际音效会在射弹初始化的时候提供
            Item.UseSound = null;
            Item.value = Item.buyPrice(gold: 12);

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                NightRarity.DrawRarity(line);
                return false;
            }
            if(line.Mod == "Terraria" && line.Name == "CritChance")
            {
                NightRarity.DrawMisc(line);
                return false;
            }
            if(line.Mod == "Terraria" && line.Name == "Damage")
            {
                NightRarity.DrawMisc(line);
                return false;
            }

            if (line.Name == "FlavorTooltipsName" && line.Mod == Mod.Name)
            {
                NightRarity.DrawFlavorRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex + 1, flavorTooltips);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
        private static float UpdatePos
        {
            get
            {
                float value = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1f) * 1.2f + 1.4f);
                return Clamp(value, 1.0f, 2.4f);
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return base.PreDrawInInventory(spriteBatch, position,frame,drawColor,itemColor,origin,scale);
        }
    }
}
