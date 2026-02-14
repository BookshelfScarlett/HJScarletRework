using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FlybackHandStar : HJScarletFriendlyProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.FallenStar);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
            Main.projFrames[Type] = 8;
        }
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public ref float AddAngle => ref Projectile.localAI[0];
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            AddFrame();
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.GetTargetSafe(out NPC target, true, 600) && Timer > 30f)
            {
                Projectile.HomingTarget(target.Center, 600, 21f, 45f, 20);
            }
            else if(Timer < 30)
            {
                new Fire(Projectile.Center + Main.rand.NextVector2Circular(6f, 6f), Projectile.velocity / 3, RandLerpColor(Color.Gold, Color.Yellow), 60, Projectile.rotation, 1f, 0.1f).Spawn();
                Projectile.velocity *= 0.93f;
            }
            //闪亮的星辰啊……
        }
        public bool GrowUp = false;
        public void AddFrame()
        {
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 8)
            {
                GrowUp = false;
                Projectile.frame = 0;

            }
        }
        public override bool? CanDamage()
        {
            return Timer > 30;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreKill(int timeLeft)
        {
            for (int i = 0; i <16;i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 16, 16, DustID.IchorTorch, 0, 0);
                d.noGravity = true;
            }
                return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Tex2DWithPath tex = HJScarletTexture.Particle_OpticalLineGlow;
            Rectangle frames = Projectile.GetTexture().Frame(1, 8, 0, Projectile.frame);
            Vector2 origin = frames.Size() / 2;
            SB.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, frames, Color.White, Projectile.rotation, origin, Projectile.scale, 0, 0);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //这里最好还是用顶点画
            DrawNebulaTrail(Color.Gold, 14f);
            DrawNebulaTrail(Color.LightYellow with { A = 50 }, 12.2f);
            DrawNebulaTrail(Color.White with { A = 100 }, 10.8f);
            SB.EndShaderArea();
            
            return false;
        }
        public void DrawNebulaTrail(Color trailColor, float height)
        {
            float laserLength = 15;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_TerraRayFlow.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_TerraRayFlow.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -10);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4());
            shader.Parameters["uFadeoutLength"].SetValue(0.5f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = HJScarletTexture.Trail_TerraRayFlow.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                Vector2 posOffset = new Vector2(0,  3f * height).RotatedBy(validRot[i]);
                ScarletVertex upClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 1, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 0, 0f));
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
