using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerFlare : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            HomingToTarget
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public bool StopDrawing = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(18);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = GetSeconds(10);
            Projectile.extraUpdates = 3;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen() || StopDrawing)
                return;
            Vector2 spawnPos = Projectile.Center - Projectile.SafeDir() * 25f;
            if (Main.rand.NextBool(3))
                new ShinyCrossStar(spawnPos.ToRandCirclePosEdge(4f), Projectile.velocity / 8f, RandLerpColor(Color.Red, Color.Orange), 40, 0, 1, 0.45f, false).Spawn();
            new SmokeParticle(spawnPos.ToRandCirclePosEdge(4f), Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 4.8f), RandLerpColor(Color.OrangeRed, Color.White), 40, RandRotTwoPi, 1, 0.18f * Main.rand.NextFloat(0.45f, 1.1f), true).Spawn();
        }

        public void UpdateAttackAI()
        {
            if (Projectile.TooAwayFromOwner() && !StopDrawing)
            {
                StopDrawing = true;
                return;
            }
            if (!StopDrawing)
                return;
            //距离玩家足够远的时候干掉绘制，然后延迟5帧之后立刻返回至玩家底下
            Projectile.extraUpdates = 0;
            Timer++;
            StopDrawing = true;
            //仅仅是个计数器
            Projectile.frameCounter++;

            if (Projectile.frameCounter < 5)
            {
                Projectile.timeLeft = GetSeconds(5);
                return;
            }
            Projectile.Center = Owner.Center;
            //开始准备召唤需要的火力空袭
            if (Projectile.timeLeft % 2 == 0)
            {
                Vector2 spawnPos = Main.MouseWorld - Vector2.UnitY * 1200f + Vector2.UnitX * Main.rand.NextFloat(20f, 100f) * Main.rand.NextBool().ToDirectionInt();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, Vector2.UnitY * 30f, ProjectileType<SundownerRocket>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                ((SundownerRocket)proj.ModProjectile).ShouldHome = Owner.controlUseTile;
                if (Projectile.timeLeft % 8 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item45 with { Pitch = 0, MaxInstances = 0 }, Owner.Center);
                }
                if (Projectile.timeLeft % 6 == 0)
                {
                    proj.ai[0] = 1;

                }

            }
        }

        public void DoHomingToTarget()
        {
        }

        public void DoShoot()
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (StopDrawing)
                return false;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float oriScale = 0.94f;
            float scale = 1f;
            int length = (int)(12 * Projectile.Opacity);
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.DarkOrange, Color.OrangeRed, (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.4f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.Opacity, 0, 0);
                edgeColor = Color.Lerp(Color.Orange, Color.White, (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor * 0.25f, rot, ori, oriScale * scale * Projectile.Opacity * 0.75f, 0, 0);
            }
            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation + PiOver2, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
