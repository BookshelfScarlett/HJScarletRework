using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReAccelerationismProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<AcceleratingArrow>();
        public override int TotalListCount => 20;
        public float DrawScale = 1f;
        public List<float> RotList2 = [];
        public List<Vector2> PosList2 = [];

        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualAccelerationism;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddExtraList(proj);
        }
        public void AddExtraList(Projectile proj)
        {
            if (!proj.HJScarlet().FirstFrame)
            {
                RotList2.Clear();
                PosList2.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    RotList2.Add(0);
                    PosList2.Add(Vector2.Zero);
                }
            }
            RotList2.Add(proj.velocity.ToRotation());
            PosList2.Add(proj.Center);
            if (PosList2.Count > TotalListCount + 2)
                PosList2.RemoveAt(0);
            if (RotList2.Count > TotalListCount + 2)
                RotList2.RemoveAt(0);
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(GetInstance<AcceleratingArrow>().Texture).Value;
            SB.Draw(tex, proj.Center - Main.screenPosition, null, Color.White * proj.Opacity, proj.rotation, tex.ToOrigin(), proj.scale, 0, 0);
            if (PosList2.Count < 1 && RotList2.Count < 1)
                return;
            DrawGlow(proj,proj.Center - Main.screenPosition);
            DrawTheTrail(proj,Color.CornflowerBlue, 18f);
            DrawTheTrail(proj,Color.RoyalBlue with { A = 50}, 16.2f);
            DrawTheTrail(proj, Color.White with { A = 100}, 12.8f);


        }
        private void DrawGlow(Projectile proj,Vector2 drawPos)
        {
            SB.EnterShaderArea();
            Vector2 dir = proj.SafeDirByRot();
            Tex2DWithPath lineGlow = HJScarletTexture.Particle_OpticalLineGlow;
            Vector2 glowScale = proj.scale * new Vector2(1.2f, 0.7f);
            SB.Draw(lineGlow.Value, drawPos + dir * 10f, null, Color.CornflowerBlue * proj.Opacity, proj.rotation, lineGlow.Origin, glowScale * 0.10f, 0, 0);
            SB.EndShaderArea();
        }
        public void DrawTheTrail(Projectile proj,Color trailColor, float height)
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
            proj.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, [.. PosList2], [.. RotList2]);
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
                Vector2 oldCenter = validPosition[i] + proj.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                float ratio = Clamp(((float)(totalpoints - i) / totalpoints), 0.5f, 1f);
                Vector2 posOffset = new Vector2(0, height * DrawScale * ratio).RotatedBy(validRot[i]);
                ScarletVertex upClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 0, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            SB.EndShaderArea();
        }

    }
}
