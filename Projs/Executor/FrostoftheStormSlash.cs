using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Items.Weapons.Requirement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class FrostoftheStormSlash : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(-300, 0);
        private Vector2 TopRightPoint = new Vector2(50, -100);
        private Vector2 BottomLeftPoint = new Vector2(50, 100);
        private Vector2 BottomRightPoint = new Vector2(-300, 0);
        public float RandOffset1;
        public float RandOffset2;
        public float RandOffset3;
        public float RandOffset4;
        public bool PostFirstFrame = false;
        public override void ExSD()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 120;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 1;
            Projectile.Opacity = 0f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Intersects(projHitbox))
            {
                for (int i = 0; i < CenterPosList.Count; i++)
                {
                    Rectangle ProjHitbox = Utils.CenteredRectangle(CenterPosList[i] + Projectile.Center - Projectile.velocity, new Vector2(50, 50));
                    if (targetHitbox.Intersects(ProjHitbox))
                        return true;
                }
            }
            return false;
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            SetFirstFrame();
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.12f);
            if (Projectile.IsOutScreen())
                return;
            for (int i = 0; i < 4; i++)
            {
                int index = Main.rand.Next(10, CenterPosList.Count - 10);
                Vector2 pos = CenterPosList[index] + Projectile.Center - Projectile.SafeDir() * 30f;
                new SnowCloud(pos, Projectile.velocity * Main.rand.NextFloat(.4f), RandLerpColor(Color.LightSkyBlue, Color.Gray), 45, Main.rand.NextFloat(TwoPi), 0.45f, Main.rand.NextFloat(.9f, 1.3f) * 0.270f, Main.rand.NextBool()).SpawnToPriority();
            }
            for (int i = 0; i < 2; i++)
            {
                int index = Main.rand.Next(3, CenterPosList.Count - 3);
                Vector2 pos = CenterPosList[index] + Projectile.Center;
                float scale = Main.rand.NextFloat(.52f, .82f) * .2f;
                int lifeTime = Main.rand.Next(30, 60);
                new HRShinyOrb(pos, Projectile.velocity * 0.8f, RandLerpColor(Color.RoyalBlue, Color.LightBlue), lifeTime, scale).Spawn();
                new HRShinyOrb(pos, Projectile.velocity * 0.8f, Color.White, lifeTime, scale * .65f).Spawn();
            }
            for (int i = 0; i < 3; i++)
            {
                int index = Main.rand.Next(3, CenterPosList.Count - 3);
                Vector2 pos = CenterPosList[index] + Projectile.Center;
                float scale = Main.rand.NextFloat(.4f, .8f) * .51f;
                new ShinyCrossStar(pos, Projectile.velocity * 0.5f, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, 0, 1, 1.180f, false).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustCount = 16;
            Projectile.AddExecutionTimeImmediate(ItemType<FrostoftheStorm>());
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = target.Center.ToRandCirclePos(10f)+ dir * Main.rand.NextFloat(10f);
                new ShinyCrossStar(pos, RandVelTwoPi(2f, 9f), RandLerpColor(Color.LightSkyBlue, Color.RoyalBlue), 45, RandRotTwoPi, RandZeroToOne, Projectile.scale, false, 0.5f).Spawn();
            }
            for (int i = 0; i < 30; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.2f, 17.4f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .2f;
                new HRShinyOrb(pos, vel, RandLerpColor(Color.LightBlue, Color.RoyalBlue), 45, scale).Spawn();
                new HRShinyOrb(pos, vel, Color.White, 45, scale * .75f).Spawn();
                Dust d = Dust.NewDustPerfect(pos, DustID.WhiteTorch, RandVelTwoPi(0.2f, 3.1f));
                d.scale *= 1.3f;
            }

            for (int i = 0; i < 20; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = target.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (target.Center - spawnPos).ToSafeNormalize()*Main.rand.NextFloat(1f, 20f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }
        }
        public void SetFirstFrame()
        {
            if (PostFirstFrame)
                return;
            PostFirstFrame = true;
            RandOffset1 = Main.rand.NextFloat(0, 10);
            RandOffset2 = Main.rand.NextFloat(0, 10);
            RandOffset3 = Main.rand.NextFloat(0, 10);
            RandOffset4 = Main.rand.NextFloat(0, 10);
            float maxPoints = 50;
            float xMult = 3.5f;
            float yMult = 2f;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= xMult;
                finalPos.Y *= yMult;
                CenterPosList.Add(finalPos.RotatedBy(Projectile.rotation));
            }
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Texture_StandardGradient.Value;
            Effect e = HJScarletShader.AlphaFade;
            e.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            e.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            e.Parameters["uFadeinTopLength"].SetValue(0);
            e.Parameters["uFadeinBottomLength"].SetValue(0.4f);
            e.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));   
            e.CurrentTechnique.Passes[0].Apply();
            Color setColor = Color.Lerp(Color.RoyalBlue, Color.WhiteSmoke, 0.7f);
            DrawBaseWave(tex, Color.LightSkyBlue* 0.30f, 18f);
            DrawBaseWave(tex, Color.SkyBlue* 0.40f, 15f);
            DrawBaseWave(tex, Color.DarkBlue * .50f, 12f);
            DrawBaseWave(tex, Color.RoyalBlue* .50f, 6f);
            DrawBaseWave(tex, setColor* .90f, 1.5f);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            Vector4 vector4 = new(0.2f, 0.2f, 0.05f, 0.6f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.79f + RandOffset1), new Vector2(5f, 0.07f), Color.LightSkyBlue);
            DrawBaseWave(texture2, Color.LightSkyBlue, 10f);

            texture2 = HJScarletTexture.Noise_Misc.Value;
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.15f + RandOffset2), new Vector2(5f, 0.1f), Color.LightBlue);
            DrawBaseWave(texture2, Color.LightBlue* 0.82f, 10f);

            texture2 = HJScarletTexture.Noise_Aura.Value;
            setColor = Color.Lerp(Color.LightSkyBlue, Color.WhiteSmoke, 0.70f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.15f + RandOffset3), new Vector2(2f, 0.205f), Color.White);
            DrawBaseWave(texture2, setColor, 20f);
            DrawBaseWave(texture2, Color.RoyalBlue * 0.5f, 20f);


            setColor = Color.Lerp(Color.RoyalBlue, Color.WhiteSmoke, 0.28f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.35f + RandOffset4), new Vector2(1.5f, 0.31f), setColor);
            DrawBaseWave(texture2, setColor, 20f);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!PostFirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;

            return false;
        }
        public void DrawBaseWave(Texture2D tex, Color color, float v)
        {

            List<ScarletVertex> VertexList = [];
            Vector2 projVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 42;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 posHead = CenterPosList[i] + Projectile.Center - Main.screenPosition;
                Vector2 posSrc = CenterPosList[i] + Projectile.Center - Main.screenPosition - projVel * v * Projectile.Opacity;
                VertexList.Add(new ScarletVertex(posHead, color, new Vector3(progress, 0, 0)));
                VertexList.Add(new ScarletVertex(posSrc, color, new Vector3(progress, 1, 0)));
            }
            if (VertexList.Count < 3)
                return;
            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexList.ToArray(), 0, VertexList.Count - 2);
        }
    }
}
