using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class TearEyeProj : ExecutorWhipProj
    {
        public override int OriginalWhip => ItemType<TearEye>();
        public override (Texture2D LineTexture, Color LineColor, int LineEndCut, bool FullBright) LineSetting => (
            TextureAssets.FishingLine.Value, Color.SkyBlue, HeadPosOffsetFactor, false);
        public override (int SegmentCount, float RangeFactor, int ExtraUpdates, int SpriteFrames) WhipDefaults => (
            12, 0.40f, 1, 4);
        public override (int ExecutorProgressAdd, float PenetrateDamageRedcution) WhipHitDefaults => (1, 0.15f);

        public override int HeadPosOffsetFactor => base.HeadPosOffsetFactor;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnWhipActualSwinging(float swingProgress)
        {
            base.OnWhipActualSwinging(swingProgress);
        }
        public override void DrawWhipInPreDraw(List<Vector2> list, Texture2D texture, Rectangle frame, Vector2 origin, SpriteEffects flip)
        {
            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 ori = origin;
                int frameHeight = frame.Height + 4;
                float scale = 1f;
                ori.Y -= 6f;
                if (i == 0)
                {
                    frame.Y = 0;
                    frame.Height = 32 - 0;
                }
                else
                {
                    if (i > Projectile.WhipSettings.Segments / 2)
                    {
                        ori.Y -= 4;
                        frame.Y = 66;
                        frame.Height = 88 - 66;
                    }
                    else
                    {
                        ori.Y -= 4;
                        frame.Y = 38;
                        frame.Height = 60 - 38;
                    }
                }
                if (i == list.Count - 2)
                {
                    frame.Y = 94;
                    frame.Height = 126 - 94;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.25f, Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true));
                }
                Vector2 elment = list[i];
                Vector2 differ = list[i + 1] - elment;
                float rot = differ.ToRotation() - PiOver2;
                SB.Draw(texture, pos - Main.screenPosition, frame, Color.White, rot, ori, scale, flip, 0);
                pos += differ;
            }
        }
        public override void DrawMiscOnHead(Vector2 vector2, SpriteEffects flip)
        {
            base.DrawMiscOnHead(vector2, flip);
        }
        public override void ExOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.ExOnHitNPC(target, hit, damageDone);
        }

    }
}