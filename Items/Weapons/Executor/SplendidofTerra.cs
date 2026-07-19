using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class SplendidofTerra : ExecutorWeaponClass
    {
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override int ExecutionProgress => 18 * (GaiaStriker.BloodBulletCount - 2);
        public override void ExSSD()
        {
            HJScarletList.MiscRarityDrawDictionary.Add(Type, LivingRarity.DrawRarity);
            Type.ShimmerEach<GaiaStriker>();
        }
        public override void ExSD()
        {
            Item.CloneDefaults(ItemType<GaiaStriker>());
        }
        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Type, out Texture2D itemDrawFrame, out Rectangle frame);
            Vector2 ori = frame.Size() / 2f;
            Vector2 pos = Item.Bottom - Main.screenPosition - new Vector2(0, ori.Y);
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(itemDrawFrame, pos + (TwoPi / 16f * i).ToRotationVector2() * 2, null, Color.Lime.ToAddColor(), rotation, ori, scale, 0, 0);
            spriteBatch.Draw(itemDrawFrame, pos, null, Color.White, rotation, ori, scale, 0, 0);
            return false;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == Mod.Name && (line.Name == "FlavorTooltipsName"))
            {
                LivingRarity.DrawFlavorRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }
}
