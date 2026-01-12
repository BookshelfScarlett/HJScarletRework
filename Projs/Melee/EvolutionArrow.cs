using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.Specialized;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class EvolutionArrow : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(20, 2);
        public enum Style
        {
            Attack,
            Fade
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer += 0.025f;

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f);
                    new StarShape(pos, Projectile.SafeDir() * 4f, Color.Green.RandLerpTo(Color.LimeGreen), 0.4f * Projectile.Opacity, 10).Spawn();
                    new StarShape(pos, Projectile.SafeDir() * 4f, Color.White, 0.15f * Projectile.Opacity, 10).Spawn();
                }
            }
            //在这个过程生成需要的粒子
            if (AttackType == Style.Attack)
            {
                
                if (Projectile.GetTargetSafe(out NPC target, true, 1200f))
                {
                    Projectile.extraUpdates = 5;
                    Projectile.HomingTarget(target.Center, -1f, 20f, 35f);
                }
                else
                    Projectile.extraUpdates = 4;
            }
            else
            {
                Projectile.velocity *= 0.98f;
                Projectile.Opacity -= 0.025f;
                if (Projectile.Opacity == 0)
                    Projectile.Kill();
            }
            
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == Style.Attack)
            {
                //如果有传入的目标，或者说压根没目标，我们才正常切换状态
                if (Projectile.HJScarlet().GlobalTargetIndex == -1 || Projectile.HJScarlet().GlobalTargetIndex == target.whoAmI)
                {
                    AttackType = Style.Fade;
                    Projectile.netUpdate = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return false;
            SB.EnterShaderArea();
            DrawTrailBeam(Color.DarkGreen, 0.8f);
            DrawTrailBeam(Color.LightGreen, 0.4f);
            DrawTrailBeam(Color.White, 0.2f);
            SB.EndShaderArea();
            return false;
        }
        public void DrawStar(Vector2 drawPos, float rot, Color starColor)
        {
            Texture2D sharpTears = HJScarletTexture.Particle_HRStar.Value;
            Vector2 targetSize = 0.36f * Projectile.scale * new Vector2(1.2f, 0.25f);
            SB.Draw(sharpTears, drawPos, null, starColor, rot, sharpTears.Size() / 2, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, drawPos, null, Color.White with { A = 150 }, rot, sharpTears.Size() / 2, targetSize * 0.5f, SpriteEffects.None, 0);
        }
        public void DrawTrailBeam(Color color, float heightMul)
        {
            Texture2D trail = HJScarletTexture.Trail_TerraRayFlow.Value;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(50, trail.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 50);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.2f);
            shader.Parameters["uFadeinLength"].SetValue(0.2f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[0] = trail;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            Projectile.ClearInvaidData(out List<Vector2> validPos, out List<float> validRot);
            //直接获取需要的贝塞尔曲线。
            List<VertexPositionColorTexture2D> list = [];
            int totalpoints = validPos.Count;
            //创建顶点列表
            for (int i = 0; i < validPos.Count; i++)
            {
                Vector2 oldCenter = validPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPos.Count - 1);
                Vector2 posOffset = new Vector2(0, 40f * heightMul).RotatedBy(validRot[i]);
                VertexPositionColorTexture2D upClass = new(oldCenter - posOffset, color, new Vector3(progress, 0, 0f));
                VertexPositionColorTexture2D downClass = new(oldCenter + posOffset, color, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
            /*
            DrawSetting drawSetting = new DrawSetting(trail, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPos.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPos[j + 1] - validPos[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2);
                trailDrawDates.Add(new(validPos[j] + Projectile.Size / 2 + posOffset, color, new Vector2(0, 33f * heightMul), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
            */
        }
    }
}
