using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class PrunusMumeFlower : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float OriginalRotation => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
        }
        public bool Reverse = false;
        public override void OnFirstFrame()
        {
            if (Projectile.HJScarlet().ExecutionStrike)
                Projectile.timeLeft = GetSeconds(31);
            OriginalRotation = Projectile.rotation;
        }
        public override void ProjAI()
        {
            Projectile.velocity *= 0.95f;
            Timer++;
            DrawParticle();
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.rotation += .02f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter - Vector2.UnitY * 130f, 0.2f);
                if (Projectile.MeetMaxUpdatesFrame(Timer, 60))
                {
                    Timer = 0;
                    if (!Projectile.IsMe())
                        return;
                    SoundEngine.PlaySound(SoundID.Item109 with { MaxInstances = 0, Pitch = 0.2f, PitchVariance = 0.1f });
                    for (int i = 0; i < 8; i++)
                    {
                        float rad = ToRadians(360f / 8 * i) + Projectile.rotation;
                        float speed = i % 2 == 0 ? 1.5f : 3f;
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, rad.ToRotationVector2() * speed * 2f, ProjectileType<PrunusMumePetal>(), Projectile.damage, 1f, Owner.whoAmI);
                        ((PrunusMumePetal)proj.ModProjectile).AttackStyle = PrunusMumePetal.Style.ExecutionStrike;
                        proj.extraUpdates = 2;
                        proj.timeLeft = 180;
                        proj.ai[2] = Reverse.ToDirectionInt();
                    }
                    Reverse = !Reverse;
                }
            }
            else
            {
                Projectile.rotation = Projectile.SpeedAffectRotation(1, 1) + OriginalRotation;
                if (Timer >= 60f)
                {
                    Timer = 0f;
                    if (Projectile.IsMe())
                    {
                        SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { Variants = [1], MaxInstances = 0, Pitch = 0.3f, PitchVariance = .1f, Volume = 0.5f });
                        for (int i = 0; i < 5; i++)
                        {
                            float rad = ToRadians(360f / 5 * i) + Projectile.rotation;
                            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, rad.ToRotationVector2() * 3f, ProjectileType<PrunusMumePetal>(), Projectile.damage, 1f, Owner.whoAmI);
                            ((PrunusMumePetal)proj.ModProjectile).AttackStyle = PrunusMumePetal.Style.NormalStrike;
                            proj.extraUpdates = 1;
                            proj.timeLeft = 100;
                        }
                        Projectile.Kill();
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return !Projectile.HJScarlet().ExecutionStrike;
        }
        public void DrawParticle()
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                if (Main.rand.NextBool(4))
                {
                    new SnowCloud(Projectile.Center.ToRandCirclePos(30f), Vector2.UnitY * -Main.rand.NextFloat(0.1f, 1.5f), RandLerpColor(Color.IndianRed, Color.HotPink), 40, 0, 0.73f, 0.1f * 0.5f, true).Spawn();
                }
                if (Main.rand.NextBool(8))
                {
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = Projectile.Center.ToRandCirclePos(30);
                        p.Velocity = Vector2.UnitY * -1.5f;
                        p.Scale = 0.105f * Main.rand.NextFloat(0.75f, 0.95f);
                        p.DrawColor = RandLerpColor(Color.IndianRed, Color.DarkRed);
                        p.Lifetime = 40;
                        p.Opacity = 1;
                        p.GlowCenterMult = 0.65f;
                    });
                }

                return;
            }
            if (Projectile.velocity.Length() > Main.rand.NextFloat())
            {
                if (Main.rand.NextBool())
                {
                    new SnowCloud(Projectile.Center.ToRandCirclePos(15f), Projectile.velocity / 4f, RandLerpColor(Color.IndianRed, Color.HotPink), 40, 0, 0.73f, 0.1f * 0.5f, true).Spawn();
                }
            }
            else
            {
                if (Main.rand.NextBool(4))
                {
                    new SnowCloud(Projectile.Center.ToRandCirclePos(15f), Projectile.SafeDir() * Main.rand.NextFloat(0.1f, 1.5f), RandLerpColor(Color.IndianRed, Color.HotPink), 40, 0, 0.73f, 0.1f * 0.5f, true).Spawn();
                }
                if (Main.rand.NextBool(8))
                {
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = Projectile.Center.ToRandCirclePos(15);
                        p.Velocity = Projectile.SafeDir() * 1.5f;
                        p.Scale = 0.105f * Main.rand.NextFloat(0.75f, 0.95f);
                        p.DrawColor = RandLerpColor(Color.IndianRed, Color.DarkRed);
                        p.Lifetime = 40;
                        p.Opacity = 1;
                        p.GlowCenterMult = 0.65f;
                    });
                }

            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float rad = Timer / 60f;
            Color glowColor = Color.Lerp(Color.Transparent, Color.White, rad);
            Projectile.DrawGlowEdge(glowColor);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
