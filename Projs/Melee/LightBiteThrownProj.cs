using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<LightBite>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(15, 2);
        }
        public ref float Timer => ref Projectile.ai[0];
        public ref float SpawnStar => ref Projectile.ai[1];
        public float MaxTime = 10f;
        public float Ratios = 0f;
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 250;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.light = 0.5f;
            Timer++;
            SpawnStar += 1f;
            Ratios = Clamp( Timer / MaxTime, 0f, 1f);
            if (SpawnStar > 3f * Projectile.MaxUpdates)
            {
                SpawnStar = 0;
                Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f, 12f), ProjectileType<LightBiteDarkStar>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                star.localAI[0] = RandRotTwoPi;
                star.ai[2] = Main.rand.NextFloat(0.7f, 1.1f);
            }
            if (!HJScarletMethods.OutOffScreen(Projectile.Center))
                DrawParticle();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            Projectile.DrawGlowEdge(Color.Lerp(Color.Black, Color.Gold, Ratios), rotFix: ToRadians(135));
            Projectile.DrawProj(Color.Lerp(Color.White, Color.Black, Ratios), 1, 0.7f, ToRadians(135));
            return false;
        }

        private void DrawParticle()
        {
            Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(5f);
            if (Main.rand.NextBool())
            {
                new StarShape(Projectile.Center.ToRandCirclePosEdge(5f), Projectile.velocity / 3, RandLerpColor(Color.DarkGoldenrod, Color.OrangeRed),Main.rand.NextFloat(0.5f, 0.75f), 30).Spawn();
                new StarShape(spawnPos, Projectile.velocity / 3, Color.Black, Main.rand.NextFloat(0.5f, 0.75f), 30).SpawnToNonPreMult();
            }
            if (Main.rand.NextBool())
            {
                Vector2 dVel = Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f, 6f);
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(12f), dVel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, dVel.ToRotation(), 1f, 0.75f, Main.rand.NextFloat(ToRadians(-5f),ToRadians(5f))).Spawn();
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(8f), dVel * Main.rand.NextFloat(0.8f, 1.2f), RandLerpColor(Color.Orange, Color.OrangeRed), 40, 0.95f).Spawn();
            }
        }

        private void DrawTrail()
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float generalProgress = Ratios;
            generalProgress = Clamp(generalProgress, 0f, 1f);
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                float clampValue = (1 - rads) * generalProgress * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0, 1);
                Color drawColor = (Color.Lerp(Color.Black, Color.OrangeRed, rads * generalProgress).ToAddColor((byte)(200 * (1- rads)))) * 0.9f * clampValue;
                float scaleRatios = Clamp((1 - rads) * 1.4f, 0.80f, 1.20f);
                Vector2 drawScale = Projectile.scale * new Vector2(1f, 1.2f) * scaleRatios;
                float drawRot = Projectile.oldRot[i] - PiOver2;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.2f);
                Vector2 drawPos = lerpPos + Projectile.PosToCenter() + Projectile.SafeDirByRot().RotatedBy(PiOver2) * -2.5f + Projectile.SafeDirByRot() * 30f;
                SB.Draw(star, drawPos, null, drawColor, drawRot, star.Size() / 2, drawScale, 0, 0);
                drawColor = (Color.Lerp(Color.Orange, Color.OrangeRed, rads * generalProgress).ToAddColor()) * 0.9f * clampValue;
                SB.Draw(star, drawPos, null, drawColor, drawRot, star.Size() / 2, drawScale * 0.7f, 0, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //发射粒子
            for (float i = 0; i < 12f; i += 1f)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(12f), Projectile.oldVelocity * Main.rand.NextFloat(0.4f, 0.7f), RandLerpColor(Color.DarkGoldenrod, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.75f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(ToRadians(-5f), ToRadians(7f))).Spawn();
                new HRShinyOrb(Projectile.Center.ToRandCirclePos(8f), Projectile.oldVelocity.ToRandVelocity(1f, 3.2f), RandLerpColor(Color.DarkGoldenrod, Color.OrangeRed), 40, 0, 1, 0.12f).Spawn();
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (float i = 0; i < 12f; i += 1f)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(12f), Projectile.oldVelocity * Main.rand.NextFloat(0.4f, 0.7f), RandLerpColor(Color.DarkGoldenrod, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.75f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(ToRadians(-5f), ToRadians(7f))).Spawn();
                new HRShinyOrb(Projectile.Center.ToRandCirclePos(8f), Projectile.oldVelocity.ToRandVelocity(1f, 3.2f), RandLerpColor(Color.DarkGoldenrod, Color.OrangeRed), 40, 0, 1, 0.12f).Spawn();
            }

            //发射粒子
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
    }
}
