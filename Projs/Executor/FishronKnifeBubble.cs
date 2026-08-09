using ContinentOfJourney.Items.Pylons;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FishronKnifeBubble : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.FlaironBubble);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7);
        }
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
        }
        public override void ProjAI()
        {
            Projectile.rotation += .2f * Projectile.GetHorizonDirection();
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 15f))
            {
                if (Projectile.GetTargetSafe(out NPC target))
                    Projectile.HomingTarget(target.Center, -1, 12f, 10, 10);
                else
                    Projectile.velocity *= 0.9f;
            }
            else
                Projectile.velocity *= 0.98f;
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1, 0.34f * .42f, .031f);
            if (Projectile.velocity.LengthSquared() > Main.rand.NextFloat(5 * 5, 9 * 9) && Main.rand.NextBool(4))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1f, .32f);
        }
        public override bool? CanDamage()
        {
            return Timer > Projectile.MaxUpdates * 15f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScarletSound(SoundID.Item54, Projectile.Center,instances:0);
            ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(3), Vector2.Zero, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 20, 1, 0.68f, .021f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            tex = GetVanillaAsset(VanillaAsset.Projectile, ProjectileID.FlaironBubble);
            int length = Projectile.oldPos.Length;
            float generalScale = .84f;
            SB.EnterShaderArea();
            SB.FastDraw(HJScarletTexture.Particle_CrossGlow.Value, drawPosition, Color.SkyBlue, 0, HJScarletTexture.Particle_CrossGlow.Origin, Projectile.scale * generalScale * .14f, 0);
            SB.EndShaderArea();
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                Color c = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, ratios);
                Vector2 sharpScale = new Vector2(0.83f, 1.4f);
                c = Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, ratios).ToAddColor(250);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * generalScale * scale, se);

            }
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale * generalScale, se);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -40.2f * offsetHeight);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)((Projectile.oldPos.Length-6) * Clamp(Projectile.velocity.Length(), 0, 1));
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] == Vector2.Zero)
                    continue;
                float rot = Projectile.oldRot[j];
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2 +Projectile.SafeDir() * 10f, drawColor, new Vector2(0, 13 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

    }
}
