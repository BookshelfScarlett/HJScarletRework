using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class StormSaberSlash : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
        }
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(-300, 0);
        private Vector2 TopRightPoint = new Vector2(100, -300);
        private Vector2 BottomLeftPoint = new Vector2(100, 300);
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
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 120;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 1;
            Projectile.Opacity = 0f;
        }
        public override void OnFirstFrame()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float heldscale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            PostFirstFrame = true;
            RandOffset1 = Main.rand.NextFloat(0, 10);
            RandOffset2 = Main.rand.NextFloat(0, 10);
            RandOffset3 = Main.rand.NextFloat(0, 10);
            RandOffset4 = Main.rand.NextFloat(0, 10);
            float maxPoints = 50 * heldscale;
            float xMult = 2.5f * heldscale;
            float yMult = 0.57f * heldscale;
            //float yMult = 0.5f;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= xMult;
                finalPos.Y *= yMult;
                CenterPosList.Add(finalPos.RotatedBy(Projectile.rotation));
            }

            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.12f);
            if (Projectile.IsOutScreen())
                return;
            for (int i = 0; i < 2; i++)
            {
                int index = Main.rand.Next(10, CenterPosList.Count - 10);
                Vector2 pos = CenterPosList[index] + Projectile.Center - Projectile.SafeDir() * 30f;
                ECSParticle.SnowCloud(pos, Projectile.velocity * Main.rand.NextFloat(.4f), RandLerpColor(Color.White, Color.Gray), 45, RandRotTwoPi, 0.45f, Main.rand.NextFloat(.9f, 1.3f) * 0.270f);
            }
            for (int i = 0; i < 6; i++)
            {
                int index = Main.rand.Next(10, CenterPosList.Count - 10);
                Vector2 pos = CenterPosList[index] + Projectile.Center;
                int lifeTime = Main.rand.Next(30, 60);
                ECSParticle.LiliesFire(pos, Projectile.velocity * Main.rand.NextFloat(.4f), RandLerpColor(Color.White, Color.Gray), lifeTime, RandRotTwoPi, 0.55f, Main.rand.NextFloat(.9f, 1.3f) * 0.40f, true, BlendState.Additive);
            }

            for (int i = 0; i < 3; i++)
            {
                int index = Main.rand.Next(3, CenterPosList.Count - 3);
                Vector2 pos = CenterPosList[index] + Projectile.Center;
                ECSParticle.ShinyCrossStarECS(pos, Projectile.velocity / 2f, RandLerpColor(Color.WhiteSmoke, Color.White), 40, 1, .40f, 0.2f);
            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //if (targetHitbox.Intersects(projHitbox))
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate<StormSaber>();
            for (int i = 0; i < 34; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 4.2f), Color.White, 40, 1, 0.6f);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 6.2f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, 1, 0.9f, 0.31f, blendstate: BlendState.AlphaBlend);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            if (Projectile.IsOutScreen())
                return false;

            PixelatedRenderManager.BeginDrawProj = true;

            return false;
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Texture_StandardGradient.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(.15f, .15f, 0f, .65f), Vector2.Zero, Vector2.One);
            Color setColor = Color.Lerp(Color.RoyalBlue, Color.WhiteSmoke, 0.7f);
            DrawBaseWave(tex, Color.WhiteSmoke * .75f, 1.2f);
            DrawBaseWave(tex, Color.White * 0.30f, 4.4f);
            DrawBaseWave(tex, Color.White * 0.50f, 8f);
            DrawBaseWave(tex, Color.WhiteSmoke * .40f, 16f);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            Vector4 vector4 = new(0.25f, 0.25f, 0.05f, 0.46f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.59f + RandOffset1), new Vector2(1.5f, 0.091f), Color.White);
            DrawBaseWave(texture2, Color.White, 10f);

            texture2 = HJScarletTexture.Noise_Misc.Value;
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.25f + RandOffset2), new Vector2(5f, 0.1f), Color.White);
            DrawBaseWave(texture2, Color.White * 0.82f, 10f);

            texture2 = HJScarletTexture.Noise_Aura.Value;
            vector4 = new(0.2f, 0.2f, 0.05f, 0.56f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.45f + RandOffset3), new Vector2(2.4f, 0.205f), Color.White);
            DrawBaseWave(texture2, Color.WhiteSmoke * .5f, 20f);
            DrawBaseWave(texture2, Color.White * 0.5f, 20f);

            setColor = Color.Lerp(Color.White, Color.WhiteSmoke, 0.28f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.35f + RandOffset4), new Vector2(1.55f, 0.31f), setColor);
            DrawBaseWave(texture2, setColor, 20f);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public void DrawBaseWave(Texture2D tex, Color color, float v)
        {

            List<ScarletVertex> VertexList = [];
            Vector2 projVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 42;
            float heldscale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 posHead = CenterPosList[i] + Projectile.Center - Main.screenPosition;
                Vector2 posSrc = CenterPosList[i] + Projectile.Center - Main.screenPosition - projVel * v * Projectile.Opacity * heldscale;
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
