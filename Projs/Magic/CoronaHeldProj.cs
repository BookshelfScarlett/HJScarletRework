using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Magic
{
    public class CoronaHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<Corona>().Texture;
        public override ClassCategory Category => ClassCategory.Magic;
        public int UseTime = -1;
        public bool ShouldPlaySound = false;
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetUpHeldProj();
            Projectile.scale = 1;
            Projectile.Opacity = 0;
        }

        public override void OnFirstFrame()
        {
            float angleToWhat = (Projectile.Center - Owner.Center).ToRotation();
            Projectile.rotation = angleToWhat;
        }
        public float Oscillation = 0;
        public void HandleAppearAndDisapper()
        {
            //用这个来简单控制一下声音
            if (DrawRatios > 0.05f && DrawRatios < 0.2f)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = 0.25f }, Owner.Center);
            }
            bool isHoldingWeapon = Owner.HeldItem.type == ItemType<Corona>();
            if (isHoldingWeapon)
            {
                Projectile.timeLeft = GetSeconds(2);
                Projectile.Opacity = Projectile.Opacity <= 0.98f ? Lerp(Projectile.Opacity, 1.01f, 0.2f) : 1f;
                DrawRatios = Projectile.Opacity;
            }
            else
            {
                if (Projectile.Opacity <= 0.024f)
                {
                    Projectile.Kill();
                    return;
                }
                else
                {
                    if (Projectile.Opacity == 1f)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30f) - Vector2.UnitY * 15f, Vector2.UnitY.ToRandVelocity(ToRadians(10f), 4.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            new SmokeParticle(Projectile.Center.ToRandCirclePos(30f) - Vector2.UnitY * 15f, Vector2.UnitY.ToRandVelocity(ToRadians(10f), 4.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriority();
                        }
                    }
                    DrawRatios *= Projectile.Opacity;
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.20f);
                }
            }
        }
        public void HandlePosition()
        {
            Oscillation += ToRadians(3f);
            if (Oscillation > ToRadians(360f))
                Oscillation = ToRadians(-360f);
            Timer++;
            if (Timer > 30f)
                Timer = 30f;
            //锤子应当朝向的位置
            float anchorPosX = Owner.MountedCenter.X;
            float anchorPosY = Owner.MountedCenter.Y - (50f * MathF.Sin(Oscillation) / 9f) - 70f * Projectile.Opacity;
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            float lerpValue = 0.15f + (Timer < 30f).ToInt() * 0.2f;
            if(Owner.controlUseTile)
            {
                anchorPos = Owner.Center;
                lerpValue = .15f;
            }
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, lerpValue);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = (Projectile.Center - Owner.Center).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.48f);

        }

        public override void ProjAI()
        {
            HandleAppearAndDisapper();
            HandlePosition();
            HandleAttack();
        }
        public float DrawRatios = 0;
        public bool GrowUp = true;
        public bool PlaySound = false;
        public void HandleAttack()
        {
            if (Projectile.Opacity != 1f)
                return;
            int attackSpeed = 30;
            float useTimeRatios = Clamp(UseTime / (float)attackSpeed - 0.2f, 0f, 1f);
            if (!IsUsing)
            {
                if (!ShouldPlaySound)
                {
                }
                ShouldPlaySound = true;
                if (UseTime > 0)
                    UseTime--;
                return;
            }
            else
            {
                if(ShouldPlaySound)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = 0.35f }, Owner.Center);
                    ShouldPlaySound = false;
                }
                Vector2 particlePos = Projectile.Center.ToRandCirclePos(40f * DrawRatios);
                if (Main.rand.NextBool(3))
                    new SmokeParticle(particlePos, Vector2.UnitY * -Main.rand.NextFloat(6f, 8f) * 0.91f, RandLerpColor(Color.Lerp(Color.OrangeRed, Color.DarkOrange, 0.5f), Color.Lerp(Color.Black, Color.OrangeRed, 0.6f)), 40, RandRotTwoPi, 1f, 0.36f, true).SpawnToPriority();
            }
            HandleOwnerArms();
            Item item = Owner.HeldItem;
            UseTime++;
            if (Collision.SolidTiles(Projectile.Center, Projectile.width, Projectile.height))
                return;
            if (UseTime < attackSpeed)
                return;
            if (!Owner.CheckMana(Owner.HeldItem, (int)(Owner.HeldItem.mana * Owner.manaCost),true,false))
                return;
            
            Vector2 vel = Vector2.UnitX;
            for (int i = -1; i < 2; i += 2)
            {
                for (int j = 0; j < 26; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        new StarShape(Projectile.Center.ToRandCirclePosEdge(3), vel.ToRandVelocity(0, -1f, 9.6f).RotatedBy(k * PiOver2) * i, RandLerpColor(Color.Orange, Color.OrangeRed), 0.8f, 40).Spawn();
                        new StarShape(Projectile.Center.ToRandCirclePosEdge(3), vel.ToRandVelocity(0, -1f, 9.6f).RotatedBy(PiOver4 + k * PiOver2) * i, RandLerpColor(Color.Orange, Color.OrangeRed), 0.8f, 40).Spawn();
                    }
                }
            }
            //crossStar，在周围生成
            for (int i = 0; i < 50; i++)
            {
                Vector2 spawnPos2 = Projectile.Center.ToRandCirclePos(100);
                Vector2 dir = (spawnPos2 - Projectile.Center).ToSafeNormalize();
                new ShinyCrossStar(Projectile.Center + dir * Main.rand.NextFloat(5f), dir * Main.rand.NextFloat(4f, 8f), RandLerpColor(Color.Orange, Color.OrangeRed), 40, RandRotTwoPi, 1f, Projectile.scale, false).Spawn();
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_MagicStaffFire with { MaxInstances = 0, Pitch = 0.7f, Volume = 0.30f });
            Vector2 dir2 = (-(Projectile.Center - Main.MouseWorld)).ToSafeNormalize();
            for (int i = 0; i < 7; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(50);
                new StarShape(Projectile.Center.ToRandCirclePos(3f), dir2.ToRandVelocity(ToRadians(5f), 6f, 9f) * 0.85f, RandLerpColor(Color.OrangeRed, Color.Orange), Projectile.scale * 1.2f, 40).Spawn();
            }
            for (int i = 0; i < 8; i++)
            {
                dir2 = Vector2.UnitX.RotatedBy((PiOver4 * i) + Main.rand.NextFloat(ToRadians(-15f), ToRadians(15f)));
                Projectile proj1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir2 * 7f, ProjectileType<CoronaFireball>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
            }
            UseTime = 0;

        }

        private void HandleOwnerArms()
        {
            if (Owner.direction > 0)
                Owner.ControlPlayerArm(-PiOver2 * DrawRatios);
            else
                Owner.ControlPlayerArm(Lerp(-Pi - PiOver4, -PiOver2, DrawRatios));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + PiOver2 + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color endColor = Color.Lerp(Color.White, Color.DarkGoldenrod, 0.5f);
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 2f * DrawRatios, null, Color.White.ToAddColor() * DrawRatios * Projectile.Opacity, drawRot, rotationPoint, Projectile.scale, flipSprite, default);
            }
            SB.Draw(tex, drawPos, null, Color.White * Projectile.Opacity, drawRot, rotationPoint, Projectile.scale, flipSprite, default);
            tex = HJScarletTexture.Particle_RingShiny.Value;
            float ratiosReal = Clamp(DrawRatios, 0f, 1) * 0.5f;
            SB.Draw(tex, drawPos, null, Color.Black * DrawRatios, Pi, tex.ToOrigin(), ratiosReal * 0.4f, 0, 0);
            SB.EnterShaderArea();
            SB.Draw(tex, drawPos, null, Color.OrangeRed * DrawRatios, 0, tex.ToOrigin(), ratiosReal * 0.4f, 0, 0);
            SB.Draw(tex, drawPos, null, Color.WhiteSmoke * DrawRatios, Pi, tex.ToOrigin(), ratiosReal * 0.4f, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
