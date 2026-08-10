using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheGreatDipperGalaxy : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.timeLeft = GetSeconds(4);
            Projectile.width = Projectile.height = 600;
            Projectile.SetupImmnuity(8, ImmnuityType.Static);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override void ProjAI()
        {
            Timer++;
            float ratios = Utils.GetLerpValue(0, 30, Timer, true);
            Vector2 pos1 = Projectile.Center - new Vector2(155);
            Vector2 pos2 = Projectile.Center - new Vector2(-140, -20);
            Vector2 pos3 = Projectile.Center - new Vector2(0, -240);
            if (Main.rand.NextBool())
                ECSParticle.TurbulenceShinyOrb(pos1.ToRandCirclePos(80 * ratios), Main.rand.NextFloat(.4f, .9f), RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), Main.rand.Next(100, 140), 1f, Main.rand.NextFloat(.8f, 1.1f) * .1f, glowMult: .5f);
            if (Main.rand.NextBool())
                ECSParticle.TurbulenceShinyOrb(pos2.ToRandCirclePos(80 * ratios), Main.rand.NextFloat(.4f, .9f), RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), Main.rand.Next(100, 140), 1f, Main.rand.NextFloat(.8f, 1.1f) * .1f, glowMult: .5f);
            if (Main.rand.NextBool())
                ECSParticle.TurbulenceShinyOrb(pos3.ToRandCirclePos(80 * ratios), Main.rand.NextFloat(.4f, .9f), RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), Main.rand.Next(100, 140), 1f, Main.rand.NextFloat(.8f, 1.1f) * .1f, glowMult: .5f);
            ECSParticle.SnowCloud(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), RandDirTwoPi, RandLerpColor(Color.DeepSkyBlue, Color.LightSkyBlue), 120, RandRotTwoPi, .54f * ratios, 0.25f);
            for (int i = 0; i < 1; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                float scale = Main.rand.NextFloat(.13f, .16f);
                int lifeTime = Main.rand.Next(35, 46) * 2;
                ECSParticle.CrossGlow(pos, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), lifeTime, 1, scale * ratios, .2f);
                ECSParticle.CrossGlow(pos, Color.White, lifeTime, 1, scale * ratios * .65f, .2f);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                ECSParticle.ShinyCrossStarECS(pos, Vector2.UnitX, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, Main.rand.NextFloat(.2f, .40f) * ratios, .2f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SB.EnterShaderArea();
            FastDrawGlowCenter(new Vector2(155), .45f);
            FastDrawGlowCenter(new Vector2(-140, -20), .35f);
            FastDrawGlowCenter(new Vector2(0, -240), .30f);
            SB.EndShaderArea();

            return false;
        }
        public void FastDrawGlowCenter(Vector2 offset, float scaleMult = .85f)
        {
            Texture2D tex = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            SB.FastDraw(tex, drawPos - offset, Color.SkyBlue, 0, tex.Size() / 2f, .45f * scaleMult, 0);
            SB.FastDraw(tex, drawPos - offset, Color.White * .75f, 0, tex.Size() / 2f, .40f * scaleMult, 0);
        }
    }
}
