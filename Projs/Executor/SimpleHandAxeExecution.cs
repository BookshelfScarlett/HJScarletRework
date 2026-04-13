using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SimpleHandAxeExecution : HJScarletProj
    {
        public override string Texture => GetInstance<SimpleHandAxeProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public bool IsHit = false;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public NPC CurTarget = null;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            Projectile.velocity *= 1.4f;
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Toss with {Variants = [1], MaxInstances = 0, Pitch = -0.412f, Volume = 0.825f, PitchVariance = 0.25f }, Projectile.Center);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            if(IsHit)
            {
                Timer++;
                if (!CurTarget.IsLegal())
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.rotation += Projectile.SpeedAffectRotation(12,12) / 4f;
                Projectile.velocity *= 0.87f;
                if ((Projectile.Center - CurTarget.Center).LengthSquared() < 50f * 50f)
                    Projectile.velocity = Projectile.SafeDir() * 16f;
                else
                {
                    if (Timer > 30f)
                    {
                        if (Timer == 31f)
                            Projectile.velocity = (Projectile.Center - CurTarget.Center).ToSafeNormalize() * (-27f - Projectile.numHits *3f);
                        Projectile.HomingTarget(CurTarget.Center, -1, 28f + Projectile.numHits * 3f, 14f, 15f);
                    }
                }
            }
            else
            {
                Projectile.rotation += 0.2f;
            }
            if (Projectile.IsOutScreen())
                return;
                        if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
                d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f) / 4f;
            }
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4f), DustID.SilverCoin, -Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f));
                d.noGravity = true;
                d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f) / 4f;
                d.scale *= 1.1f;
            }
            if(Main.rand.NextBool(4))
            {
                new StarShape(Projectile.Center.ToRandCirclePosEdge(8f), Projectile.velocity / 8f, RandLerpColor(Color.White, Color.DarkGoldenrod), 0.45f, 40).Spawn();
            }

        }
        public override bool? CanHitNPC(NPC target)
        {
            if (IsHit)
            {
                if (target.Equals(CurTarget) && CurTarget.IsLegal() && Timer > 30f)
                    return null;
                else
                    return false;
            }
            else
                return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsHit && target.IsLegal())
            {
                CurTarget = target;
                IsHit = true;
            }
            Timer = 0;
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with {Variants = [2], MaxInstances = 0, Pitch = 0.412f + Projectile.numHits * 0.1f, Volume = 0.925f }, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity.ToRandVelocity(ToRadians(20f), 2.4f, 14.7f), RandLerpColor(Color.Gold, Color.Silver), 40, RandRotTwoPi, 1, 1, false).Spawn();
            }
            for (int i = 0; i < 8; i++)
            {
                new StarShape(Projectile.Center.ToRandCirclePosEdge(10f) - Projectile.SafeDir() * 10f, Projectile.velocity.ToRandVelocity(ToRadians(2f), 1.4f, 14f), Color.Silver, 0.8f, 40).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            for(int i =0;i<8;i++)
            {
                SB.Draw(projTex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.2f, null, Color.White.ToAddColor(), Projectile.rotation, ori, Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
