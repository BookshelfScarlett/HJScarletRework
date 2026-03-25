using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JudgementHolyCross : HJScarletFriendlyProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;

        private ref float Counter => ref Projectile.ai[0];
        private float OriginalSpeed => Projectile.ai[1];
        private ref float Rotation => ref Projectile.ai[2];
        private float MountedX => Projectile.localAI[0];
        private float MountedY => Projectile.localAI[1];
        private float InitVec = 0;
        public override void ExSD()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(10, ImmnuityType.Static);
            //这玩意是转起来的，所以实际dps会更少的，给他多点判定吧！！
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 80;

        }
        float opc = 1;
        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1))
                return null;
            return false;
        }
        public override void AI()
        {
            //刚加入时的初始化。
            if (Counter == 0)
            {
                InitVec = Projectile.velocity.ToRotation();
                Projectile.rotation = InitVec;
            }
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Counter++;
            //让这个东西绕着转一会……
            Rotation += ToRadians(1.5f);
            //增加这个……转角。
            float curRot = InitVec + Rotation;
            //最后算速度。和一些别的。
            Projectile.velocity = curRot.ToRotationVector2() * OriginalSpeed;
            //转角处理。
            Projectile.rotation = Projectile.velocity.ToRotation();
            //维持悬挂让他跟随敌对单位
            Projectile.Center = Vector2.Lerp(Projectile.Center, new Vector2(MountedX, MountedY), 0.1f);
            if (Counter > 60)
            {
                opc = Lerp(opc, 0f, 0.1f);
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            HJScarletMethods.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 900, targetHitbox, 24);
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        private void DrawLaser(Texture2D warn, Vector2 drawPos, Color drawColor, Vector2 scale)
        {
            Vector2 ori = warn.Size() / 2 * new Vector2(0, 1);
            //SB.End();
            //SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(warn.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(120, warn.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() *opc);
            shader.Parameters["uFadeoutLength"].SetValue(0.4f);
            shader.Parameters["uFadeinLength"].SetValue(0f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(warn, drawPos, null, drawColor, Projectile.rotation, ori, scale, SpriteEffects.None, 0);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            //贴图。
            Texture2D warn = HJScarletTexture.Trail_ManaStreak.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int laserLengthScale = 35;
            //基础大小设定
            Vector2 baseScale = Projectile.scale * 0.32f * new Vector2(1, opc);
            DrawLaser(warn, drawPos, Color.DarkOrange * opc, baseScale * new Vector2(laserLengthScale, 1.2f));
            DrawLaser(warn, drawPos, Color.Orange* opc, baseScale * new Vector2(laserLengthScale, 0.8f));
            DrawLaser(warn, drawPos, Color.LightYellow* opc, baseScale * new Vector2(laserLengthScale, 0.65f));
            DrawLaser(warn, drawPos, Color.White* opc, baseScale * new Vector2(laserLengthScale, 0.35f));

            HJScarletMethods.EndShaderAreaPixel();
        }
    }
}
