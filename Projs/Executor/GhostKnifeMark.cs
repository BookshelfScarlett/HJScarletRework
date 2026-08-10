using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public abstract class KnifeMarkClass : HJScarletProj, IPixelatedRenderer
    {
        public static string MarkPath => $"HJScarletRework/Assets/Texture/Items/Weapons/";
        public override string Texture => MarkPath + GetType().Name;
        public ref float Osci => ref Projectile.ai[0];
        public bool CanKillCurrentMark
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }
        /// <summary>
        /// 这个标记射弹需要持续多少秒
        /// <br>默认<see langword="60"/>秒，即<see langword="3600"/>帧</br>
        /// </summary>
        public virtual int LifeTime => 60;
        /// <summary>
        /// 这个标记射弹的背景颜色
        /// <br>绘制是统一管理的，如果你<see langword="真的"/>需要完全接管绘制自己重写</br>
        /// <br>请自行覆盖<see cref="PreDraw(ref Color)"/>与像素化渲染<see cref="RenderPixelated(SpriteBatch)"/>本身</br>
        /// </summary>
        public virtual Color BackgroundColor => Color.White;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 40;
            Projectile.timeLeft = GetSeconds(LifeTime);
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            ExSD();
        }
        public sealed override void OnFirstFrame()
        {
            ExtraFirstFrame();
        }
        /// <summary>
        /// 第一帧的初始化内的其他操作
        /// <br>该操作发生在完成射弹index传入玩家内之后</br>
        /// <br>你可以根据需要在这里写入一些特效，或者一些杂七杂八的东西</br>
        /// </summary>
        public virtual void ExtraFirstFrame()
        {
        }
        public override void ProjAI()
        {
            //这里检查当前射弹的Type是否与玩家内的Index一致
            //如果不一致，准备进入处死状态
            Owner.HJScarlet().KnifeMarkIndex = Type;
            if (Owner.HJScarlet().KnifeMarkIndex != Type)
            {
                CanKillCurrentMark = true;
                Projectile.netUpdate = true;
            }

            if (Projectile.timeLeft > 50 && !CanKillCurrentMark)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.21f);
                Projectile.scale = Lerp(Projectile.scale, 1.15f, .21f);
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.1f);
                Projectile.scale = Projectile.Opacity;
                Owner.HJScarlet().KnifeMarkIndex = -1;
            }
            Osci += ToRadians(.5f);
            Vector2 targetVector = Owner.Center.GetNormalVector2(Main.MouseWorld);
            Vector2 mountedPos = Owner.MountedCenter - Vector2.UnitX.RotatedBy(targetVector.ToRotation()) * 80f;
            mountedPos.Y += (float)Math.Sin((Osci * 2f)) * 10f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.32f);
            ExProjAI();
            //更新type的状态
            if (!CanKillCurrentMark)
                Owner.HJScarlet().KnifeMarkIndex = Type;
        }
        public virtual void ExProjAI() { }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforePlayer;
        public BlendState BlendState => BlendState.Additive;

        public void RenderPixelated(SpriteBatch sb)
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_Lightning4.Texture;
            float BeamLength = (Projectile.Center - Owner.MountedCenter).Length();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            //轨迹
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -40);
            shader.Parameters["uColor"].SetValue(BackgroundColor.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, BackgroundColor, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.25f * Projectile.scale), 0, 0);
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.20f * Projectile.scale), 0, 0);
            //边框
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D ring = HJScarletTexture.Particle_ShinySquareSplit.Value;
            Texture2D block = HJScarletTexture.Texture_WhiteCube.Value;
            float scale = Projectile.scale * 0.195f;
            //用于填色
            SB.Draw(block, Projectile.Center - Main.screenPosition, null, BackgroundColor * .65f, PiOver4, block.ToOrigin(), Projectile.scale * 2.1f, 0, 0);
            for (int i = 0; i < 4; i++)
                SB.Draw(ring, Projectile.Center - Main.screenPosition, null, BackgroundColor, PiOver2 * i, ring.ToOrigin(), scale, 0, 0);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteEffects se = Owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rot = Owner.direction > 0 ? Projectile.rotation + PiOver4 : Projectile.rotation - PiOver4;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(360f / 8 * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(50), rot, tex.ToOrigin(), Projectile.scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, rot, tex.ToOrigin(), Projectile.scale, se, 0);
            return false;
        }
    }
    public class GhostKnifeMark : KnifeMarkClass
    {
        public override Color BackgroundColor => Color.WhiteSmoke;
        public override void ExtraFirstFrame()
        {
            Vector2 spawnPos = Projectile.Center;
            for (int i = 0; i < 6; i++)
            {
                Color color = RandLerpColor(Color.SkyBlue, Color.White);
                new NoiseShockRing(spawnPos, Vector2.Zero, color, 45, 1f, .13f + i * 0.2f, -1, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 50; i++)
                ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
            ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, 0.45f);
        }
        public override void ExProjAI()
        {
            if (Main.rand.NextBool(3))
            {
                new SmokeParticle(Projectile.ToRandRec(), Vector2.UnitY, RandLerpColor(Color.SkyBlue, Color.White), Main.rand.Next(20, 40), RandRotTwoPi, 1, 0.20f, true).Spawn();
            }
        }
    }
}
