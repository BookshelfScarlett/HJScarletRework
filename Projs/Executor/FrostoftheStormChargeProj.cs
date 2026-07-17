using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
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
    public class FrostoftheStormChargeProj : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => GetInstance<FrostoftheStormHeldProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public AnimationStruct Helper = new(5);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public int HeavySwordLength = 140;
        public bool Flip = false;
        public float Height = 1f;
        public bool KeepSpawning = false;
        public float SlashOpacity = 1;
        public bool SpawnProj = false;
        public int CurTime = 0;
        public int TotalSwingTime = 5;
        public List<Vector2> OldAimPos = [];
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.extraUpdates, 5 * Projectile.extraUpdates);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 12;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            Projectile.originalDamage = Projectile.damage;
            int slowSwingTime = (int)(AttackSpeed * 0.40f);
            int fastSwingTime = (int)(AttackSpeed * 0.275f);
            int slowEndTime = (int)(AttackSpeed * 0.30f);
            int fastEndTime = (int)(AttackSpeed * 0.18f);
            bool slowSwing = CurTime == 0 || FinalSwing;
            int fastFinal = (int)(AttackSpeed * 0.25f);
            int slowFinal = (int)(AttackSpeed * 0.40f);
            Helper.MaxProgress[0] = slowSwing ? slowSwingTime : fastSwingTime;
            Helper.MaxProgress[1] = slowSwing ? (slowEndTime) : fastEndTime;
            Helper.MaxProgress[2] = FinalSwing ? fastFinal : slowFinal;
            Helper.MaxProgress[3] = (int)(AttackSpeed * 0.25f);
            Helper.MaxProgress[4] = (int)(AttackSpeed * .50f);
            TargetRotation = BeginTargetRotation;
            HeavySwordLength = 140;
            Projectile.HJScarlet().HasExecutionMechanic = true;
            Projectile.HJScarlet().ExecutionStrike = true;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            HandleAttackAnimation();
            HandleHeldProjState();
            HandlePlayerState();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            float easedProgress = EaseInBack(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;

            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + OldAimPos[^1];
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }


        public bool SlowSwing => CurTime == 0 || FinalSwing;
        public bool FinalSwing => CurTime > TotalSwingTime;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FinalSwing)
            {
                Projectile.AddExecutionTimeImmediate(ItemType<FrostoftheStorm>(), 3);
            }
            else
            {
                Projectile.AddExecutionTimeImmediate(ItemType<FrostoftheStorm>());
            }
            if (Projectile.numHits < 1)
            {
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, (-(5 + SlowSwing.ToInt() * 80)) * -Owner.direction, 20, TwoPi, 0.5f, true, 1000);
                if (!SlowSwing)
                    SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { MaxInstances = 1, Volume = 0.6f, Pitch = -0.5f + 0.05f * CurTime });
                else
                    SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { MaxInstances = 0 });
            }
            if (!target.CanBeChasedBy() || HJScarletMethods.OutOffScreen(target.Center))
                return;
            if (SlowSwing)
            {
                Vector2 safeDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        ECSParticle.SnowCloud(target.Center.ToRandCirclePos(5f), safeDir.RotatedBy(PiOver2 * j) * Main.rand.NextFloat(.1f, 16f), RandLerpColor(Color.Lerp(Color.SkyBlue, Color.WhiteSmoke, .5f), Color.RoyalBlue), 40, 0, .35f, .62f * .21f);
                        ECSParticle.StarShape(target.Center.ToRandCirclePos(3f), safeDir.RotatedBy(PiOver2 * j) * Main.rand.NextFloat(.1f, 10f), RandLerpColor(Color.SkyBlue, Color.WhiteSmoke), 20, 1f, 1.1f);
                    }
                }
                new KiraStar(target.Center, Vector2.Zero, RandLerpColor(Color.Blue, Color.RoyalBlue), 20, safeDir.ToRotation(), 0.58f, 0.8f * 1.1f, 0, true, useAlt: true).Spawn();
                new KiraStar(target.Center, Vector2.Zero, Color.White, 20, safeDir.ToRotation(), 0.58f, 0.68f * 1.1f, 0, true, useAlt: true).Spawn();
                float ringScale = 0.35f * 1.1f;
                new ShinyRing(target.Center, Vector2.Zero, Color.Lerp(Color.RoyalBlue, Color.WhiteSmoke, 0.5f), 20, ringScale, 0, 0, 0.85f, true).SpawnToPriorityNonPreMult();
                new ShinyRing(target.Center, Vector2.Zero, Color.WhiteSmoke, 20, ringScale, Pi + PiOver4, 0, 0.65f, true).Spawn();
            }
            else
            {
                Vector2 safeDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        ECSParticle.SnowCloud(target.Center.ToRandCirclePos(5f), safeDir.RotatedBy(PiOver2 * j) * Main.rand.NextFloat(.1f, 16f), RandLerpColor(Color.Lerp(Color.SkyBlue, Color.WhiteSmoke, .5f), Color.RoyalBlue), 40, 0, .35f, .62f * .21f);
                        ECSParticle.StarShape(target.Center.ToRandCirclePos(3f), safeDir.RotatedBy(PiOver2 * j) * Main.rand.NextFloat(.1f, 10f), RandLerpColor(Color.SkyBlue, Color.WhiteSmoke), 20, 1f, 1.1f);
                    }
                }

                float starScale = .60f;
                new KiraStar(target.Center, Vector2.Zero, RandLerpColor(Color.Blue, Color.RoyalBlue), 20, safeDir.ToRotation(), 0.58f, starScale, 0, true, useAlt: true).Spawn();
                new KiraStar(target.Center, Vector2.Zero, Color.White, 20, safeDir.ToRotation(), 0.58f, starScale * 0.80f, 0, true, useAlt: true).Spawn();

            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.2f, 17.4f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .2f;
                ECSParticle.HRShinyOrb(pos, vel, Color.White, 45, 1, scale * .75f);
                Dust d = Dust.NewDustPerfect(pos, DustID.WhiteTorch, RandVelTwoPi(0.2f, 3.1f));
                d.scale *= 1.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (FinalSwing)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<FrostoftheStormHeldProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((FrostoftheStormHeldProj)proj.ModProjectile).Flip = !Flip;
                ((FrostoftheStormHeldProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                proj.HJScarlet().HasExecutionMechanic = true;
            }
            else
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((FrostoftheStormChargeProj)proj.ModProjectile).Flip = !Flip;
                ((FrostoftheStormChargeProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                ((FrostoftheStormChargeProj)proj.ModProjectile).CurTime = CurTime += 1;
                proj.HJScarlet().HasExecutionMechanic = true;
            }
        }

        public void HandlePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void HandleAttackAnimation()
        {
            //跳过末尾动画，如果玩家试图一直攻击的话
            if (!Helper.IsDone[0])
                UpdateBeginAnimation();
            else if (!Helper.IsDone[1])
                UpdateMidAnimation();
            else if (!Helper.IsDone[2] && SlowSwing)
            {
                UpdateEndAnimation();
            }
            else
                Projectile.Kill();
        }
        public bool PlaySound = false;
        public void UpdateBeginAnimation()
        {
            if (Helper.GetAniProgress(0) > 0.5f)
            {
                if (!PlaySound)
                {
                    SoundEngine.PlaySound(HJScarletSounds.Frostwave_Release with { Variants = [2], MaxInstances = 0, Pitch = -.05f + .1f * CurTime });
                    PlaySound = true;
                }
                if (!SpawnProj && !SlowSwing)
                {

                    SpawnProj = true;
                    Vector2 fireVel = (Main.MouseWorld - Owner.Center).ToSafeNormalize() * 40;
                    Vector2 pos = Owner.MountedCenter - fireVel.ToSafeNormalize() * (300 + SlowSwing.ToInt() * 100f);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, fireVel, ProjectileType<FrostoftheStormSlashGiant>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.ai[2] = SlowSwing.ToInt();
                }
            }
            if (Helper.GetAniProgress(0) > 0.8f && !SpawnProj && SlowSwing)
            {
                SpawnProj = true;
                Vector2 fireVel = (Main.MouseWorld - Owner.Center).ToSafeNormalize() * 40;
                Vector2 pos = Owner.MountedCenter - fireVel.ToSafeNormalize() * (300 + SlowSwing.ToInt() * 100f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, fireVel, ProjectileType<FrostoftheStormSlashGiant>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.ai[2] = SlowSwing.ToInt();
            }

            //这里挥砍动画一定程度上使用了矩阵变化。
            Helper.UpdateAniState(0);
            float easedProgress = EaseInBack(Helper.GetAniProgress(0));
            //末尾角度，也是下一个动画进程的起始角度
            float endAngle = 155f * Flip.ToDirectionInt();
            float beginAngle = -175f * Flip.ToDirectionInt();
            if (CurTime == 0)
            {
                beginAngle = -185f * Flip.ToDirectionInt();
            }
            if (SlowSwing)
                endAngle = 170f * Flip.ToDirectionInt();

            //更新当前的转角
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //将其投影到矩阵上，并进行形变
            float xScale = SlowSwing ? 1.54f : 1.2f;
            float height = SlowSwing ? Height * 1.2f : 1f;
            //if (HJScarletMethods.HasFuckingCalamity)
            //{
            //    xScale *= 1.14f;
            //    height *= 1.14f;
            //}
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(xScale, height, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            float heldscale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldscale;
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
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(xScale, height, 1f);
                float xScale2 = SlowSwing ? 1.7f : 1.7f;
                float lenght = 200f;
                //if(HJScarletMethods.HasFuckingCalamity)
                //{
                //    xScale2 *= 1.14f;
                //    lenght *= 1.14f;
                //}
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * xScale2;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * lenght * heldscale;
                OldAimPos.Add(slashPosFinal);
                for (int i = 1; i <= 2; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + slashTargetPos.RotatedBy(TargetRotation) * 180, Main.rand.NextFloat(.60f, .70f));
                    if (i == 2)
                        pos = Vector2.Lerp(Projectile.Center, Projectile.Center + slashTargetPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.50f, .98f));
                    float scale = .64f * Main.rand.NextFloat(0.8f, 1.3f);

                    Vector2 dir = (pos - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * Main.rand.NextFloat(0.1f, 1.5f) + dir * Main.rand.NextFloat(0.1f, 44f);
                    ECSParticle.SnowCloud(pos, vel * .05f, RandLerpColor(Color.Lerp(Color.SkyBlue, Color.WhiteSmoke, 0.5f), Color.RoyalBlue), 20, RandRotTwoPi, .150f + 0.050f * i, scale * (0.50f + i * 0.2f));
                }
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + slashTargetPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.01f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(12f, 20.5f);
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1f, .1f * Projectile.scale * Main.rand.NextFloat(.8f, 1.1f), glowMult: .51f);
                }
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + slashTargetPos.RotatedBy(TargetRotation) * 200, Main.rand.NextFloat(.01f, .98f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(5f, 9f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 30, 1f, Main.rand.NextFloat(.7f, 1.01f) * Projectile.scale * .75f, 0.2f);
                }
            }
        }
        public void UpdateMidAnimation()
        {
            Helper.UpdateAniState(1);
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            if (SlowSwing)
                easedProgress = EaseInCubic(Helper.GetAniProgress(1));
            float beginAngle = 125 * Flip.ToDirectionInt();
            float endAngle = 165 * Flip.ToDirectionInt();
            if (SlowSwing)
            {
                beginAngle = 165 * Flip.ToDirectionInt();
                endAngle = 170 * Flip.ToDirectionInt();
            }
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            float heldscale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float xScale = SlowSwing ? 1.54f : 1.2f;
            float height = SlowSwing ? Height * 1.2f : 1f;
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(xScale, height, 1f);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldscale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            float lerp = SlowSwing ? 0f : 0.1f;
            SlashOpacity = Lerp(SlashOpacity, 0f, lerp / Projectile.extraUpdates);
            if (SlashOpacity < 0.02f)
                SlashOpacity = 0;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .2f);
        }


        public void UpdateEndAnimation()
        {
            float lerp = FinalSwing ? 0.12f : 0.03f;
            SlashOpacity = Lerp(SlashOpacity, 0f, lerp / Projectile.extraUpdates);
            if (SlashOpacity < 0.02f)
                SlashOpacity = 0;

            float easedProgress = EaseInCubic(Helper.GetAniProgress(2));
            float beginAngle = 185 * Flip.ToDirectionInt();
            float endAngle = 165 * Flip.ToDirectionInt();
            if (SlowSwing)
            {
                beginAngle = 170 * Flip.ToDirectionInt();
                endAngle = 175 * Flip.ToDirectionInt();
            }
            float height = SlowSwing ? Height * 1.2f : 1f;
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            float scale = SlowSwing ? 1.54f : 1.2f;
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(scale, height, 1);
            float heldscale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldscale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Helper.UpdateAniState(2);
        }

        public void HandleHeldProjState()
        {
            Projectile.Center = Owner.MountedCenter;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            //Owner.ChangeDir(Projectile.direction);
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (FinalSwing)
            {
                modifiers.FinalDamage *= 1.3f;
                modifiers.SetCrit();
            }
            else
            {
                modifiers.SourceDamage *= 1.2f;
            }
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
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            if (SlowSwing)
                mult *= 0.92f;
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * (mult * 1f) + Projectile.Center - Main.screenPosition;
                Vertexlist.Add(new ScarletVertex(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new ScarletVertex(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
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
            SB.EnterShaderArea();
            tex = HJScarletTexture.Particle_CrossGlow.Value;
            float easedProgress = (Helper.GetAniProgress(2));
            float progress = Clamp(Lerp(0.91f, 0f, easedProgress), 0f, .91f);
            SB.Draw(tex, realDrawPos + Projectile.SafeDirByRot() * 60f * Projectile.scale, null, Color.RoyalBlue, ToRadians(0f), tex.ToOrigin(), Projectile.scale * 0.31f * .91f * progress, se, 0);
            SB.Draw(tex, realDrawPos + Projectile.SafeDirByRot() * 60f * Projectile.scale, null, Color.White, ToRadians(0f), tex.ToOrigin(), Projectile.scale * 0.28f * .91f * progress, se, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();
        }
    }
}
