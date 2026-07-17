using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class BoneSlapProj : ExecutorWhipProj
    {
        public override int OriginalWhip => ItemType<BoneSlap>();
        public override (Texture2D LineTexture, Color LineColor, int LineEndCut, bool FullBright) LineSetting => (
            TextureAssets.FishingLine.Value, Color.SkyBlue, HeadPosOffsetFactor, false);
        public override (int SegmentCount, float RangeFactor, int ExtraUpdates, int SpriteFrames) WhipDefaults => (
            10, 0.2f, 1, 4);
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
            base.DrawWhipInPreDraw(list, texture, frame, origin, flip);
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
