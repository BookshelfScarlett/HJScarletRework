using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class FloatingPlants : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Typeless;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public enum Plants
        {
            Daybloom,
            Moonglow,
            Blinkroot,
            Waterleaf,
            Deathweed,
            Shiverthorn,
            Fireblossom
        }
        public Plants PlantTypes = Plants.Daybloom;
        public static int[] PlantArrat =
            [
            ItemID.Daybloom,
            ItemID.Moonglow,
            ItemID.Blinkroot,
            ItemID.Waterleaf,
            ItemID.Deathweed,
            ItemID.Shiverthorn,
            ItemID.Fireblossom
            ];

        public ref float Osci => ref Projectile.ai[0];
        public int PlantType
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public enum State
        {
            Floating,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24, 2);
        }
        public override void ExSD()
        {
            //故意放大，因为这个是要人捡起来的
            Projectile.width = Projectile.height = 60;
            Projectile.scale = 0f;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void ProjAI()
        {
            DoHoming();
            UpdateParticle();
        }

        private void UpdateParticle()
        {
        
        }

        public void DoHoming()
        {
            Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
            Projectile.velocity *= 0.92f;
            Osci += ToRadians(2.5f);
            Vector2 floatingPosition = Projectile.Center + Vector2.UnitY * (int)(10f * Math.Sin(Osci));
            Projectile.Center = Vector2.Lerp(Projectile.Center, floatingPosition, 0.12f);
            float distance = (Projectile.Center - Owner.Center).LengthSquared();
            bool legalTarget = Owner.HasBuff<HerbBagBuff>(); 
            if (distance < 320f * 320f && legalTarget)
            {
                Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.Center - Owner.Center).ToRotation(), 0.2f);
                Projectile.HomingTarget(Owner.Center, 320f, 12f, 10f);
                if(Projectile.IntersectOwnerByDistance(30))
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);

                }
            }
        }

        public void DoFloating()
        {
        }

        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            int clamp = (int)Clamp(PlantType, 0, 6);
            Texture2D plant = GetVanillaAsset(VanillaAsset.Item, PlantArrat[clamp]);
            for (int i = 0; i < 8; i++)
                DrawPlants(plant, ToRadians(60f * i).ToRotationVector2() * 1.2f, Color.White.ToAddColor());
            DrawPlants(plant, Vector2.Zero, Color.White);
            return false;
        }
        public void DrawPlants(Texture2D plant, Vector2 posOffset,Color color)
        {
            SB.Draw(plant, Projectile.Center - Main.screenPosition + posOffset, null, color, Projectile.rotation, plant.ToOrigin(), Projectile.scale, 0, 0);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkSeaGreen, 1.48f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.LawnGreen, 1.26f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Lime, 0.88f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.54f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.StandardFlowShader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.StandardFlowShader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.StandardFlowShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.StandardFlowShader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.StandardFlowShader.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.StandardFlowShader.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.StandardFlowShader.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 8 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }


    }
}
