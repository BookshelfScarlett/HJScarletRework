using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Projs
{
    public abstract class ReOreKnifeProj : ReVisualProjClass
    {
        public override bool ApplyOriginalDraw => true;
        public float SpriteRotation = 0;
        public float Timer = 0;
        public float Spin = 0;
        public sealed override void RevisualUpdate(Projectile proj)
        {
            Timer++;

            if (Timer == 30f)
                Spin = -1;
            if (Spin == 0)
                SpriteRotation = proj.velocity.ToRotation();
            else
                SpriteRotation = proj.rotation;
            OreParticle(proj, proj.SafeDir());

        }
        public virtual void OreParticle(Projectile proj, Vector2 velDir) { }
        public sealed override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            ExPreDrawRevisual(proj, ref lightColor);
        }

        public virtual void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
        }
    }
    public class ReTinKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<TinKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.Tin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 1.1f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;

        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {

        }
    }
    public class ReCopperKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<CopperKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.CopperCoin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 1.1f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;
        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
        }
    }

    public class ReIronKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<IronKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.Iron, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 0.79f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;

            base.OreParticle(proj, velDir);
        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
        }
    }

    public class ReLeadKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<LeadKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.Lead, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;
        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
        }
    }

    public class ReTungstenKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<TungstenKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.SilverCoin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;

        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            for (int i = 0; i < 6; i++)
                SB.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), SpriteRotation + PiOver4 * (Spin == 0).ToInt(), proj.GetTexture().ToOrigin(), proj.scale, 0, 0);

        }
    }

    public class ReSilverKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<SilverKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.SilverCoin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 1.01f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;
        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            for (int i = 0; i < 6; i++)
                SB.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), SpriteRotation + PiOver4 * (Spin == 0).ToInt(), proj.GetTexture().ToOrigin(), proj.scale, 0, 0);
        }
    }

    public class ReGoldKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<GoldKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.GoldCoin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 1.05f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;

        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            for (int i = 0; i < 6; i++)
                SB.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), SpriteRotation + PiOver4 * (Spin == 0).ToInt(), proj.GetTexture().ToOrigin(), proj.scale, 0, 0);
        }
    }
    public class RePlatinumKnifeProj : ReOreKnifeProj
    {
        public override int ApplyProj => ProjectileType<PlatinumKnife>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOreknife;
        }
        public override void OreParticle(Projectile proj, Vector2 velDir)
        {
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(8f), DustID.PlatinumCoin, proj.velocity.ToRandVelocity(ToRadians(5f), 2.4f));
            d.scale *= 1.1f;
            d.velocity += proj.rotation.ToRotationVector2() * Main.rand.NextFloat(1.1f);
            d.noGravity = true;

        }
        public override void ExPreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            for (int i = 0; i < 6; i++)
                SB.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), SpriteRotation + PiOver4 * (Spin == 0).ToInt(), proj.GetTexture().ToOrigin(), proj.scale, 0, 0);
        }
    }
}
