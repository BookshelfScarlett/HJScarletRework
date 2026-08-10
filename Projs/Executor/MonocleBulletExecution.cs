using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MonocleBulletExecution : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 12;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1400) && Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 1;
                Vector2 vel = Projectile.Center.GetNormalVector2(target.Center) * Projectile.oldVelocity.Length();
                Projectile.velocity = vel;
                Timer = 0;
            }
            else
            {
                Projectile.BounceOnTile(oldVelocity);
                Projectile.ai[1] = 0;
            }
            ECSParticle.CrossGlow(Projectile.Center, Color.Violet, 45, 1, 0.2f, 0.1f);
            ECSParticle.CrossGlow(Projectile.Center, Color.DarkViolet, 45, 1, 0.18f, .1f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 45, 1, 0.16f, .1f);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center, 1.1f, RandLerpColor(Color.Violet, Color.Purple), 45, 1, 0.12f, glowMult: .7f);
            }

            return false;
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 MountedPos = Projectile.SafeDir() * -15f;
            Timer++;
            if (Timer > Projectile.MaxUpdates * 3)
            {
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1400) && Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] = 1;
                    Vector2 vel = Projectile.Center.GetNormalVector2(target.Center) * Projectile.oldVelocity.Length();
                    Projectile.velocity = vel;
                    Timer = 0;
                    ECSParticle.CrossGlow(Projectile.Center, Color.Violet, 45, 1, 0.2f, 0.1f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.DarkViolet, 45, 1, 0.18f, .1f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.White, 45, 1, 0.16f, .1f);
                    for (int i = 0; i < 16; i++)
                    {
                        ECSParticle.TurbulenceShinyOrb(Projectile.Center, 1.1f, RandLerpColor(Color.Violet, Color.Purple), 45, 1, 0.12f, glowMult: .7f);
                    }
                }
            }
            if (Projectile.IsOutScreen())
                return;
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.Violet, 10, 1, 0.35f);
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4) + MountedPos, Projectile.velocity / 8f, RandLerpColor(Color.Violet, Color.White), 40, 1, 0.54f, .2f);
            ECSParticle.HighResolutionThunder(Projectile.Center, Projectile.SafeDirByRot().ToRandVelocity(ToRadians(1), .1f, .2f), RandLerpColor(Color.Violet, Color.Purple), 45, 1, Projectile.SafeDirByRot().ToRotation(), 0.12f, 1);
            for (int i = 0; i < 2; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center + Projectile.SafeDir() * i * 1.5f + MountedPos, 0.3f, RandLerpColor(Color.Violet, Color.Purple), 45, 1, 0.12f, glowMult: .5f);

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int maxHit = Monocle.ExecutionPenetrate;
            float ratios = Utils.GetLerpValue(0, maxHit, Projectile.numHits, true);
            float damageMult = Lerp(Monocle.ExecutionDamageMult, 0.75f, ratios);
            if (Projectile.numHits < 1)
                modifiers.SetCrit();
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 pos = Projectile.Center;
            Timer = 0;
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.Violet, Color.Purple), timeLeft, 1, scale);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 19.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.LightntingGlow(pos2, vel, RandLerpColor(Color.Purple, Color.Violet), timeLeft, 1, scale);
            }
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.HighResolutionThunder(pos.ToRandCirclePos(3), Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5), .1f, .2f), RandLerpColor(Color.Violet, Color.Purple), 45, 1, Projectile.SafeDirByRot().ToRotation(), 0.12f, 1);
            }
            ScarletSound(HJScarletSounds.Atom_StrikeAlt, Projectile.Center, 0.85f, 1, pitch: Projectile.numHits * .1f);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            return false;
        }
    }
}
