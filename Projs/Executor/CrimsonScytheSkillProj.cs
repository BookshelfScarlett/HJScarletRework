using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheSkillProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<CrimsonScythe>();
        public override string Texture => GetInstance<CrimsonScytheHeldProj>().Texture;
        public AnimationStruct Helper = new AnimationStruct(4);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1.15f;
        public float Width = 1.15f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public List<Vector2> OldAimPos = [];
        public float ArmRotation = 0;
        public float BeginArmRotation = 0;
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }

        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Owner.CheckExecution(OriginalItemID);
            Helper.MaxProgress[0] = (int)(AttackSpeed * .95f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .45f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * .6f);
            Helper.MaxProgress[3] = (int)(AttackSpeed * .95f);
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;
            BeginArmRotation = Owner.direction < 0 ? Pi : 0;
            ArmRotation = BeginArmRotation;
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center, 1, 1, 0.4f);

        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();

        }

        public void UpdateAnimation()
        {
            //TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                UpdateBeginAnimation();
            }
            else if (!Helper.IsDone[1])
            {
                Helper.UpdateAniState(1);
                UpdateMidAnimation();

            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.OnAnimationBegin(2))
                {
                    BreakTheStone();

                }
                Helper.UpdateAniState(2);
                UpdateEndAnimation();
            }
            else if (!Helper.IsDone[3])
            {
                Helper.UpdateAniState(3);
                UpdateFinalAnimation();

            }
            else
            {
                //这个音符只需要生成一个，明显用旧粒子系统制作起来速度更快，而且就一个东西能有什么性能问题？
                new MusicSymbol(Owner.Center - Vector2.UnitY * 40f, (-Vector2.UnitY).ToRandVelocity(ToRadians(25), 1f, 3f), Color.White, 90, 0, 1, 0.28f).Spawn();
                Projectile.Kill();
            }
        }
        public void BreakTheStone()
        {
            Owner.KillCertainProj(ProjectileType<CrimsonScytheSoulStone>());
            ScarletSound(HJScarletSounds.Tlipoca_StoneShatter, Projectile.Center, 0.475f, 1, -0.5f);
            Vector2 pos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, ArmRotation - PiOver2).ToRandCirclePos(4) + ArmRotation.ToRotationVector2() * 20f;
            ScreenShakeSystem.AddScreenShakes(Owner.Center, 3, 50, 0, RandRotTwoPi, easingFunc: EaseInOutQuad);
            ScreenDarknessSystem.AddScreenDarkness(.85f, 4, 1, 42, EaseInCubic, EaseInCubic);
            ECSParticle.CrossGlow(pos, Color.White, 45, 1, 0.94f, 0.2f);


            for (int i = 0; i < 46; i++)
            {
                ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePosEdge(10), Main.rand.NextFloat(0.8f, 1.15f) * 4f, Color.White, 140, 1, .1f);
            }
            for (int i = 0; i < 46; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(1.2f, 3.3f), Color.White, 120, 1, .71f);
            }
        }

        public void UpdateBeginAnimation()
        {
            float curAniPro = Helper.GetAniProgress(0);
            float pro = EaseOutCubic(curAniPro);
            float beginAngle = -180 * Flip.ToDirectionInt();
            float endAngle = 180 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

            //这里处理手臂的动画
            float armBeginAngle = -180 * Flip.ToDirectionInt();
            float armEndAngle = 0 * Flip.ToDirectionInt();
            float armRot = Helper.UpdateAngle(armBeginAngle, armEndAngle, Owner.direction, pro);
            Matrix armForm = Matrix.CreateRotationZ(armRot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 armTarPos = Vector2.Transform(Vector2.UnitX, armForm) * 1.5f;
            ArmRotation = armTarPos.ToRotation() + BeginArmRotation;

        }
        public void UpdateMidAnimation()
        {
            float curAniPro = Helper.GetAniProgress(1);
            float pro = EaseInOutExpo(curAniPro);
            float beginAngle = 180 * Flip.ToDirectionInt();
            float endAngle = 185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

            //这里处理手臂的动画
            float armBeginAngle = 0 * Flip.ToDirectionInt();
            float armEndAngle = 0 * Flip.ToDirectionInt();
            float armRot = Helper.UpdateAngle(armBeginAngle, armEndAngle, Owner.direction, pro);
            Matrix armForm = Matrix.CreateRotationZ(armRot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 armTarPos = Vector2.Transform(Vector2.UnitX, armForm) * 1.5f;
            ArmRotation = armTarPos.ToRotation() + BeginArmRotation;

        }
        public void UpdateEndAnimation()
        {
            float curAniPro = Helper.GetAniProgress(2);
            float pro = EaseInOutExpo(curAniPro);
            float beginAngle = 185 * Flip.ToDirectionInt();
            float endAngle = 183 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

            ////这里处理手臂的动画
            //float armBeginAngle = 0 * Flip.ToDirectionInt();
            //float armEndAngle = 15 * Flip.ToDirectionInt();
            //float armRot = Helper.UpdateAngle(armBeginAngle, armEndAngle, Owner.direction, pro);
            //Matrix armForm = Matrix.CreateRotationZ(armRot) * Matrix.CreateScale(Width, Height, 1);
            //Vector2 armTarPos = Vector2.Transform(Vector2.UnitX, armForm) * 1.5f;
            //ArmRotation = armTarPos.ToRotation() + BeginArmRotation;

        }
        public void UpdateFinalAnimation()
        {
            float curAniPro = Helper.GetAniProgress(2);
            float pro = EaseInOutExpo(curAniPro);
            float beginAngle = 183 * Flip.ToDirectionInt();
            float endAngle = 185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

            ////这里处理手臂的动画
            //float armBeginAngle = 15 * Flip.ToDirectionInt();
            //float armEndAngle = -15 * Flip.ToDirectionInt();
            //float armRot = Helper.UpdateAngle(armBeginAngle, armEndAngle, Owner.direction, pro);
            //Matrix armForm = Matrix.CreateRotationZ(armRot) * Matrix.CreateScale(Width, Height, 1);
            //Vector2 armTarPos = Vector2.Transform(Vector2.UnitX, armForm) * 1.5f;
            //ArmRotation = armTarPos.ToRotation() + BeginArmRotation;

        }

        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(ArmRotation, 1);
            Owner.ControlPlayerArm(Projectile.rotation, -1);

        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            if (Helper.Progress[2] <= 0)
            {
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
            }
            //Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }


        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override bool? CanDamage() => false;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            float mult = !Helper.IsDone[0] ? 1 - EaseOutExpo(Helper.GetAniProgress(0)) : EaseInCubic(Helper.GetAniProgress(3));
            float edgeMult = Helper.IsDone[2] ? 1 - EaseOutExpo(Helper.GetAniProgress(3)) : EaseOutBack(Helper.GetAniProgress(2));
            rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(tex.Width, tex.Height) - new Vector2(25) : new Vector2(0, tex.Height) - new Vector2(-25, 25);
            if (edgeMult >= 0.02f)
            {
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 1.5f * edgeMult, null, Color.Red.ToAddColor() * edgeMult, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
                }
            }
            HJScarletMethods.ApplyMeltShader(tex, Color.Red, mult);
            SB.Draw(tex, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            SB.EndShaderArea();
            if (Helper.IsDone[1])
                return false;
            SB.EnterShaderArea();
            Texture2D stone = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Vector2 pos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, ArmRotation - PiOver2);
            pos.Y += Owner.gfxOffY;
            Vector2 dir = ArmRotation.ToRotationVector2();
            Color c = Color.Lerp(Color.Transparent, Color.White, Helper.GetAniProgress(0));
            SB.Draw(stone, pos - Main.screenPosition + dir * 20f, null, c, 0, stone.Size() / 2, Projectile.scale * 0.4f, flipSprite, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
