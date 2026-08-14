using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class StarofHopeProj : ExecutorWhipProj
    {
        public override int OriginalWhip => ItemType<StarofHope>();
        public override (Texture2D LineTexture, Color LineColor, int LineEndCut, bool FullBright) LineSetting => (
            TextureAssets.FishingLine.Value, Color.SkyBlue, HeadPosOffsetFactor, false);
        public override int HeadPosOffsetFactor => 2;
        public bool HasFireStar = false;
        public override void OnWhipActualSwinging(float swingProgress)
        {
            if (Projectile.IsMe() && !HasFireStar)
            {
                ScarletSound(HJScarletSounds.Misc_Ding, Owner.Center, .6f, 0, .4f);
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Owner.Center - Vector2.UnitY * Main.rand.NextFloat(600, 700) + Main.rand.NextFloat(80, 120) * i * Vector2.UnitX * Main.rand.NextBool().ToDirectionInt() - ((Main.MouseWorld.X - Owner.Center.X) > 0).ToDirectionInt() * Vector2.UnitX * 100;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, pos.GetNormalVector2(Main.MouseWorld) * 30f, ProjectileType<StarofHopeStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                HasFireStar = true;
            }
        }
        public override void DrawWhipInPreDraw(List<Vector2> list, Texture2D texture, Rectangle frame, Vector2 originalOrigin, SpriteEffects flip)
        {
            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 origin = originalOrigin;
                int frameHeight = frame.Height + 4;
                float scale = 1f;

                //把手。
                if (i == 0)
                {
                    //原点略微上移，以确保抓到把手位置
                    origin.Y -= 4f;
                }
                else
                {
                    //这样。设置正确的帧图位置。
                    int segmentToDraw = 1;
                    frame.Y = frameHeight * segmentToDraw;
                }

                bool isHead = i == list.Count - 2;
                //如果是头结点（鞭末尾），准备设置一个从大到小的scale动画。
                if (isHead)
                {
                    //帧图中鞭末端的位置
                    frame.Y = frameHeight * 2 - 6;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = Lerp(0.5f, 1.35f, Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true));

                }
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;
                float rotation = diff.ToRotation() - PiOver2;
                //鞭末端才会无视亮度
                Color color = isHead ? Color.White : Lighting.GetColor(pos.ToTileCoordinates(), Color.White);
                //鞭末端是一个星星，这里会让其全局自转起来
                rotation += isHead ? (float)Main.timeForVisualEffects * .051f : 0;
                //不是把手，我们才描边
                if (i != 0)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        SB.Draw(texture, pos + (TwoPi / 16f * j).ToRotationVector2() * 1.5f - Main.screenPosition, frame, color.ToAddColor(), rotation, origin, scale, flip, 0);
                    }
                }
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                pos += diff;
            }
        }
        public override void DrawMiscOnHead(Vector2 vector2, SpriteEffects flip)
        {

        }
        public override void ExOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.ExOnHitNPC(target, hit, damageDone);
        }
    }
}
