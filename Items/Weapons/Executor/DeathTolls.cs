using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DeathTolls : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionProgress => 25;
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override void ExSSD()
        {
            HJScarletList.NightRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<DeathTollsProj>();
            Item.knockBack = 8f;
            Item.width = 88;
            Item.height = 94;
            Item.damage = 214;
            Item.useTime = 21;
            //这里的UseTime是有意改的很慢的
            Item.useAnimation = 21;
            Item.shootSpeed = 19f;
            //这里不会给音效，因为要考虑一些射弹的联动
            //实际音效会在射弹初始化的时候提供
            Item.UseSound = null;

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (!HJScarletConfigClient.Instance.SpecialRarity)
                return base.PreDrawTooltipLine(line, ref yOffset);

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
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value)
            {
                OverrideColor = Color.Lerp(Color.MediumPurple, Color.LightPink, 0.3f)
            };

            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex + 1, flavorTooltips);
        }
    }
}
