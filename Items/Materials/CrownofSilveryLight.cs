using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Materials
{
    public class CrownofSilveryLight : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Materials;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
            ItemID.Sets.ItemNoGravity[Type] = true;
        }
        public override void ExSD()
        {
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.material = true;
            Item.value = Item.sellPrice(gold: 4, silver: 30);
        }
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            string text = this.GetLocalizationKey("FlavorTooltip").ToLangValue();
            TextboxSettings sets = new TextboxSettings
            {
                HasTitle = false,
                BackgroundColor = Color.White * .24f,
                BackgroundEdgeColor = Color.White,
                TextColor = Color.White,
                TextEdgeColor = Color.Black,
                MainText = text
            };
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Type, out Texture2D itemTexture, out Rectangle itemFrame);
            Vector2 drawOrigin = itemFrame.Size() / 2;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(itemTexture, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 1.2f, itemFrame, Color.White.ToAddColor(), rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(itemTexture, drawPosition, itemFrame, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
