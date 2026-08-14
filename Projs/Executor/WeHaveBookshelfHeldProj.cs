using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    internal class WeHaveBookshelfHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<WeHaveBookshelf>().Texture;
        public override int OriginalItemID => ItemType<WeHaveBookshelf>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1.5f;
        public float Width = 1.5f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public List<int> BookcaseList = 
            [
            ItemID.Bookcase, 
            ItemID.BorealWoodBookcase, 
            ItemID.EbonwoodBookcase, 
            ItemID.RichMahoganyBookcase, 
            ItemID.BambooBookcase, 
            ItemID.PalmWoodBookcase, 
            ItemID.ShadewoodBookcase
            ];
        public int RandomBookcase = -1;
        public int SwitchBookcase = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {

            Projectile.SetUpHeldProj(10);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.penetrate = 1;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            RandomBookcase = Main.rand.Next(0, BookcaseList.Count);
            ScarletSound(HJScarletSounds.TheSevenStar_Swing, Projectile.Center, 0.95f, 1, -0.34f, 0.1f, 1);
            if (SwingTime > 4)
                Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
            else
                Helper.MaxProgress[0] = (int)(AttackSpeed);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .5f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * .65f);
            TargetRotation = Owner.Center.ToMouseVector2().ToRotation();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            HandleExecution();
            if (Owner.HJScarlet().bookcaseBuffTime > 0 && Projectile.FinalUpdateNextBool(4))
            ECSParticle.LightntingGlow(Owner.ToRandRec(), -Vector2.UnitY, RandLerpColor(Color.LimeGreen, Color.GreenYellow), 40, 1, 0.4f);
        }
        public override void OnExecution()
        {
            Owner.HJScarlet().bookcaseBuffTime = GetSeconds(15);
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
            for(int i =0;i<8;i++)
            {
                ECSParticle.LightntingGlow(Owner.ToRandRec(), -Vector2.UnitY, RandLerpColor(Color.LimeGreen, Color.GreenYellow), 40, 1, 0.4f);
            }
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            {
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
                Owner.heldProj = Projectile.whoAmI;
                if (Owner.dead)
                    Projectile.Kill();
                else
                    Projectile.timeLeft = 2;
            }
        }
        public void UpdateAnimation()
        {
            if (StopTiming > 0)
            {
                StopTiming--;
                return;
            }
            UpdateHalfCircleSwingAnimation();
        }

        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                {
                    int count = 2;
                    if (Owner.HJScarlet().bookcaseBuffTime > 0)
                    {
                        count = 4;

                    }
                    for (int i = 0; i < count; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Owner.ToMouseVector2().ToRandVelocity(ToRadians(15f), 4.4f, 5.8f), ProjectileType<WeHaveBookshelfBook>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    }
                }
                UpdateBeginAnimation();
            }
            else
                Projectile.Kill();
        }
        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -195f * Flip.ToDirectionInt();
            float endAngle = 195f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.61f, .91f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                }
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .8f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                }
            }
        }
        public void UpdateEndAnimation()
        {

        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                ((WeHaveBookshelfHeldProj)proj.ModProjectile).TargetRotation = Projectile.rotation;
                ((WeHaveBookshelfHeldProj)proj.ModProjectile).Flip = !Flip;
                ((WeHaveBookshelfHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
                proj.HJScarlet().HasExecutionMechanic = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HJScarlet().bookcaseBuffTime == 0)
                Projectile.AddExecutionTimeImmediate(OriginalItemID);
                target.HJScarlet().StopNpcTime = 16;
            if (Projectile.numHits < 1)
            {
                StopTiming = 4 * Projectile.MaxUpdates;
                ScreenShakeSystem.AddScreenShakes(target.Center, 10f, 20, Projectile.Center.GetNormalVector2(target.Center).ToRotation(), 0);
                ScarletSound(HJScarletSounds.Misc_GunHit, Projectile.Center, .65f, 1, -.32f, .1f);
            }

            switch(RandomBookcase)
            {
                case 0:
                    QuickSetParticle(target, RandLerpColor(Color.Brown, Color.Orange), RandLerpColor(Color.Brown, Color.Orange), DustID.DesertTorch);
                    break;
                case 1:
                    QuickSetParticle(target, RandLerpColor(Color.AliceBlue, Color.RoyalBlue), RandLerpColor(Color.AliceBlue, Color.RoyalBlue), DustID.IceTorch);
                    break;
                case 2:
                    QuickSetParticle(target, RandLerpColor(Color.Green, Color.LimeGreen), RandLerpColor(Color.Green, Color.LimeGreen), DustID.CursedTorch);
                    break;
                case 3:
                    QuickSetParticle(target, RandLerpColor(Color.Pink, Color.HotPink), RandLerpColor(Color.IndianRed, Color.Pink), DustID.PinkTorch);
                    break;
                case 4:
                    QuickSetParticle(target, RandLerpColor(Color.DarkGreen, Color.LimeGreen), RandLerpColor(Color.DarkGreen, Color.LimeGreen), DustID.JungleTorch);
                    break;
                case 5:
                    QuickSetParticle(target, RandLerpColor(Color.Orange, Color.LightGoldenrodYellow), RandLerpColor(Color.Orange, Color.LightGoldenrodYellow), DustID.PalmWood);
                    break;
                case 6:
                    QuickSetParticle(target, RandLerpColor(Color.DarkRed, Color.Red), RandLerpColor(Color.DarkRed, Color.Crimson), DustID.CrimsonTorch);
                    break;
             }
        }
        public void QuickSetParticle(NPC target, Color smokeColor, Color starColor, int dType)
        {
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f), RandVelTwoPi(.3f, 14f), smokeColor, 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6), RandVelTwoPi(0.3f, 10.1f), starColor, 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                Dust d = Dust.NewDustPerfect(target.Center, dType);
                d.velocity = RandVelTwoPi(1.2f, 6.2f) * 3f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.2f, 1.61f);
            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 32;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            tex = GetVanillaAsset(Globals.Enums.VanillaAsset.Item,BookcaseList[RandomBookcase]);
            
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}
