using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace HJScarletRework.Projs.Executor
{
    public class PureDaggerStar : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public NPC CurTarget = null;
        public enum State
        {
            Shoot,
            Homing,
            Hit
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 800;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Projectile.localAI[1] = Main.rand.NextFloat(-1, 1);
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (AttackState == State.Shoot)
            {
                float maxTime = Projectile.MaxUpdates * 120;
                float pro = Utils.GetLerpValue(0, maxTime, Timer,true);
                Timer++;
                Projectile.velocity *= .951f;
                float rotValue = ToRadians(2f) * Projectile.localAI[1];
                Projectile.velocity = Projectile.velocity.RotatedBy(rotValue);
                if (Timer > Projectile.MaxUpdates * 120)
                {
                    Timer = Projectile.MaxUpdates * 120;
                }
                else
                {
                    if (Main.rand.NextBool(6) &&Main.rand.NextFloat() > pro)
                        ECSParticle.TurbulenceShinyOrb(Projectile.Center, 0.7f, Color.White, 40, 1, 0.124f * Projectile.scale);
                }
                    if (Main.rand.NextBool(6) &&Main.rand.NextFloat() < pro)
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), -Vector2.UnitY * Main.rand.NextFloat() * 4f, Color.White, 40, 1, 0.30f,0.2f);

            }
            else if (AttackState == State.Homing)
            {
                if(Main.rand.NextBool(9))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), -Vector2.UnitY * Main.rand.NextFloat() * 4f, Color.White, 40, 1, 0.30f,0.2f);
                if (CurTarget.IsLegal())
                {
                    float maxTime = Projectile.MaxUpdates * 30;
                    float pro = Utils.GetLerpValue(0, maxTime, Timer, true);
                    Timer++;
                    float speed = Lerp(0, 12, pro);
                    float angle = Lerp(0, 45, pro);
                    Projectile.HomingTarget(CurTarget.Center, -1, speed, 10,angle);
                }
                else
                {
                    AttackState = State.Hit;
                }
            }
            else
            {
                Projectile.velocity *= .14f;
                Projectile.scale = Lerp(Projectile.scale, 0, .12f);
                if (Projectile.scale <= .02f)
                {
                    for(int i =0;i<16;i++)
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), -Vector2.UnitY * Main.rand.NextFloat() * 4f, Color.White, 40, 1, 0.30f,0.2f);
                    Projectile.Kill();
                }
            }
            
        }
        public override bool? CanDamage()
        {
            return AttackState == State.Homing;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = CurTarget.IsLegal() && target.Equals(CurTarget) && AttackState == State.Homing;
            if (canHit)
                return null;
            return false;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(AttackState == State.Homing)
            {
                AttackState = State.Hit;
                Projectile.timeLeft = 150;
            }
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawCoreStar(spriteBatch);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkGray, 1.26f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Silver, 0.8f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f);
            HJScarletMethods.EndShaderAreaPixel();

        }
         public void DrawCoreStar(SpriteBatch sb)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i);
                Color drawColor = Color.Lerp(Color.DarkGray* colorAlpha, Color.White* colorAlpha, colorAlpha);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White* colorAlpha, Projectile.rotation, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White* colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
            }
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Clamp(Projectile.velocity.Length(),0,1));
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] == Vector2.Zero)
                    continue;
                float rot = Projectile.oldRot[j];
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 18 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(0.9f, 1.4f);
            Vector2 beginScale = new(0.1f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 1f;
        }


    }
}
