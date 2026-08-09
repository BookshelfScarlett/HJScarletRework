using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using rail;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class StormSaberExecution : HJScarletProj,IPixelatedRenderer
    {
        public override string Texture => GetInstance<StormSaberHeldProj>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        private Vector2 DrawOffset = new Vector2(100, 0);
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(-100, 0);
        private Vector2 TopRightPoint = new Vector2(50, -100);
        private Vector2 BottomLeftPoint = new Vector2(50, 100);
        private Vector2 BottomRightPoint = new Vector2(-100, 0);
        public float RandOffset1;
        public float RandOffset2;
        public float RandOffset3;
        public float RandOffset4;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(60);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 8;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(-1);
            Projectile.width = Projectile.height = 30;
            Projectile.Opacity = 0;

        }
        public override void OnFirstFrame()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, .1f);
            if (Projectile.IsOutScreen())
                return;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -40.2f * offsetHeight);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)((Projectile.oldPos.Length-6) * Clamp(Projectile.velocity.Length(), 0, 1));
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] == Vector2.Zero)
                    continue;
                float rot = Projectile.oldRot[j];
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2 +Projectile.SafeDir() * 10f, drawColor, new Vector2(0, 28 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            Vector2 drawRot = tex.Size() / 2f;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkGray, 1.26f, .75f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Silver, 0.8f, .75f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f, .75f, 0.95f);

            SB.EndShaderArea();
            SB.Draw(tex, drawPosition - DrawOffset.RotatedBy(Projectile.rotation), null, Color.White, drawRotation, drawRot, Projectile.scale * 1.2f, se, 0);
            
            return false;
        }
    }
}
