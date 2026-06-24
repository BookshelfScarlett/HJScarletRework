using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
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
    public class FrostlightHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<Frostlight>().Texture;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public int Flip = 1;
        public AnimationStruct Helper = new AnimationStruct(3);
        public override int OriginalItemID => ItemType<Frostlight>();
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public List<Vector2> OldAimPos = [];
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = (int)(AttackSpeed * 1f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * 1.2f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * 0.3f);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .82f });
            SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { MaxInstances = 0, Pitch = .82f });
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            //Projectile.velocity = Projectile.SafeDir();
            HandleHeldProjState();
            HandleAttackAnimation();
            HandlePlayerState();
            if (OldAimPos.Count > 60)
                OldAimPos.RemoveAt(0);
        }

        public void HandleHeldProjState()
        {
            Owner.heldProj = Projectile.owner;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

        }
        public void HandleAttackAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                UpdateMidAnimatio();
                Helper.UpdateAniState(1);
            }
            else
            {
                if (!Owner.HasProj<FrostlightHeldProjAlt>(out int projID) && Main.mouseLeft)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, projID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.rotation = Projectile.rotation;
                    proj.ai[0] = 114514;
                }
                Projectile.Kill();
            }
        }

        public void UpdateMidAnimatio()
        {
            //这里挥砍动画一定程度上使用了矩阵变化。
            if (Helper.GetAniProgress(1) == 0)
            {
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 8f, 30, Projectile.rotation);
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_LightRelease with { MaxInstances = 1, Pitch = -.2f });
                Vector2 dir2 = Projectile.rotation.ToRotationVector2();
                Vector2 posBase = Projectile.Center + dir2 * 65f * Projectile.scale;
                for (int i = 0; i < 30; i++)
                {
                    Color color = RandLerpColor(RandLerpColor(Color.RoyalBlue, Color.LightBlue), Color.WhiteSmoke);
                    Vector2 smokeVel = RandVelTwoPi(1.4f, 34f);
                    ECSParticle.SmokeParticle(posBase, smokeVel, color, Main.rand.Next(25, 30), RandRotTwoPi, .70f, Main.rand.NextFloat(.9f, 1.1f) * .35f, true,BlendState.Additive);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = posBase + RandVelTwoPi(0f, 80f);
                    Vector2 vel = RandVelTwoPi(1.2f, 30.4f);
                    float scale = Main.rand.NextFloat(0.4f, 0.9f) * .20f;
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.Next(30, 40), 1f, scale, .75f);
                }
                for (int i = 0; i < 30; i++)
                {
                    new TurbulenceGlowOrb(posBase.ToRandCirclePos(30), Main.rand.NextFloat(1.2f, 4.2f) * 1.2f, RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.Next(60, 120), Main.rand.NextFloat(.7f, .9f) * .3f, RandRotTwoPi, true).SpawnToPriority();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 nextPos = posBase + RandVelTwoPi(10f, 30f);
                    new StarShape(nextPos, (nextPos - posBase).ToRandVelocity(ToRadians(10f), 8f, 18f), RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.NextFloat(0.95f, 1.27f) * 1.01f, 45, true).Spawn();
                }
                //HandleMidParticleInit();
            }
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            //末尾角度，也是下一个动画进程的起始角度
            float endAngle = 160f * Flip;
            float beginAngle = -215f * Flip;
            //更新当前的转角
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //将其投影到矩阵上，并进行形变
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, 1, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f;
            //这样，Scale就是tarPos的向量模长
            Projectile.scale = tarPos.Length();
            //武器的角度为（起始角度 + 目标角度）的值
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

            //下面基本上是粒子生成了。
            float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1f, 1f, 1f);
            Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.1f;
            Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 65f;
            OldAimPos.Add(slashPosFinal);
            //最后，粒子效果
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            if (Main.rand.NextBool(5))
            {
                //这里代码基本上是没法省的，不用看了
                //这里基本上是为了对准法杖中心的位置，直接的硬编码
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-30f, 30f);
                if (Main.rand.NextBool(3))
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    new Fire(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 40), RandRotTwoPi, 0.47f, Main.rand.NextFloat(.9f, 1.1f) * .25f).SpawnToPriority();
                }
                else
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    ECSParticle.SmokeParticle(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 40), RandRotTwoPi, 0.47f, Main.rand.NextFloat(.9f, 1.1f) * .45f, Main.rand.NextBool(), BlendState.Additive);
                }

            }
            if (Main.rand.NextBool(4))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                Vector2 setPos = posBase + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-60f, 60f);
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(4f, 12f);
                //计算水平偏移量
                float offsetX = setPos.X - posBase.X;
                //判断是否需要镜像
                bool needMirror = (offsetX > 0 && vel.X < 0) || (offsetX < 0 && vel.X > 0);
                if (needMirror)
                {
                    //镜像：偏移量取反，保持绝对距离一致
                    setPos.X = posBase.X - offsetX;
                }
                //这样，我们就能确保火星永远向外展开
                ECSParticle.ShinyCrossStarECS(setPos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.Next(30, 60), 1f, Main.rand.NextFloat(.8f, 1.1f) * .48f, .2f);
            }
        }

        public void UpdateBeginAnimation()
        {
            if(Helper.OnAnimationBegin(0))
            {
            }
            //这里挥砍动画一定程度上使用了矩阵变化。
            float easedProgress = EaseOutBack(Helper.GetAniProgress(0));
            //末尾角度，也是下一个动画进程的起始角度
            float endAngle = -215f * Flip;
            float beginAngle = -105f * Flip;
            //更新当前的转角
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //将其投影到矩阵上，并进行形变
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, 1, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f;
            //这样，Scale就是tarPos的向量模长
            Projectile.scale = tarPos.Length();
            //武器的角度为（起始角度 + 目标角度）的值
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            //最后。粒子效果的处理
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale;
            Vector2 starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(120f, 190f);
            Vector2 starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
            if (Main.rand.NextBool(7))
            {
                Vector2 vel = starShapeDir * Main.rand.NextFloat(4f, 9);
                float scale = Main.rand.NextFloat(0.7f, 0.91f) * Projectile.scale;
                new StarShape(starShapeSpawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), scale, 40, true).Spawn();
            }
            if (Main.rand.NextBool(5))
            {
                starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(120f, 190f);
                starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
                ECSParticle.ShinyCrossStarECS(starShapeSpawnPos, starShapeDir * Main.rand.NextFloat(.8f, 12f), RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.Next(40, 60), 1, 0.90f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
            }
            if (Main.rand.NextFloat() > Helper.GetAniProgress(0))
                return;

            if (Main.rand.NextBool(3))
            {
                posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-30f, 30f);
                if (Main.rand.NextBool(3))
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    new Fire(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .25f).SpawnToPriority();
                }
                else
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    ECSParticle.SmokeParticle(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .45f, Main.rand.NextBool(), BlendState.Additive);
                }

            }
        }

        public void HandlePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ControlPlayerArm(Projectile.rotation);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + PiOver4 + (Projectile.spriteDirection == -1 ? PiOver2 : 0);
            Vector2 origin = new Vector2(Projectile.spriteDirection == -1 ? tex.Width : 0, tex.Height);
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -25f + Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(PiOver2) * 0;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, realDrawPos, null, Color.White, rotation, origin, Projectile.scale, se, 0);

            SB.EnterShaderArea();


            if (OldAimPos.Count > 0)
            {
                Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
                Effect effect = HJScarletShader.AlphaFade;
                effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
                effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
                effect.Parameters["uFadeinTopLength"].SetValue(0.11f);
                effect.Parameters["uFadeinBottomLength"].SetValue(0.11f);
                effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
                effect.CurrentTechnique.Passes[0].Apply();
                DrawSlash(texture, Color.RoyalBlue * 0.80f, 0.7f);
                DrawSlash(texture, Color.LightBlue * 0.36f, 0.4f);
                DrawSlash(texture, Color.DarkBlue * 0.40f, 0f);


                Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
                effect2.Parameters["uFadeoutLeftLength"].SetValue(0.32f);
                effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
                effect2.Parameters["uFadeinTopLength"].SetValue(0.11f);
                effect2.Parameters["uFadeinBottomLength"].SetValue(0.11f);
                effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 1.95f, 0));
                effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
                effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                effect2.CurrentTechnique.Passes[0].Apply();
                Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
                DrawSlash(texture2, Color.RoyalBlue * .75f, 0.60f);
                texture2 = HJScarletTexture.Noise_Aura.Value;
                DrawSlash(texture2, Color.WhiteSmoke * .70f, 0.61f);

                Texture2D slashTex = HJScarletTexture.Texture_SwordSlash.Value;
                Effect shader = HJScarletShader.AlphaFadeNoiseColor;
                shader.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
                shader.Parameters["uFadeinRigtLength"].SetValue(0.1f);
                shader.Parameters["uFadeinTopLength"].SetValue(0.11f);
                shader.Parameters["uFadeinBottomLength"].SetValue(0.11f);
                shader.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0f, 0));
                shader.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
                shader.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                shader.CurrentTechnique.Passes[0].Apply();
                DrawSlash(slashTex, Color.White * .35f, 0.60f);
                DrawSlash(slashTex, Color.RoyalBlue * .35f, 0.61f);

                SB.EnterShaderArea();
            }

            Texture2D star = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            Vector2 pos = drawPos + dir * 60f * Projectile.scale;
            float scale = Projectile.scale * .285f * easedProgress;
            SB.Draw(star, pos, null, Color.RoyalBlue * .9f * easedProgress, 0, star.ToOrigin(), scale * 0.95f, 0, 0);
            SB.Draw(star, pos, null, Color.SkyBlue * .9f * easedProgress, 0, star.ToOrigin(), scale * .90f, 0, 0);
            SB.Draw(star, pos, null, Color.LightBlue * .85f *easedProgress, 0, star.ToOrigin(), scale * 0.85f, 0, 0);


            //出于我不清楚的原因，这里得重置两次批次，才能确保正常
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
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
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor , new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
    }
}
