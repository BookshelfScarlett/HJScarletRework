using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Melee
{
    public class FlowerofDanceFlower: HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float OriginalRotation => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
        }
        public bool Reverse = false;
        public override void OnFirstFrame()
        {
            OriginalRotation = RandRotTwoPi;
        }
        public override void ProjAI()
        {
            Projectile.velocity *= 0.96f;
            Timer++;
            DrawParticle();
            Projectile.rotation += Lerp(0.3f, 0.01f, Clamp(Timer / 60f,0,1));
            if (Timer >= 65f )
            {
                if (Projectile.ai[1] == 1)
                {
                    if (Projectile.IsMe() && CurTarget.IsLegal())
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            float rad = ToRadians(360f / 5 * i) + Projectile.rotation;
                            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, rad.ToRotationVector2() * 5f, ProjectileType<FlowerofDancePetal>(), Projectile.originalDamage, 1f, Owner.whoAmI);
                            ((FlowerofDancePetal)proj.ModProjectile).CurTarget = CurTarget;
                            proj.extraUpdates = 1;
                        }
                    }
                    SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { Variants = [1], MaxInstances = 1, Pitch = 0.3f, PitchVariance = .1f, Volume = 0.5f });
                    for (int i = 0; i < 12; i++)
                    {
                        ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16), Main.rand.NextFloat(1.1f, 4.6f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 120, 1, 0.1f * Main.rand.NextFloat(0.85f, 1.15f), RandRotTwoPi, glowMult: 0.65f, blendState: BlendState.Additive);
                    }
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.timeLeft = 250;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }
        public void DrawParticle()
        {
            if (Projectile.velocity.Length() > Main.rand.NextFloat())
            {
                if(Main.rand.NextBool(9))
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(18f), Projectile.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.1f, 1.5f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 40, 1, Projectile.scale * Main.rand.NextFloat(0.85f, 1.15f) * 0.2f, 0.2f, BlendState.Additive);
                }


            }
            else
            {
                if (Main.rand.NextBool(6))
                {
                    ECSParticle.SnowCloud(Projectile.Center.ToRandCirclePos(15f), Projectile.SafeDir() * Main.rand.NextFloat(0.1f, 1.5f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 40, 0, 0.73f, 0.1f * 0.25f, BlendState.Additive);
                }
                if(Main.rand.NextBool(9))
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(18f), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 1.5f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 40, 1, Projectile.scale * Main.rand.NextFloat(0.85f, 1.15f) * 0.2f, 0.2f, BlendState.Additive);
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
