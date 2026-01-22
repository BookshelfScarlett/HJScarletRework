using ContinentOfJourney.Dusts;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<Tonbogiri>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting();
        public ref float Timer => ref Projectile.ai[0];
        public int TotalBubbles
        {
            get => (int)Projectile.HJScarlet().ExtraAI[0];
            set => Projectile.HJScarlet().ExtraAI[0] = value;
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 offset = Projectile.SafeDir() * 90f + Projectile.SafeDir().RotatedBy(PiOver2) * 2.1f;
            Vector2 mountedPos = Projectile.Center + offset;
            Dust d = Dust.NewDustPerfect(mountedPos + Main.rand.NextVector2Circular(6f, 6f), DustID.IceTorch);
            d.noGravity = true;
            new TurbulenceShinyOrb(mountedPos + Main.rand.NextVector2Circular(6f, 6f), 1.2f, RandLerpColor(Color.DeepSkyBlue, Color.Blue), 40, 0.1f, Projectile.SafeDir().ToRotation()).SpawnToPriority();

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnDust(Projectile.Center, Projectile.SafeDirByRot());
            SpawnBubbles();
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnDust(Projectile.Center, Projectile.SafeDirByRot());
            SpawnBubbles();
        }
        public void SpawnBubbles()
        {
            for (float i  = 0; i < TotalBubbles;i++)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 spawnPos = Projectile.Center;
                Vector2 velDir = dir.ToRandVelocity(ToRadians(60)) ;
                Projectile bubble = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, velDir*  Main.rand.NextFloat(12.2f,14.2f), ProjectileType<TonbogiriBubble>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);

            }
        }
        private static void SpawnDust(Vector2 spawnPos, Vector2 dir)
        {
            float numberOfDusts = 36f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(2.4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                new ShinyOrbParticle(spawnPos + offset, velOffset, RandLerpColor(Color.SkyBlue, Color.SlateBlue), 40, 0.8f).Spawn();
            }
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustID.BlueTorch);
                d.velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(2.2f, 4.2f);
                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                d.noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.Black, Color.DarkSlateBlue);
                new SmokeParticle(spawnPos, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            DrawTrail(drawPos);
            Projectile.DrawGlowEdge(Color.White, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White, rotFix:ToRadians(135));
            return false;
        }
        public void DrawTrail(Vector2 drawPosBase)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 offset = Projectile.SafeDir() * 45f;
            int length = 16;
            Vector2 trailScale = Projectile.scale * new Vector2(0.34f, 2f);
            Vector2 projDir = Projectile.SafeDirByRot();
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos.Length < 4)
                    continue;
                float rads = (float)i / length;
                Vector2 drawPos = drawPosBase + offset - projDir * 8f * i;
                Color drawColor = (Color.Lerp(Color.Blue, Color.DeepSkyBlue, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * Clamp(Projectile.velocity.Length(), 0 ,1);
                //中心高光的颜色
                Color drawColor2 = (Color.Lerp(Color.AliceBlue, Color.White, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * Clamp(Projectile.velocity.Length(), 0, 1);
                SB.Draw(star, drawPos, null, drawColor, Projectile.rotation - PiOver2, star.Size() / 2, trailScale, 0, 0);
                SB.Draw(star, drawPos, null, drawColor2, Projectile.rotation - PiOver2, star.Size() / 2, trailScale * 0.4f, 0, 0);
            }
        }
    }
}
