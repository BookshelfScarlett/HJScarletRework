using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!item.IsLegal() || Main.dedServ)
                return true;
            HandleLerpValue(item, position, scale);
            if (Math.Abs(1f - simpleImmersiveBackpackValueAlt) > .03f)
            {
                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, new Rectangle?(frame), drawColor, 0, origin, scale * simpleImmersiveBackpackValue, 0, 0f);
                return false;
            }
            return true;
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawSpecialIconDisplay(item, spriteBatch, position);
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawGhost && (tintIconDrawLerp) <= .98f)
            {
                Vector2 iconPosition = position + new Vector2(15f * Main.inventoryScale, 15f * Main.inventoryScale);
                float iconScale = 0.31f * simpleImmersiveBackpackValueAlt;
                Rectangle rect = new(0, GhostFrame * 44, 46, 42);
                Vector2 recorigin = new(23, 21);
                Texture2D tex = HJScarletTexture.ScarletGhost.Value;
                for (int i = 0; i < 6; i++)
                    spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, rect, Color.White.ToAddColor() * (1f - tintIconDrawLerp), 0f, recorigin, iconScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, iconPosition, rect, Color.White * (1f - tintIconDrawLerp), 0f, recorigin, iconScale, SpriteEffects.None, 0f);

            }
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawIcon && (tintIconDrawLerp) <= 0.98f)
            {
                Vector2 iconPosition = position + new Vector2(15f * Main.inventoryScale, 12f * Main.inventoryScale);
                float iconScale = 0.34f * simpleImmersiveBackpackValueAlt;
                Texture2D tex = HJScarletTexture.LostbeltJourneyIconLegacy.Value;
                for (int i = 0; i < 6; i++)
                    spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * (1f - tintIconDrawLerp), 0f, tex.ToOrigin(), iconScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, iconPosition, null, Color.White * (1f - tintIconDrawLerp), 0f, tex.ToOrigin(), iconScale, SpriteEffects.None, 0f);
            }
        }
        public void DrawSpecialIconDisplay(Item item, SpriteBatch spriteBatch, Vector2 position)
        {
            if (tintIconDrawLerp <= 0.02f)
                return;
            //缩写。
            float time = (float)Main.timeForVisualEffects;
            //算提示图标的摆动
            float amp = 1.2f;
            float omega = .03f;
            float xOffset = (float)Math.Sin(time * omega) * amp * .7f;
            float yOffset = (float)Math.Cos(time * omega) * amp * 0.5f;
            Vector2 iconPosition = position - new Vector2(10) + new Vector2(xOffset, yOffset);
            iconPosition.Y -= Lerp(5f, 0f, tintIconDrawLerp);
            //算放缩
            float iconScale = Lerp(0.31f, 0.34f, (float)(Math.Abs(Math.Sin(time)))) * simpleImmersiveBackpackValueAlt;

            //算颜色情况
            Color lerpColor = Color.Lerp(Color.White, Color.Silver, (float)Math.Abs(Math.Sin(time * .05f)));

            //最后我们再尝试将其画出
            Texture2D tex = HJScarletTexture.InvisAsset.Value;
            //这个Type是个打表，用来打表mod内有特殊搭配的物品
            //现在我们没有wiki，因此是得这么做
            if (LocalPlayer.HJScarlet().drawUseableItemIcon != -1)
                tex = TextureAssets.Item[LocalPlayer.HJScarlet().drawUseableItemIcon].Value;
            Vector2 recorigin = tex.ToOrigin();
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * tintIconDrawLerp, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, iconPosition, null, lerpColor * tintIconDrawLerp, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
        }

    }
}
