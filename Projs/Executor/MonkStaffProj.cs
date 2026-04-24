using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Configuration;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MonkStaffProj : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        private float TargetRotation = 0;
        private Vector2 HeldPos;
        private Vector2 InitVector;
        public AnimationStruct Helper = new(3);
        public bool ProjType
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public float OpacityGlow = 0f;
        public float ScaleGlow = 0f;
        public Vector2 GlowPosCenter = Vector2.Zero;
        public float RotPointMultipler = 1f;
        public int BeginAngle = 30;
        public int EndAngle = 140;
        public int FinalAngle = 140;
        public int FinalAngleBefore = 600;
        public float GlowPosOffset => ProjType ? 70f : 120f;
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 60;
            Helper.MaxProgress[1] = 110;
            Helper.MaxProgress[2] = 20;
            InitVector = Owner.direction  > 0 ? PiOver4.ToRotationVector2() : (PiOver4 + PiOver2).ToRotationVector2();
            TargetRotation = InitVector.ToRotation();
            Projectile.rotation = TargetRotation;
            BeginAngle = Main.rand.Next(-160, -140);
            if(!ProjType)
            BeginAngle = Main.rand.Next(-100, -80);
            EndAngle = BeginAngle - Main.rand.Next(60, 80);
            FinalAngleBefore = Main.rand.Next(520, 610);
            FinalAngle = FinalAngleBefore + Main.rand.Next(10, 31);
        }
        public override bool ShouldUpdatePosition() => false;
        public override void ProjAI()
        {
            if (CheckOwnerDead())
            {
                Projectile.Kill();
                return;
            }
            UpdateProj();
            UpdatePlayerState();

        }

        public void UpdateProj()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset * RotPointMultipler;
            Lighting.AddLight(mountedPos, ProjType ? Color.GreenYellow.ToVector3() : Color.AliceBlue.ToVector3());
            if (!Helper.IsDone[0])
            {
                if(Helper.GetAniProgress(0) == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { MaxInstances = 0 , Pitch = 0.5f}, Owner.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { MaxInstances = 0 , Pitch = 0.5f}, Owner.Center);
                }
                Helper.UpdateAniState(0);
                UpdateBeginAni();
            }
            else if (!Helper.IsDone[1])
            {
                if (Helper.GetAniProgress(1) == 0)
                {
                    SetUpInitParticle();
                    Owner.HJScarlet().monkStaffHeal = true;
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { MaxInstances = 0}, Owner.Center);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { MaxInstances = 0}, Owner.Center);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { MaxInstances = 0}, Owner.Center);
                }
                Helper.UpdateAniState(1);
                UpdateMidAni();

            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.GetAniProgress(2) == 0)
                {
                }
                Helper.UpdateAniState(2);
                UpdateEndAni();
            }
            else
                Projectile.Kill();
        }

        public void SetUpInitParticle()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset  * RotPointMultipler;
            for (int i = 0; i < 40; i++)
            {
                Color starColor = ProjType ? RandLerpColor(Color.Green, Color.DarkGreen) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
                new ShinyCrossStar(mountedPos, RandVelTwoPi(0.8f,9.7f), starColor, 40, 0, 1, 0.75f, false).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                Color starColor = ProjType ? Color.Lime : Color.Aqua;
                new StarShape(mountedPos.ToRandCirclePosEdge(4f), RandVelTwoPi(1.2f, 9.6f), starColor, 0.84f, 60).Spawn();
            }
            for (int i = 0; i < 18; i++)
            {
                Color starColor = ProjType ? RandLerpColor(Color.LawnGreen, Color.LimeGreen) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
                new SmokeParticle(mountedPos.ToRandCirclePosEdge(20f), RandVelTwoPi(5.2f, 6.4f), starColor, 60, RandRotTwoPi, 1f, 0.37f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }
            if(!ProjType)
            {
                new LightningParticle(mountedPos, Vector2.Zero, Color.DeepSkyBlue, 60, RandRotTwoPi, 0.568f, 2).Spawn();
                new LightningParticle(mountedPos, Vector2.Zero, Color.SkyBlue, 60, RandRotTwoPi, 0.568f, 2).Spawn();
                new LightningParticle(mountedPos, Vector2.Zero, Color.RoyalBlue, 60, RandRotTwoPi, 0.568f, 2).Spawn();
            }
            new CrossGlow(mountedPos, ProjType ? Color.DarkGreen : Color.DeepSkyBlue, 40, 1, 0.38f).Spawn();
            new CrossGlow(mountedPos, ProjType? Color.Green : Color.Aqua, 40, 1, 0.35f).Spawn();
            new CrossGlow(mountedPos, Color.White, 40, 1, 0.30f).Spawn();
        }

        public void UpdateEndAni()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(2));
            OpacityGlow = ScaleGlow = Lerp(OpacityGlow,0f, 0.21f);
            OpacityGlow = ScaleGlow = (1 - easedProgress);
            float curRot = Helper.UpdateAngle(FinalAngleBefore, FinalAngle, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * (1 -easedProgress);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }
        int healAmt = 0;
        public void UpdateHealParticle()
        {
            Projectile.frameCounter += 1;
            foreach (var Player in Main.ActivePlayers)
            {
                if (Player.team != Owner.team)
                    continue;
                if (Projectile.frameCounter % 5 == 0)
                    Player.Heal(1);
                Vector2 pos = Player.Center + Vector2.UnitY * (Player.height * 0.5f);
                if (Main.rand.NextBool())
                {
                    pos.X += Main.rand.NextFloat(-1f, 1.1f) * Player.width;
                    pos.Y -= Main.rand.NextFloat(0f, 1f) * Player.height;
                    new StarShape(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.4f), Color.Lime, 0.4f, 40).Spawn();
                }
                if (Main.rand.NextBool())
                {
                    pos = Player.Center + Vector2.UnitY * (Player.height * 0.5f);
                    pos.X += Main.rand.NextFloat(-1f, 1.1f) * Player.width;
                    pos.Y -= Main.rand.NextFloat(0f, 1f) * Player.height;
                    new ShinyCrossStar(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, .4f), RandLerpColor(Color.Lime, Color.LimeGreen), 40, 0, 1, 0.4f, false).Spawn();
                }

            }
        }   

        public void UpdateMidAni()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
            float curRot = Helper.UpdateAngle(EndAngle, FinalAngleBefore, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            UpdateMidParticle();
            UpdateHealParticle();

        }

        public void UpdateMidParticle()
        {

            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset * RotPointMultipler+ RandVelTwoPi(5f, 45);
            Vector2 vel = RandVelTwoPi(1.2f, 1.6f);
            Color starColor = ProjType ? RandLerpColor(Color.DarkGreen, Color.Green) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
            if(Main.rand.NextBool())
            new ShinyCrossStar(mountedPos, vel, starColor, 40, 0, 1, 0.60f, false).Spawn();
            mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset * RotPointMultipler + RandVelTwoPi(5f, 45);
            vel = RandVelTwoPi(1.2f, 1.6f);
            starColor = ProjType ? RandLerpColor(Color.Lime, Color.DarkGreen) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
            if(!ProjType)
            {
                if (Main.rand.NextBool())
                {
                    new LightningParticle(mountedPos, Vector2.Zero, Color.DeepSkyBlue, 60, Projectile.rotation, 0.158f, 2).Spawn();
                    new LightningParticle(mountedPos, Vector2.Zero, Color.SkyBlue, 60, Projectile.rotation + RandRotTwoPi, 0.138f, 2).Spawn();
                    new LightningParticle(mountedPos, Vector2.Zero, Color.White, 60, Projectile.rotation + RandRotTwoPi, 0.128f, 2).Spawn();
                }
            }
            if (Main.rand.NextBool())
            {
                new HRShinyOrb(mountedPos, vel, starColor, 40, 0.08f).Spawn();
                new HRShinyOrb(mountedPos, vel, Color.White, 40, 0.08f * 0.5f).Spawn();
            }
        }
        public float FrontArmRot = 0;
        public void UpdateBeginAni()
        {
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            OpacityGlow = ScaleGlow = Lerp(OpacityGlow,1.1f,0.2f);
            float curRot = Helper.UpdateAngle(BeginAngle, EndAngle, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1) * easedProgress;
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            FrontArmRot = Projectile.rotation;
            Projectile.scale = tarPos.Length();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset * RotPointMultipler;
            Vector2 pos = mountedPos + RandVelTwoPi(100f, 190f);
            Vector2 vel = (mountedPos - pos).ToSafeNormalize() * Main.rand.NextFloat(3.2f, 8.4f);
            Color starColor = ProjType ? RandLerpColor(Color.Lime, Color.DarkGreen) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
                new StarShape(pos, vel, starColor, 0.75f, 40).Spawn();
            mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset * RotPointMultipler;
            pos = mountedPos + RandVelTwoPi(100f, 190f);
            vel = (mountedPos - pos).ToSafeNormalize() * Main.rand.NextFloat(2.2f, 8.4f);
            starColor = ProjType ? RandLerpColor(Color.Green, Color.DarkGreen) : RandLerpColor(Color.DeepSkyBlue, Color.Aqua);
                new ShinyCrossStar(pos, vel, starColor, 40, 0, 1, 0.5f, false).Spawn();

        }

        public bool CheckOwnerDead()
        {
            if (Owner.dead || !Owner.active || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return true;
            }
            return false;
        }

        private void UpdatePlayerState()
        {
            Projectile.Center = Owner.Center + HeldPos;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.direction =  Projectile.spriteDirection = Owner.direction;
            Owner.ControlPlayerArm(Projectile.rotation,-1);
            Owner.ControlPlayerArm(FrontArmRot + TwoPi - Pi,1);
            GlowPosCenter = HeldPos;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = ProjType ? TextureAssets.Projectile[ProjectileID.MonkStaffT1].Value : TextureAssets.Projectile[ProjectileID.MonkStaffT3].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rpValue = 20 * RotPointMultipler;
            Vector2 rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(projTex.Width - rpValue, projTex.Height - rpValue) : new Vector2(rpValue, projTex.Height - rpValue);
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);

            SB.Draw(projTex, drawPos, null, Color.White, drawRotation, rotationPoint, Projectile.scale * 1.4f, flipSprite, 0);
            Vector2 pos = Projectile.Center  + Projectile.rotation.ToRotationVector2() * (GlowPosOffset * RotPointMultipler* Projectile.scale)- Main.screenPosition;
            SB.EnterShaderArea();
            Texture2D kiraStar = HJScarletTexture.Particle_CrossGlow.Value;
            Texture2D ring = HJScarletTexture.Particle_RingHard.Value;
            float opValue = Projectile.Opacity * OpacityGlow;
            float scaleValue = Projectile.scale * ScaleGlow;
            SB.Draw(kiraStar, pos, null, ProjType ? (Color.DarkGreen) : Color.DeepSkyBlue * opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.55f, 0, 0);
            SB.Draw(kiraStar, pos, null, ProjType ?  Color.Green : Color.SkyBlue* opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.35f, 0, 0);
            SB.Draw(kiraStar, pos, null, ProjType ? Color.GreenYellow : Color.Aqua* opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.20f, 0, 0);
            SB.EndShaderArea();


            return false;
        }
    }
}
