using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    //public abstract class DaggerMarkClass : ModType
    //{
    //    public int Type = 0;
    //    /// <summary>
    //    /// 这个Mark对应的物品
    //    /// </summary>
    //    public virtual int BelongToWeapon => -1;
    //    /// <summary>
    //    /// Mark对应的贴图材质
    //    /// </summary>
    //    public virtual Texture2D MarkTexture => null;
    //}
    /// <summary>
    /// 代办：应该会改成ModType，for sake of crossover content
    /// </summary>
    public abstract class DaggerMarkClass : ModProjectile,  ILocalizedModType, IPixelatedRenderer
    {
        public Player Owner => Main.player[Projectile.owner];
        public DaggerMarkPlayer DaggerPlayer => Owner.GetModPlayer<DaggerMarkPlayer>();
        public static string MarkPath => $"HJScarletRework/Assets/Texture/Items/Weapons/";
        public override string Texture => MarkPath + GetType().Name;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforePlayer;
        public BlendState BlendState => BlendState.Additive;

        public new string LocalizationCategory => "Projs.Friendly.Executor";
        public static int PositionIndex = -1;
        public float Osci = 0f;
        public virtual Color BackgroundColor => Color.White;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.timeLeft = GetSeconds(10);
            Projectile.tileCollide = false;
            Projectile.scale = 0;
            Projectile.Opacity = 0;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public bool JustAdded = false;
        public sealed override void AI()
        {
            int listCount = DaggerPlayer.DaggerList.Count;
            if (Projectile.scale < 0.98f)
            {
                Projectile.scale = Lerp(Projectile.scale, 1f, 0.21f);
                Projectile.Opacity = Projectile.scale;
            }
            else
                Projectile.scale = Projectile.Opacity = 1;

            if (!Projectile.HJScarlet().FirstFrame)
            {
                if (!DaggerPlayer.DaggerList.Contains(Type))
                {
                    DaggerPlayer.DaggerList.Add(Type);
                }
                //列表最多只有三个元素，无论生成时怎么样，都直接击杀掉最开始的那个元素
                if (DaggerPlayer.DaggerList.Count > 2)
                    DaggerPlayer.DaggerList.RemoveAt(0);
                JustAdded = true;
                FirstFrame();
                Projectile.netUpdate = true;
            }
            //列表最多只有三个元素
            PositionIndex = DaggerPlayer.DaggerList.IndexOf(Type);
            Projectile.timeLeft = 2;
            Osci += ToRadians(0.5f);
            if (Osci >= ToRadians(360f))
                Osci = ToRadians(-360f);
            Vector2 mountedPos = Owner.MountedCenter - Vector2.UnitX.RotatedBy(ToRadians(120f * PositionIndex) + Osci * Owner.direction) * 82f * Owner.direction;
            mountedPos.Y += (float)Math.Sin((Osci * 2f)) * 10f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.32f);
            MarkAI();
        }
        public override bool ShouldUpdatePosition() => false;

        public virtual void MarkAI()
        {

        }

        public virtual void FirstFrame()
        {
            
        }
        public sealed override void OnKill(int timeLeft)
        {
            if (DaggerPlayer.DaggerList.Contains(Type))
                DaggerPlayer.DaggerList.Remove(Type);
            ExOnKill(timeLeft);
        }
        public SpriteBatch SB { get => Main.spriteBatch; }
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
            for(int i =0;i<8;i++)
                SB.Draw(tex, drawPos + ToRadians(360f / 8 * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(50), rot, tex.ToOrigin(), Projectile.scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, rot, tex.ToOrigin(), Projectile.scale, se, 0);
            
            ExPostPreDraw();
            return false;
        }

        public virtual void ExPostPreDraw()
        {

        }

        public virtual void ExOnKill(int timeLeft)
        {
        }
    }
    public class DaggerMarkPlayer : ModPlayer
    {
        public List<int> DaggerList = new();
        public List<int> DaggerIndexList = new();
    }
    public class DesertDaggerMark : DaggerMarkClass
    {
        public override Color BackgroundColor => Color.DarkGoldenrod;
        public override void MarkAI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
                d.velocity = Vector2.UnitY;
                d.noGravity = true;
                d.scale *= 1.4f;
            }

        }
        public override void ExPostPreDraw()
        {
        }
    }
    public class GhostDaggerMark : DaggerMarkClass
    {
        public override Color BackgroundColor => Color.GhostWhite;
        public override void MarkAI()
        {
            if (Main.rand.NextBool(3))
            {
                //new ShinyCrossStar(Projectile.ToRandRec(), Vector2.UnitY, RandLerpColor(Color.DarkOrange, Color.OrangeRed), Main.rand.Next(20, 40), 0, 1, 0.3f, false).Spawn();
                new SmokeParticle(Projectile.ToRandRec(), Vector2.UnitY, RandLerpColor(Color.Gray, Color.GhostWhite), Main.rand.Next(20, 40), RandRotTwoPi, 1, 0.20f, true).Spawn();
            }

            base.MarkAI();
        }
        public override void ExPostPreDraw()
        {
            base.ExPostPreDraw();
        }
    }
    public class MoltenDaggerMark : DaggerMarkClass
    {
        public override Color BackgroundColor => Color.OrangeRed;
        public override void MarkAI()
        {
            if (Main.rand.NextBool(3))
            {
                new ShinyCrossStar(Projectile.ToRandRec(), Vector2.UnitY, RandLerpColor(Color.DarkOrange, Color.OrangeRed), Main.rand.Next(20, 40), 0, 1, 0.5f, false).Spawn();
            }
        }
        public override void ExPostPreDraw()
        {
            base.ExPostPreDraw();
        }
    }
}
