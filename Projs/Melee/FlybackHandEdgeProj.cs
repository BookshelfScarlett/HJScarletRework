using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class FlybackHandEdgeProj: HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float MountedX => ref Projectile.localAI[0];
        public ref float MountedY => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.ai[0];
        private enum Styles
        {
            Toss,
            HomingBack,
            Fading
        }
        private Styles AttackType
        {
            get => (Styles)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public int MountedIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(40, 2);
        }
        public override void SetDefaults()
        {
            Projectile.height = Projectile.width = 10;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
            Projectile.friendly = true;
        }
        public bool ShouldReturnToOwner = false;
        public override bool ShouldUpdatePosition() => ShouldReturnToOwner;
        public override void AI()
        {
            Projectile.timeLeft = 2;
            switch(AttackType)
            {
                case Styles.Toss:
                    DoToss();
                    break;
                case Styles.HomingBack:
                    DoHomingBack();
                    break;
                case Styles.Fading:
                    DoFading();
                    break;
            }
        }

        private void DoFading()
        {
            //此处，需要先让小光球自行消失，轨迹后面跟上
            Projectile.Opacity -= 0.05f;
            //启用计时器。在合适的时候强制处死他
            Timer++;
            if (Timer > 60f)
                Projectile.Kill();
            //固定玩家位置
            Projectile.Center = Owner.MountedCenter;
        }

        private void DoToss()
        {
            Timer++;
            //基于这个原点。
            Vector2 mountedCenter = new Vector2(MountedX, MountedY);
            Vector2 targetCenter = mountedCenter + Projectile.SafeDir().RotatedBy(ToRadians(Timer * 6)) * 250f;
            Projectile.Center = Vector2.Lerp(new Vector2(MountedX, MountedY), targetCenter, 0.2f);
            Projectile mountedProj = Main.projectile[MountedIndex];
            if (mountedProj.type != ProjectileType<FlybackHandClockMounted>() || !mountedProj.active)
            {
                ShouldReturnToOwner = true;
                AttackType = Styles.HomingBack;
            }

        }

        private void DoHomingBack()
        {
            //为射弹提供一个随机的初速度
            if(Timer >= 0)
            {
                Projectile.velocity = Vector2.UnitY.RotatedByRandom(TwoPi) * 12f;
                Timer = -1;
                return;
            }
            Timer--;
            if (Timer < -30f)
            {
                //让其返程至玩家手上，并回复玩家一定的血量
                Projectile.HomingTarget(Owner.Center, 1800f, 20f, 20f);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Owner.Heal(5);
                    AttackType = Styles.Fading;
                    Timer = 0;
                    ShouldReturnToOwner = false;
                }
            }
            else
            {
                //计时器不合规的情况下，减速慢行即可
                Projectile.velocity *= 0.98f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawTrails(Color.DeepSkyBlue, 14f);
            DrawTrails(Color.SkyBlue, 12.2f);
            DrawTrails(Color.LightBlue, 10.8f);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawStar(Projectile.Center - Main.screenPosition, Projectile.rotation, Color.SkyBlue);
            SB.End();
            SB.BeginDefault();
            return false;
        }
        public void DrawTrails(Color trailColor, float height)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4());
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.51f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.01f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPos, out List<float> validRot);
            GD.Textures[0] = HJScarletTexture.Trail_ManaStreak.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            Vector2[] ribbonPositions = [.. validPos];
            DrawSetting drawSetting = new(HJScarletTexture.Trail_ManaStreak.Value, true, false);
            List<TrailDrawDate> trailDrawDate = [];
            int positionCount = ribbonPositions.Length;
            for (int i = 0; i < positionCount - 1; i++)
            {
                // 这个顶点的旋转，从这个位置指向下一个位置
                Vector2 Position = ribbonPositions[i];
                Vector2 NextPosition = ribbonPositions[i + 1];
                float rot = (NextPosition - Position).ToRotation();
                trailDrawDate.Add(new(Position + Projectile.Size /2, Color.White, new Vector2(0, height * 0.28f), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDate], drawSetting);
        }
        public void DrawStar(Vector2 drawPos, float rot, Color starColor)
        {
            Texture2D sharpTears = HJScarletTexture.Particle_HRShinyOrb.Value;
            Vector2 targetSize = 0.24f * Projectile.scale * new Vector2(0.5f, 0.5f);
            SB.Draw(sharpTears, drawPos, null, starColor, rot, sharpTears.Size() / 2, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, drawPos, null, Color.White with { A = 150 }, rot, sharpTears.Size() / 2, targetSize * 0.5f, SpriteEffects.None, 0);
        }
    }
}
