using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class TrueScarlet : ExecutorWeaponClass
    {
        public override int ExecutionTime => 25;
        public override void ExSD()
        {
            Item.width = Item.height = 126;
            Item.damage = 98;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shootSpeed = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 22;
            Item.shoot = ProjectileType<TrueScarletProj>();
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DreamingLight>().
                AddIngredient<TheJudgement>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
