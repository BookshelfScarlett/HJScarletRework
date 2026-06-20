using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            SoundEngine.PlaySound(HJScarletSounds.TheMars_Toss with { Pitch= -.5f});
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.SafeDir();
            HandleHeldProjState();
            HandleAttackAnimation();
            HandlePlayerState();
        }

        public void HandleHeldProjState()
        {
            Owner.heldProj = Projectile.owner;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.Center = Owner.MountedCenter;
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
                SoundEngine.PlaySound(HJScarletSounds.Misc_AirCharge with { MaxInstances = 1, Pitch = -.2f });
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
            //最后，粒子效果
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            if (Main.rand.NextBool(5))
            {
                //这里代码基本上是没法省的，不用看了
                //这里基本上是为了对准法杖中心的位置，直接的硬编码
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale;
                posBase += dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-15f, 16f);
                new SmokeParticle(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), RandLerpColor(RandLerpColor(Color.RoyalBlue, Color.LightBlue), Color.WhiteSmoke), Main.rand.Next(35, 50), RandRotTwoPi, 0.5f, Main.rand.NextFloat(.9f, 1.1f) * .45f, true).SpawnToPriority();
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + Main.rand.NextVector2Circular(30,30);
                posBase += dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-60f, 60f);
                Dust d = Dust.NewDustPerfect(posBase, DustID.IceTorch);
                d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                d.velocity = -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f);
                d.noGravity = true;
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
                new ShinyCrossStar(setPos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, vel.ToRotation(), 1f, Main.rand.NextFloat(0.8f, 1.2f) * 0.8f, Main.rand.NextBool()).Spawn();
            }
            //HandleMidParticle();
            //HandlePlayerHealing();
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
                new ShinyCrossStar(starShapeSpawnPos, starShapeDir * Main.rand.NextFloat(0.8f, 8f), Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), 40, 0, 1, 0.8f * Main.rand.NextFloat(0.8f, 1.1f), false).Spawn();
            }
            if (Main.rand.NextBool(3))
            {
                starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(100f, 160f);
                starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
                new SmokeParticle(starShapeSpawnPos, starShapeDir * Main.rand.NextFloat(2f, 12f), RandLerpColor(RandLerpColor(Color.RoyalBlue, Color.LightBlue), Color.White), 45, RandRotTwoPi, 0.35f, Main.rand.NextFloat(0.4f, 0.7f), true).SpawnToPriority();
            }
            //更新动画进程，封装的方法
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
            Texture2D star = HJScarletTexture.Particle_CrossGlow.Value;
            SB.EnterShaderArea();
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            Vector2 pos = drawPos + dir * 55f * Projectile.scale;
            float scale = Projectile.scale * .285f * easedProgress;
            SB.Draw(star, pos, null, Color.RoyalBlue* .9f, 0, star.ToOrigin(), scale * 0.95f, 0, 0);
            SB.Draw(star, pos, null, Color.DeepSkyBlue* .9f, 0, star.ToOrigin(), scale * .90f, 0, 0);
            SB.Draw(star, pos, null, Color.LightBlue* .85f, 0, star.ToOrigin(), scale * 0.85f, 0, 0);
            //出于我不清楚的原因，这里得重置两次批次，才能确保正常
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
        }
    }
}
