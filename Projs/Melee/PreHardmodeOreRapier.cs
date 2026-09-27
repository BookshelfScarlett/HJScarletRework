using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public abstract class PreHardmodeOreRapierProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public virtual int TrailLength => 0;
        public override void SetStaticDefaults()
        {
            if (TrailLength != 0)
                Projectile.ToTrailSetting(TrailLength);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.SetupImmnuity(-1);
        }
        public ref float Timer => ref Projectile.ai[0];
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > Projectile.MaxUpdates * 24f)
                Projectile.AffactedByGrav(.98f, yMult: 1.03f, yAdd: .2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rot = Projectile.rotation + PiOver4;
            ExPreDraw(projTex, drawPos, ori, rot, ref lightColor);
            return false;
        }
        protected virtual void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {

        }
    }
    public class PlatinumRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<PlatinumRapierThrown>().Texture;
        public override int TrailLength => 4;
        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 2;
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.PlatinumCoin);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.PlatinumCoin);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Lime, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }
    public class GoldRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<GoldRapierThrown>().Texture;
        public override int TrailLength => 4;
        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 2;

        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.GoldCoin);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.GoldCoin);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }

            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.LightGoldenrodYellow, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.Gold, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);

        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Gold, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.LightGoldenrodYellow, Color.Gold, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
        }
    }
    public class TungstenRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<TungstenRapierThrown>().Texture;
        public override int TrailLength => 4;
        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 1;
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Tungsten);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Tungsten);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.DarkOliveGreen, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.DarkOliveGreen, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.DarkOliveGreen, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Olive, Color.DarkGreen, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }

    public class SilverRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<SilverRapierThrown>().Texture;

        public override int TrailLength => 4;

        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 1;
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.SilverCoin);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.SilverCoin);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Silver, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Silver, Color.Silver, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }
    public class IronRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<IronRapierThrown>().Texture;
        public override int TrailLength => 4;
        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 2;
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Iron);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Iron);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.SaddleBrown, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Brown, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.SaddleBrown, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }

    public class LeadRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<LeadRapierThrown>().Texture;
        public override int TrailLength => 4;

        public override void ExSD()
        {
            base.ExSD();
            Projectile.penetrate = 2;
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Lead);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Lead);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.MidnightBlue, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.DarkBlue, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.MidnightBlue, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.DarkSlateBlue, Color.DarkBlue, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }
    public class CopperRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<CopperRapierThrown>().Texture;
        public override int TrailLength => 4;

        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.CopperCoin);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.CopperCoin);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.Brown, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.SaddleBrown, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.DarkOrange, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Orange, Color.Brown, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }
    public class TinRapierThrownProj : PreHardmodeOreRapierProj
    {
        public override string Texture => GetInstance<TinRapierThrown>().Texture;
        public override int TrailLength => 4;

        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnKill(int timeLeft)
        {
            ScarletSound(SoundID.Dig, Projectile.Center);
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Tin);
                d.velocity = (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 6f);
                d.scale = .8f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6), DustID.Tin);
                d.velocity = RandVelTwoPi(8f);
                d.scale = .8f;
                d.noGravity = true;
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(6))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 3f, Color.White, 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .41f);
        }
        protected override void ExPreDraw(Texture2D tex, Vector2 drawPos, Vector2 ori, float rot, ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, 1f, (progress));
                float yMult = Lerp(1f, 1f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .19f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Lime, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.5f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);

            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            base.ExPreDraw(tex, drawPos, ori, rot, ref lightColor);
        }
    }
}
