using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MoltenKnifeProj: HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<MoltenKnife>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.Orange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .38f, .12f);
            if (Main.rand.NextBool())
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.DarkOrange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .1f, .40f);
            if (Main.rand.NextBool())
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 8f, RandLerpColor(RandLerpColor(Color.OrangeRed, Color.Orange), Color.LightYellow), Main.rand.Next(60, 75), RandRotTwoPi, 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .25f, true, BlendState.Additive);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<MoltenKnifeBoom>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { MaxInstances = 0, Pitch = .35f }, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawProj(Vector2.Zero);
            return false;
        }
        public void DrawProj(Vector2 offset)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            drawPos -= offset;
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            float disapperRatios = 1;
            int length = (int)((Projectile.oldPos.Length));
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = EaseInOutExpo(1 - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter() - offset;
                float rot = Projectile.oldRot[i];
                Color c= Color.Lerp(Color.OrangeRed, Color.Lerp(Color.Orange, Color.White, 0.39f), ratios) * 0.70f;
                float scale = Projectile.scale * Lerp(.45f, 1f, ratios);
                SB.Draw(projTex, pos, null, c.ToAddColor(70), rot + PiOver4, orig, scale, 0, 0);
                Vector2 sharpScale = new Vector2(0.73f, 1.4f);
                SB.Draw(HJScarletTexture.Particle_SharpTear, pos, null, c.ToAddColor(), rot + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale*scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + (TwoPi / 8 * i).ToRotationVector2() * 1.5f, null, Color.Orange.ToAddColor(), Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            Color mainC = Color.Lerp(Color.White, Color.WhiteSmoke, disapperRatios);
            SB.Draw(projTex, drawPos, null, mainC, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
        }
    }
}
