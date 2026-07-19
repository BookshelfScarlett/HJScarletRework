using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerBeam : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }
        public override void OnFirstFrame()
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(15f), 1f, 28f);
                BloodyMetaball.SpawnParticle(Projectile.Center + Projectile.SafeDir() * 10f, vel, 0.86f, vel.ToRotation(), false);
            }
            for (int i = 0; i < 22; i++)
            {
                Vector2 bloodVel = Projectile.SafeDir().RotatedByRandom(ToRadians(20)) * Main.rand.NextFloat(11f, 35.2f) - Vector2.UnitY * 5f;
                ECSParticle.BloodDrop(Projectile.Center.ToRandCirclePos(20, 5), bloodVel, RandLerpColor(Color.DarkRed, Color.Black), 40, 1f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * .14f, 1, true, BlendState.AlphaBlend);
            }
            for (int i = 0; i < 17; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(5, 5);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(.1f, 1.2f) * 10f;
                vel = Projectile.SafeDir().RotatedByRandom(PiOver4) * Main.rand.NextFloat(.1f, 1.2f) * 19f;
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.DarkRed, Color.Black), 16, RandRotTwoPi, 0.95f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.45f, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 17; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(5, 5);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(.1f, 1.2f) * 5f;
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.DarkRed, Color.Black),16, RandRotTwoPi, 0.95f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.45f, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }

        }
        public override void ProjAI()
        {
            Projectile.velocity *= 1.013f;
            int DustCount = 6;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < DustCount; i++)
                BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(11) + Projectile.velocity / DustCount * i, Projectile.rotation.ToRotationVector2(), Main.rand.NextFloat(0.4f, 0.55f) * .86f, Projectile.rotation);
            if (Main.rand.NextBool())
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(5, 5);
                ECSParticle.SmokeParticle(pos, Projectile.velocity.ToSafeNormalize(), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 0.55f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.575f, false, BlendState.NonPremultiplied);
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 bloodVel = -Vector2.UnitY.RotatedByRandom(ToRadians(45)) * Main.rand.NextFloat(5f, 7.2f);
                ECSParticle.BloodDrop(Projectile.Center.ToRandCirclePos(5, 5), bloodVel, RandLerpColor(Color.DarkRed, Color.Black), 60, 1f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * .14f, 0, true, BlendState.AlphaBlend);
            }
            if(Projectile.frameCounter > (3 * Projectile.MaxUpdates))
            {
                Projectile.frameCounter = 0;
                if(Projectile.GetTargetSafe(out NPC target, searchDistance:1000,canPassWall:true))
                {
                    Vector2 vel = Projectile.Center.GetNormalVector2(target.Center) * 7f;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<GaiaStrikerBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.ai[2] = Main.rand.Next(0, 2);
                    if (target.IsLegal())
                        ((GaiaStrikerBolt)proj.ModProjectile).CurTarget = target;
                }
            }
            Projectile.frameCounter++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(HJScarletRework.CrossMod_UCA.TryFind<ModItem>("CarnageRay", out ModItem value))
            {
                if (Owner.HeldItem.type == value.Type)
                {
                    if (Owner.statMana < Owner.statManaMax2 * 0.95f)
                        Owner.statMana += 10;
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
