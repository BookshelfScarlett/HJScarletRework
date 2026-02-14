using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class GalvanizedHandArrow : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<AcceleratingArrow>().Texture;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Attack,
            Decay
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1]; 
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(18, 2);
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public float DrawScale = 1f;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            if (AttackType == Style.Attack)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(5f), DustID.UnusedWhiteBluePurple, Projectile.SafeDirByRot() * 3f);
                d.scale *= 1.3f;
                d.noGravity = true;
                Timer++;
                if (Timer > 8 * Projectile.extraUpdates)
                {
                    if (Projectile.GetTargetSafe(out NPC target, false, canPassWall: true))
                    {
                        Projectile.HomingTarget(target.Center, -1f, 18f, 30f, 10f);
                    }
                    else
                    {
                        AttackType = Style.Decay;
                        Projectile.netUpdate = true;
                        Timer *= 0;
                    }
                }
            }
            else
            {
                Projectile.damage *= 0;
                DrawScale -= 0.01f;
                Projectile.Opacity -= 0.03f;
                Projectile.velocity *= 0.94f;
                if (Projectile.Opacity <= 0f)
                    Projectile.Kill();
            }
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override bool? CanDamage()
        {
            return Timer > 5 * Projectile.extraUpdates;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (AttackType == Style.Attack)
            {
                AttackType = Style.Decay;
                Projectile.netUpdate = true;
                Timer *= 0;
            }
        }
        //特效的素材复用。
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White * Projectile.Opacity,rotFix:PiOver2);

            DrawGlow(Projectile.Center - Main.screenPosition);
            DrawTrail(Color.CornflowerBlue, 18f);
            DrawTrail(Color.RoyalBlue with { A = 50}, 16.2f);
            DrawTrail(Color.White with { A = 100}, 12.8f);
            return false;
        }
        private void DrawGlow(Vector2 drawPos)
        {
            SB.EnterShaderArea();
            Vector2 dir = Projectile.SafeDirByRot();
            Vector2 glowCirclePos = drawPos + dir * 10f;
            //SB.Draw(HJScarletTexture.Texture_Swirl5.Value, glowCirclePos, null, Color.CornflowerBlue, Projectile.rotation, HJScarletTexture.Texture_Swirl5.Origin, Projectile.scale * 0.5f, 0, 0);
            Tex2DWithPath lineGlow = HJScarletTexture.Particle_OpticalLineGlow;
            Vector2 glowScale = Projectile.scale * new Vector2(1.2f, 0.7f);
            SB.Draw(lineGlow.Value, drawPos + dir * 10f, null, Color.CornflowerBlue * Projectile.Opacity, Projectile.rotation, lineGlow.Origin, glowScale * 0.10f, 0, 0);
            SB.EndShaderArea();
        }
        public void DrawTrail(Color trailColor, float height)
        {
            SB.EnterShaderArea();
            float laserLength = 50;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4() * DrawScale);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = HJScarletTexture.Trail_ManaStreak.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                if (validPosition[i].Equals(Vector2.Zero))
                    continue;
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                float ratio = Clamp(((float)(totalpoints - i) / totalpoints), 0.5f, 1f);
                Vector2 posOffset = new Vector2(0, height * DrawScale * ratio).RotatedBy(validRot[i]);
                ScarletVertex upClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 0, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
            SB.EndShaderArea();
        }
    }
}
