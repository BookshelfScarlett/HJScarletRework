using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DreamingLight : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1f;
        public override int ExecutionTime => 15;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.shootSpeed = 20f;
            Item.useTime = Item.useAnimation = 40;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.rare = RarityType<NightRarity>();
            Item.shoot = ProjectileType<DreamingLightProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { MaxInstances = 0, Pitch = -0.5f };
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "FlavorTooltipsName", value)
            {
                OverrideColor = Color.Lerp(Color.MediumPurple, Color.LightPink, 0.3f)
            };
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                NightRarity.DrawRarity(line);
                return false;
            }
            if (line.Mod == "Terraria" && line.Name == "CritChance")
            {
                NightRarity.DrawMisc(line);
                return false;
            }
            if (line.Mod == "Terraria" && line.Name == "Damage")
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DreamlessNight>().
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.Beds).
                Register();
        }
    }
}
