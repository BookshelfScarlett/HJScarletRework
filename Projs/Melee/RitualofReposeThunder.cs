using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class RitualofReposeThunder : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.penetrate = 8;
            Projectile.SetupImmnuity(15);
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Projectile.penetrate * 15 * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;

        }
        public override void ProjAI()
        {
            if (Projectile.HJScarlet().CurStoredTarget.IsLegal())
                Projectile.Center = Projectile.HJScarlet().CurStoredTarget.Center;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
            {
                if (target.IsLegal())
                    Projectile.HJScarlet().CurStoredTarget = target;
                int time = Main.rand.Next(30, 90);
                new ThunderboltParticle(Projectile.Center, 0, 1.15f, Color.Gold, time, 15f, .75f, new Vector2(0.5f, 1.25f)).Spawn();
                new ThunderboltParticle(Projectile.Center, 0, 1.05f, Color.LightGoldenrodYellow, time, 15f, .75f, new Vector2(.5f, 1.25f)).Spawn();
                Projectile.timeLeft = GetSeconds(8) * Projectile.MaxUpdates;
                Vector2 pos = Projectile.Center;
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePosEdge(30), 4.5f, RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), Main.rand.Next(60, 70), 1, 0.18f, TwoPi / 36f * i, glowMult: .46f);
                }
                for (int i = 0; i < 8; i++)
                {
                    ECSParticle.ShinyCrossStarSmall(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 28f), RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * 1.1f, 0);
                }
                float glowScale = .3f;
                ECSParticle.CrossGlow(pos, Color.Gold, 45, 1, glowScale, .3f);
                ECSParticle.CrossGlow(pos, Color.Goldenrod, 45, 1, glowScale * .95f, .3f);
                ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);
                ECSParticle.Ring(pos, Vector2.Zero, Color.DarkGoldenrod * .8f, 45, 1, 0, .15f, .12f);
                for (int i = 0; i < 8; i++)
                {
                    ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(50), Vector2.Zero, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), 45, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .3f, 2);
                }
                for (int i = -1; i < 2; i += 1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(16).ToSafeNormalize() * 12f, ProjectileType<RitualofReposeStar>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                    if (target.IsLegal())
                        proj.HJScarlet().CurStoredTarget = target;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
