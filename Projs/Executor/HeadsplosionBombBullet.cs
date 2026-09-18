using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.General;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class HeadsplosionBombBullet : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = 3;
            Projectile.SetupImmnuity(-1);
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Projectile.MaxUpdates * GetSeconds(5);
            Projectile.tileCollide = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.AffactedByGrav(velMult: .985f, yMult: 1.0f, yAdd: 0.13f, maxGravSpeed: 45);
            if (Main.rand.NextBool(8))
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(8), 0.6f, RandLerpColor(Color.Goldenrod, Color.DarkGoldenrod), 45, 1, 0.11f*Main.rand.NextFloat(.9f,1.1f),glowMult:.45f);
            if (Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), 45, 1, Main.rand.NextFloat(.9f, 1.1f) * .3f);
            Timer++;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 pos = Projectile.Center;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<HeadsplosionBoom>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            if (Projectile.HJScarlet().ExecutionStrikeManual)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (-Vector2.UnitY).ToRandVelocity(ToRadians(30), 8f, 17f), ProjectileType<HeadsplosionBombBullet>(), Projectile.damage / 2, Projectile.knockBack, Owner.whoAmI);
                }
            }
            for (int i = 0; i < 18; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 1.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.248f;
                int time = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, RandVelTwoPi(0, 8), RandLerpColor(Color.Goldenrod, Color.DarkGoldenrod), time, 1, scale);
            }
            for (int i = 0; i < 18; i++)
            {
                ECSParticle.HRShinyOrb(pos, RandVelTwoPi(0, 8), RandLerpColor(Color.Goldenrod, Color.DarkGoldenrod), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .10f, .4f);
            }
            float crossGlowScale = .14f;
            ECSParticle.CrossGlow(pos, Color.Gold, 40, 1, crossGlowScale, .2f);
            ECSParticle.CrossGlow(pos, Color.LightGoldenrodYellow, 40, 1, crossGlowScale * .98f, .2f);
            ECSParticle.CrossGlow(pos, Color.White, 40, 1, crossGlowScale * .95f, .2f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, GetSeconds(2));
        }
        public override bool? CanDamage()
        {
            return Timer > Projectile.MaxUpdates * 5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length / 4;
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkGoldenrod, 1.2f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White, 1.0f);

            DeepGlow.SubmitCustomGlow(() =>
            {
                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Goldenrod, 1.2f);
            });
            SB.EndShaderArea();
            SB.FastDraw(tex, drawPosition, Color.Yellow, drawRotation, tex.Size() / 2f, Projectile.scale, se);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();
            TrailDrawer(useTex, drawColor, multipleSize, alphaValue, offsetHeight);
                }
        public void TrailDrawer(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (Projectile.oldPos.Length < 3)
                return;

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 5) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 30 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);

        }
    }
}
