using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 感谢 Demon Lilies 提供的 风暴管束者 相关代码
    /// </summary>
    public class FrostoftheStormHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<FrostoftheStorm>();
        public override string Texture => GetInstance<FrostoftheStorm>().Texture;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override ClassCategory Category => ClassCategory.Executor;
        public AnimationStruct Helper = new(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public int HeavySwordLength = 140;
        public bool Flip = false;
        public float Height = .81f;
        public bool KeepSpawning = false;
        public float SlashOpacity = 1;
        public bool SpawnProj = false;
        public List<Vector2> OldAimPos = [];
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.extraUpdates, 5 * Projectile.extraUpdates);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 10;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            Projectile.originalDamage = Projectile.damage;
            Helper.MaxProgress[0] = (int)(AttackSpeed * 0.30f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * 0.65f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * .10f);
            TargetRotation = BeginTargetRotation;
            HeavySwordLength = 140;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //if (projHitbox.Intersects(targetHitbox))
            //    return true;
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseInBack(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + OldAimPos[^1];
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;

        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            HandleAttackAnimation();
            HandleHeldProjState();
            HandlePlayerState();
            HandleExecution();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -5 * -Owner.direction, 20, MathHelper.TwoPi, 0.5f, true, 1000);
            int dustCount = 36;
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = target.Center.ToRandCirclePos(10f) + dir * Main.rand.NextFloat(10f);
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
                Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 20f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<FrostoftheStormExecution>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                if (!Flip)
                    ((FrostoftheStormExecution)proj.ModProjectile).Flip = true;
                else
                    ((FrostoftheStormExecution)proj.ModProjectile).Flip = false;
                ((FrostoftheStormExecution)proj.ModProjectile).BeginTargetRotation = TargetRotation;
            }
            else
            if (Main.mouseLeft)
            {
                //挥舞结束的时候处死并立刻生成新的射弹。这样我们不用重置大部分的动画进程，实现起来稍微方便点
                if (!Flip)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    //改向
                    ((FrostoftheStormHeldProj)proj.ModProjectile).Flip = true;
                    //存储当前挥舞角度
                    ((FrostoftheStormHeldProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                }
                else
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    ((FrostoftheStormHeldProj)proj.ModProjectile).Flip = false;
                    ((FrostoftheStormHeldProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                }
            }
        }

        public void HandlePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void HandleAttackAnimation()
        {
            //跳过末尾动画，如果玩家试图一直攻击的话
            if (!Helper.IsDone[0])
                UpdateBeginAnimation();
            else if (!Helper.IsDone[1])
                UpdateMidAnimation();
            else
                Projectile.Kill();
        }
        public void UpdateBeginAnimation()
        {
            if (Helper.GetAniProgress(0) > 0.3f && !SpawnProj)
            {
                SpawnProj = true;
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Release with { Variants = [Main.rand.Next(1, 3)], MaxInstances = 0, PitchVariance = .3f, Pitch = -.05f });
                Vector2 fireVel = (Main.MouseWorld - Owner.Center).ToSafeNormalize() * 40;
                Vector2 pos = Owner.MountedCenter - fireVel.ToSafeNormalize() * 200;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, fireVel, ProjectileType<FrostoftheStormSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
            }
            //这里挥砍动画一定程度上使用了矩阵变化。
            Helper.UpdateAniState(0);
            float easedProgress = EaseInBack(Helper.GetAniProgress(0));
            //末尾角度，也是下一个动画进程的起始角度
            float endAngle = 135f * Flip.ToDirectionInt();
            float beginAngle = -150f * Flip.ToDirectionInt();
            //更新当前的转角
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //将其投影到矩阵上，并进行形变
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.2f, Height, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f;
            //这样，Scale就是tarPos的向量模长
            Projectile.scale = tarPos.Length();
            //武器的角度为（起始角度 + 目标角度）的值
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            //更新动画进程，封装的方法
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {

                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1.2f, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.2f;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 200f;
                OldAimPos.Add(slashPosFinal);
                for (int i = 1; i <= 2; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 180, Main.rand.NextFloat(.60f, .70f));
                    if (i == 2)
                        pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.50f, .98f));
                    float scale = .64f * Main.rand.NextFloat(0.8f, 1.3f);

                    Vector2 dir = (pos - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * Main.rand.NextFloat(0.1f, 1.5f) + dir * Main.rand.NextFloat(0.1f, 44f);
                    new SnowCloud(pos, vel * 0.05f, RandLerpColor(Color.Lerp(Color.SkyBlue, Color.WhiteSmoke, 0.5f), Color.RoyalBlue), 20, RandRotTwoPi, .150f + 0.050f * i, scale * (0.50f + i * 0.2f), Main.rand.NextBool()).SpawnToPriority();
                }
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.01f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(12f, 20.5f);
                    new HRShinyOrb(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.1f * Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 40, 0.051f * Projectile.scale).Spawn();
                }
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.01f, .98f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(5f, 9f);
                    new ShinyCrossStar(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 30, 0, 0.5f, Main.rand.NextFloat(.7f, 1.01f) * Projectile.scale * .75f, false).Spawn();
                }
            }
        }
        public void UpdateMidAnimation()
        {
            Helper.UpdateAniState(1);
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float beginAngle = 135 * Flip.ToDirectionInt();
            float endAngle = 150 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.2f, Height, 1f);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            SlashOpacity = Lerp(SlashOpacity, 0f, 0.03f / Projectile.extraUpdates);
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
            if (SlashOpacity < 0.02f)
                SlashOpacity = 0;
        }


        public void UpdateEndAnimation()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(2));
            float rot = Helper.UpdateAngle(115 * Flip.ToDirectionInt(), 125 * Flip.ToDirectionInt(), Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.1f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Helper.UpdateAniState(2);
        }

        public void HandleHeldProjState()
        {
            Projectile.Center = Owner.MountedCenter;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            DrawSword();
            return false;
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.White * 0.30f, 0.95f);
            DrawSlash(texture, Color.RoyalBlue * 0.90f, 0.7f);
            DrawSlash(texture, Color.LightBlue * 0.46f, 0.4f);
            DrawSlash(texture, Color.DarkBlue * 0.40f, 0f);

            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.2f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * .35f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
            effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.AliceBlue * .85f, 0.80f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White * .90f, 0.75f);

            texture2 = HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect3 = HJScarletShader.AlphaFadeNoiseColor;
            effect3.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect3.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect3.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0f, 0));
            effect3.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect3.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect3.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture2, Color.White * .5f, 0.64f);
            DrawSlash(texture2, Color.RoyalBlue * .5f, 0.65f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
        public void DrawSword()
        {
            //基础的一些数据
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + PiOver2;
            Vector2 drawPoint = new Vector2(tex.Width * 0.5f, tex.Height * 0.85f);
            bool ignoreFlip = Owner.direction > 0;
            SpriteEffects se = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (ignoreFlip)
            {
                se = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -12;
            Color edgeColor;
            if (!Helper.IsDone[0])
            {
                edgeColor = Color.Lerp(Color.Transparent, Color.White, Helper.GetAniProgress(0));
            }
            else
                edgeColor = Color.Lerp(Color.White, Color.Transparent, Helper.GetAniProgress(2));
            if (KeepSpawning)
                edgeColor = Color.White;
            //绘制残影
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, realDrawPos + ToRadians(360f / 8 * i).ToRotationVector2() * 2f, null, edgeColor.ToAddColor(), drawRot, drawPoint, Projectile.scale, se, 0);

            SB.Draw(tex, realDrawPos, null, Color.White, drawRot, drawPoint, Projectile.scale, se, 0);
        }
    }
}
