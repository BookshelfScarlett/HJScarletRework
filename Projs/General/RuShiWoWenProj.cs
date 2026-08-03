using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class RuShiWoWenProj :HJScarletProj,IPixelatedRenderer
    {
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.Opacity = 0;
        }
        public ref float Timer => ref Projectile.ai[0];
        public float ChargeRingShown = 0;
        public ref float ChargeRing => ref Projectile.localAI[0];
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(0, 0);
        private Vector2 TopRightPoint = new Vector2(30, -100);
        private Vector2 BottomLeftPoint = new Vector2(30, 100);
        private Vector2 BottomRightPoint = new Vector2(0, 0);
        private Vector4 RandValueSummary = new Vector4();

        public override void OnFirstFrame()
        {
            RandValueSummary = new Vector4(Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(0.7f, 1.1f));
            float maxPoints = 60;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= 0.9f;
                finalPos.Y *= 0.16f;
                CenterPosList.Add(finalPos.RotatedBy(PiOver2));
            }
            //对的没错，这是他妈的一个粒子。
            new RuShiWoWenFlower(Projectile.whoAmI, Owner.MountedCenter + Vector2.UnitY * 20f).SpawnToPriority();
        }

        public override void ProjAI()
        {
            if (Owner.HJScarlet().powerLilyVanity)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.1f);
                Projectile.timeLeft = 10;
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0f);
            }
            Vector2 mountedPos = Owner.MountedCenter - Vector2.UnitY * 20f + Vector2.UnitX * Owner.direction * -25f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.42f);
            if (Main.rand.NextBool(5))
            {
                Vector2 flowerPos = Owner.MountedCenter + Vector2.UnitY * 20f;
                ECSParticle.ShinyCrossStarECS(flowerPos.ToRandCirclePos(25, 10), -Vector2.UnitY * Main.rand.NextFloat(.1f, 1.1f) * 3f, RandLerpColor(Color.LightPink, Color.White), 40, 1, 0.3f);
            }

            //这里控制一遍圆环的效果
            Timer = Utils.GetLerpValue(GetSeconds(30) + 1, 1, Owner.HJScarlet().powerLilyTimer, true);
            ChargeRing = Lerp(ChargeRingShown,Timer, 0.51f);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;

            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.ai[1] = Owner.MountedCenter.X;
            Projectile.ai[2] = Owner.MountedCenter.Y + 20f + Owner.gfxOffY;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            SpriteEffects se = Owner.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = Projectile.GetTexture();
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + (TwoPi / 8 * i).ToRotationVector2() * 2f*Projectile.Opacity * ChargeRing, null, Color.White.ToAddColor(0), 0, tex.Size() / 2, Projectile.scale*Projectile.Opacity, se, 0);
            Color mainColor = Color.Lerp(Color.White, Color.Transparent, ChargeRing);
            SB.Draw(tex, pos, null, mainColor*Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale, se, 0);
            return false;
        }
        //复制粘贴
         public BlendState BlendState => BlendState.Additive;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Texture_StandardGradient.Value;
            Effect e = HJScarletShader.AlphaFade;
            e.Parameters["uFadeoutLeftLength"].SetValue(0.3f);
            e.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            e.Parameters["uFadeinTopLength"].SetValue(0.3f);
            e.Parameters["uFadeinBottomLength"].SetValue(0.5f);
            e.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            e.CurrentTechnique.Passes[0].Apply();
            TrailFunc(tex, Color.Gold* 0.60f, 9f);
            TrailFunc(tex, Color.Yellow* 0.20f, 6f);
            TrailFunc(tex, Color.LightGoldenrodYellow* .20f, 3f);
            TrailFunc(tex, Color.White * .20f, 2f);

            Vector4 vector4 = new(0.2f, 0.2f, 0.1f, 0.6f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 1.79f * RandValueSummary.X), new Vector2(1f, 0.985f), Color.White);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            TrailFunc(texture2, Color.LightGoldenrodYellow, 10f);

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 2.79f * RandValueSummary.Y), new Vector2(2f, 0.975f), Color.White);
            texture2 = HJScarletTexture.Noise_Misc.Value;
            TrailFunc(texture2, Color.White * 0.42f, 10f);
            texture2 = HJScarletTexture.Noise_Aura.Value;

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.79f * RandValueSummary.Z), new Vector2(3.2f, 1.94f), Color.White);
            TrailFunc(texture2, Color.White* 0.92f, 20f);
            TrailFunc(texture2, Color.White * 0.92f, 10f);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = 150;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 1.92f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
        public void TrailFunc(Texture2D tex, Color c, float mult)
        {
            List<ScarletVertex> vertices = new List<ScarletVertex>();
            Vector2 dir = (-Vector2.UnitY) * 12f * mult;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 pos = Projectile.Center - Main.screenPosition - Vector2.UnitY * 5 - Vector2.UnitX;
                Vector2 posHead = CenterPosList[i] + pos;
                Vector2 posSrc = CenterPosList[i] + pos + dir * Projectile.Opacity;
                vertices.Add(new ScarletVertex(posHead, c, new Vector3(progress, 0, 0)));
                vertices.Add(new ScarletVertex(posSrc, c, new Vector3(progress, 1, 0)));
            }
            if (vertices.Count < 3)
                return;
            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
    }
}
