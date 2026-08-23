using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Steamworks;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class RitualofReposeProj : HJScarletProj, IPixelatedRenderer
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override string Texture => GetInstance<RitualofRepose>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 3;
            Projectile.SetupImmnuity(30);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(9))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(28), Projectile.velocity / 8f, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .42f, 0);
            if (Main.rand.NextBool(7))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .6f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rotFixer = PiOver4;
            float spearRotation = Projectile.rotation + rotFixer;
            SB.FastDraw(projTex, drawPos, Color.White.ToAddColor(200), spearRotation, ori, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }
        public BlendState BlendState => BlendState.Additive;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length;
            Texture2D trailTex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle curSrc = trailTex.Bounds;
            curSrc.Width = (int)(curSrc.Width * 0.8f);
            Vector2 cutSrcOri = new Vector2(curSrc.Width / 2f, curSrc.Height / 2f);
            Vector2 sharpScale = new Vector2(0.7f, 0.60f);

            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1f - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color trailColor = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, ratios).ToAddColor(100);
                float scale = Lerp(.44f, 1f, ratios);
                float opacity = Lerp(.51f, .7f, ratios);
                Vector2 sharpPos = pos - new Vector2(20, 0).RotatedBy(Projectile.oldRot[i]);
                float trailRot = Projectile.oldRot[i];

                SB.Draw(trailTex, sharpPos, curSrc, trailColor * opacity, trailRot, cutSrcOri, sharpScale * scale, 0, 0);
            }
            //十字光晕
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 glowPos = drawPos + Projectile.rotation.ToRotationVector2() * 25f * Projectile.scale;
            Vector2 glowScale = Projectile.scale * .48f * new Vector2(.75f, 1.35f);

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .65f, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .35f, Projectile.rotation, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .45f, Projectile.rotation, glow.Size() / 2f, glowScale * .95f, 0);

            //环状光晕
            glow = HJScarletTexture.Particle_RingShiny.Value;
            glowScale = Projectile.scale * .16f * Vector2.One;

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .45f, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .4f, Projectile.rotation + MathHelper.Pi, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .5f, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2f, glowScale * .95f, 0);
            HJScarletMethods.EndShaderAreaPixel();
        }
    }
}