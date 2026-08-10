using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class GrassKnifePoisonProj : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public bool InstantDoTDamage = false;
        public NPC CurTarget = null;
        public int StackLevel = 1;
        public override void ExSD()
        {
            Projectile.timeLeft = GetSeconds(10);
            Projectile.SetupImmnuity(12);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void ProjAI()
        {
            if (InstantDoTDamage)
            {
                Projectile.damage = 0;
                int damage = Projectile.originalDamage * StackLevel * (Projectile.timeLeft / 60);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<InvisBoom>(), damage, 0, Owner.whoAmI);
                Projectile.Kill();
                CombatText.NewText(Projectile.Hitbox, Color.LimeGreen, damage);
                ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, .84f, 0, -.24f, .1f);
                for (int i = 0; i < 6; i++)
                {
                    Color color = RandLerpColor(Color.Green, Color.White);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .13f + i * 0.2f, -1, Vector2.Zero, false).Spawn();
                }
                for (int i = 0; i < 50; i++)
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.Green, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
                return;
            }
            if (CurTarget.IsLegal() && CurTarget.HasBuff<GrassPoison>())
            {

                Projectile.Center = CurTarget.Center;
                if (StackLevel > 10)
                    StackLevel = 10;
                Projectile.damage = Projectile.originalDamage * StackLevel;
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (CurTarget.IsLegal() && target.Equals(CurTarget))
                return null;
            return false;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 finalDir = RandDirTwoPi;
            for (int i = 0; i < 12; i++)
            {
                Vector2 vel = finalDir.ToRandVelocity(ToRadians(35f), .1f, 19f);
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 10; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + finalDir * 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                Dust d = Dust.NewDustPerfect(target.Center, DustID.JungleTorch);
                d.velocity = RandVelTwoPi(1.2f, 6.2f) + finalDir * 3f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.2f, 1.61f);
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            if (CurTarget.IsLegal())
            {
                int index = CurTarget.FindBuffIndex(BuffType<GrassPoison>());
                if (index != -1)
                    CurTarget.DelBuff(index);
            }
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
