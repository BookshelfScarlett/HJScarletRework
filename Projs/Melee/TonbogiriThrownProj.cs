using ContinentOfJourney.Dusts;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<Tonbogiri>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting();
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 projDir = Projectile.SafeDirByRot();
            Timer++;
            if(Timer % 12f == 0)
            {
                for (int i = -1; i< 2 ;i+=2)
                {
                    //位置和速度都会有一定的偏差，带点随机性
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(3f, 3f);
                    Vector2 dir = projDir.RotatedBy(Main.rand.NextFloat(PiOver4 / 4, PiOver4) * i);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, dir * -Main.rand.NextFloat(6f, 9f), ProjectileType<TonbogiriBubble>(), Projectile.damage, 0f);
                    //这里TimeLeft是随机的。
                    Projectile.timeLeft = Main.rand.Next(300, 601);
                    //依旧把敌人的index传入进去，我们需要让大部分射弹优先集中玩家直接命中的单位
                    proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
                }
            }
            //开始绘制需要散发的一些粒子，暂时不考虑
            //new TurbulenceShinyOrb(Projectile.Center + Main.rand.NextVector2CircularEdge(3f, 3f), 0.5f, Color.Blue.RandLerpTo(Color.DeepSkyBlue), 40, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //存入第一个敌人
            if (Projectile.HJScarlet().GlobalTargetIndex == -1)
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            DrawTrail(drawPos, 1);
            DrawTrail(drawPos, -1);
            //在尖端绘制一个圈，这个圈会随常规自转
            DrawGlowCircle(drawPos);
            Projectile.DrawGlowEdge(Color.White, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White, rotFix:ToRadians(135));
            return false;
        }

        private void DrawGlowCircle(Vector2 drawPos)
        {
            Vector2 projDir = Projectile.SafeDirByRot();
        }

        public void DrawTrail(Vector2 drawPosBase, int dir)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 offset = Projectile.SafeDir() * 60f;
            int length = 24;
            Vector2 trailScale = Projectile.scale * new Vector2(0.5f, 1.5f) * 0.7f;
            Vector2 projDir = Projectile.SafeDirByRot();
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Vector2 drawPos = drawPosBase + offset + projDir.RotatedBy(PiOver2 * dir) * 8f - projDir * 8f * i;
                Color drawColor = (Color.Lerp(Color.Blue, Color.DeepSkyBlue, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * Clamp(Projectile.velocity.Length(), 0 ,1);
                //中心高光的颜色
                Color drawColor2 = (Color.Lerp(Color.AliceBlue, Color.White, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * Clamp(Projectile.velocity.Length(), 0, 1);
                SB.Draw(star, drawPos, null, drawColor, Projectile.rotation - PiOver2, star.Size() / 2, trailScale, 0, 0);
                SB.Draw(star, drawPos, null, drawColor2, Projectile.rotation - PiOver2, star.Size() / 2, trailScale * 0.4f, 0, 0);
            }
        }
    }
}
