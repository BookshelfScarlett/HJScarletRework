using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TerraSpearArrow : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(30, 2);
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public enum Style
        {
            Shoot,
            HomingBack
        }

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ownerHitCheck = false;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            SpawnParticle();
            if (Projectile.damage == 0 && Projectile.penetrate == -1)
            {
                DisapperAI();
                return;
            }
            switch(AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.HomingBack:
                    DoHomingBack();
                    break;
            }
        }

        private void DisapperAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.01f);
            Projectile.scale = Lerp(Projectile.scale, 0f, 0.01f);
            if (Projectile.Opacity <= 0.02f)
                Projectile.Kill();
        }

        private void DoShoot()
        {
            AttackType = Style.HomingBack;
            Projectile.netUpdate = true;
            Timer *= 0;
        }
        public void SpawnParticle()
        {
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f);
                    new StarShape(pos, Projectile.SafeDir() * 4f, RandLerpColor(Color.Green, Color.LimeGreen), 0.4f * Projectile.Opacity, 10).Spawn();
                    new StarShape(pos, Projectile.SafeDir() * 4f, Color.White, 0.15f * Projectile.Opacity, 10).Spawn();
                }
            }
        }
        private void DoHomingBack()
        {
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 600f, canPassWall: true))
                Projectile.HomingTarget(target.Center, -1f, 16f, 20f, 10f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return false;
            //在前端直接绘制一个箭头类型的玩意
            SB.EnterShaderArea();
            DrawTrailBeam(Color.DarkGreen, 0.8f);
            DrawTrailBeam(Color.LightGreen, 0.4f);
            DrawTrailBeam(Color.White, 0.2f);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (Projectile.oldPos.Length > 12)
            {
                for (int k = 0; k < 12; k += 3)
                {
                    Vector2 dir = Projectile.oldPos[k] - Projectile.oldPos[k + 1];
                    DrawStar(Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, dir.ToRotation(), Color.Lerp(Color.Lime, Color.LimeGreen, (float)k / 16));
                }
            }
            SB.End();
            SB.BeginDefault();
            return false;
        }
        public void DrawStar(Vector2 drawPos, float rot, Color starColor)
        {
            Texture2D sharpTears = HJScarletTexture.Particle_HRStar.Value;
            Vector2 targetSize = 0.36f * Projectile.scale * new Vector2(0.8f, 0.25f);
            SB.Draw(sharpTears, drawPos, null, starColor * Projectile.Opacity, rot, sharpTears.Size() / 2, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, drawPos, null, Color.White with { A = 150 } * Projectile.Opacity, rot, sharpTears.Size() / 2, targetSize * 0.5f, SpriteEffects.None, 0);
        }
        public void DrawTrailBeam(Color color, float heightMul)
        {
            Texture2D trail = HJScarletTexture.Trail_ManaStreak.Value;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(50, trail.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 50);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.2f);
            shader.Parameters["uFadeinLength"].SetValue(0.25f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[0] = trail;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            Projectile.ClearInvaidData(out List<Vector2> validPos, out List<float> validRot);
            //直接获取需要的贝塞尔曲线。
            List<ScarletVertex> list = [];
            int totalpoints = validPos.Count;
            //创建顶点列表
            for (int i = 0; i < validPos.Count; i++)
            {
                Vector2 oldCenter = validPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPos.Count - 1);
                Vector2 posOffset = new Vector2(0, 14f * heightMul).RotatedBy(validRot[i]);
                ScarletVertex upClass = new(oldCenter - posOffset, color, new Vector3(progress, 0, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, color, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }
    }
}
