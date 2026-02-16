using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class BinaryStarsHeldClone: HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<BinaryStarsMain>().Texture;
        private enum DoType
        {
            IsShooted,
            IsReturn,
            IsReverse
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }
        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 72;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 6;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturn:
                    DoReturn();
                    break;
                case DoType.IsReverse:
                    DoReverse();
                    break;
            }
        }
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > 65f)
            {
                Projectile.netUpdate = true;
                AttackTimer = 0;
                AttackType = DoType.IsReturn;
            }
        }
        private void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, 3600f, 28f, 10);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                AttackType = DoType.IsReverse;
                Projectile.localNPCHitCooldown = 45;
                Projectile.netUpdate = true;
            }
        }
        private void DoReverse()
        {
            AttackTimer += 1;
            if (AttackTimer > 10f)
            {
                AttackTimer = 10f;
                if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex,searchDistance: 1800f,canPassWall:true))
                {
                    Projectile.extraUpdates = 8;
                    Projectile.HomingTarget(target.Center, 1800f, 24f, 18f);
                    if (Projectile.Hitbox.Intersects(target.Hitbox))
                        Projectile.Kill();
                }
                else
                    Projectile.extraUpdates = 6;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_GroundHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.8f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            PrettySpark(hit.Damage);
        }
        private void PrettySpark(int hitDamage)
        {
            //圆环
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = ToRadians(i * 10f).ToRotationVector2() * Projectile.scale;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 12f + dir2 * 18f;
                new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, 3.5f - Math.Abs(18f - i) / 6f, BlendStateID.Additive).Spawn();
            }
            //从灾厄抄写的锤子特效
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, hitDamage, true);
            Vector2 splatterDirection = Projectile.velocity * 0.8f;
            for (int i = 0; i < 10; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1.4f, 1.8f);
                sparkVelocity.Y -= 7f;
                new StarShape(Projectile.Center, sparkVelocity, sparkColor, sparkScale, sparkLifetime).Spawn();
            }
        }

        #region DrawMethod
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width + 20;
            width *= SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = BinaryStarsMain.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        #endregion
        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: -PiOver4);
            Projectile.DrawProj(Color.White, 4, 0.7f, -PiOver4);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center))
            {
                SB.End();
                SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Violet);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: 12f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: -12f);
                DrawTrails(HJScarletTexture.Trail_ParaLine.Texture, Color.White, 0.4f,alphaValue: 1f);

                SB.End();
                SB.BeginDefault();
            }
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            //做掉可能存在的零向量
            GD.Textures[0] = useTex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count - 1; i++)
            {
                float progress = (float)i / (validPosition.Count - 1);
                float rotated = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 + rotated.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight - Main.screenPosition;
                Vector2 posOffset = new Vector2(0, SetProjWidth(progress) * multipleSize).RotatedBy(rotated);
                ScarletVertex upClass = new(oldCenter - posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 0, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }
    }
}
