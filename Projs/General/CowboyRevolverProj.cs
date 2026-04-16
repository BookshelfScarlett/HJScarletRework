using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class CowboyRevolverProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Typeless;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.Revolver);
        public NPC CurTarget = null;
        public AnimationStruct Helper = new(2);
        public bool IsShoot = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetUpHeldProj(0);
            Projectile.extraUpdates = 2;
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 60;
            Helper.MaxProgress[1] = 10;
        }
        public override void ProjAI()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.velocity *= 0.96f;
                Projectile.rotation += 0.15f * Projectile.spriteDirection;
                //这里会时刻更新目标是否合法，因为下面不会在试图搜索一遍目标了
                if(!CurTarget.IsLegal())
                {
                    if (Projectile.GetTargetSafe(out NPC target, canPassWall: true))
                        CurTarget = target;
                }
                if (Main.rand.NextBool())
                {
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(), RandVelTwoPi(0f, 1.2f), RandLerpColor(Color.Black, Color.Gray), 40, RandRotTwoPi, 1, 0.1f).SpawnToNonPreMult();
                }
            }
            else if (!Helper.IsDone[1])
            {
                //发射弹幕
                Projectile.velocity *= 0.96f;
                Helper.UpdateAniState(1);
                if (CurTarget.IsLegal())
                    Projectile.rotation = Projectile.rotation.AngleTowards((CurTarget.Center - Projectile.Center).ToRotation(), 0.15f);
                else
                {
                    //但凡目标不合法，立刻发射子弹
                    Helper.Progress[1] += 10;
                }

                if (Main.rand.NextBool())
                {
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(), RandVelTwoPi(0f, 1.2f), RandLerpColor(Color.Black, Color.Gray), 40, RandRotTwoPi, 1, 0.1f).SpawnToNonPreMult();
                }

            }
            else
            {
                //无论咋样都直接发射这个子弹。
                //如果目标单位合理我们才吧子弹强行校准，否则随机发射
                Vector2 vel = !CurTarget.IsLegal() ? Projectile.rotation.ToRotationVector2() * 18f : HJScarletMethods.PredictAimToTarget(Projectile.Center, CurTarget.Center, CurTarget.velocity, 18f, 0);
                //射弹。
                if (!IsShoot)
                {
                    Projectile.rotation = CurTarget.IsLegal() ? (CurTarget.Center - Projectile.Center).ToRotation() : Projectile.rotation;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<CowboyBullet>(), Projectile.damage, 1f, Owner.whoAmI);
                    //生成成功之后，给这个东西一个向后退的速度
                    SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 0, Pitch = 0.2f }, Projectile.Center);
                    Projectile.velocity = -(vel / 3f);
                    //写代码特有的没轻没重。
                    for (int i = 0; i < 7; i++)
                    {
                        if (Main.rand.NextBool())
                            new SmokeParticle(Projectile.Center.ToRandCirclePos(), vel.ToRandVelocity(ToRadians(10f), 2f, 6f), RandLerpColor(Color.LightGray, Color.Black), 40, RandRotTwoPi, 1f, 0.12f, true).SpawnToPriorityNonPreMult();
                        else
                            new SmokeParticle(Projectile.Center.ToRandCirclePos(), vel.ToRandVelocity(ToRadians(10f), 2f, 6f), RandLerpColor(Color.LightGray, Color.Black), 40, RandRotTwoPi, 1f, 0.12f, true).SpawnToNonPreMult();
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        new StarShape(Projectile.Center.ToRandCirclePos(), vel.ToRandVelocity(ToRadians(10f), 1.2f, 7f), RandLerpColor(Color.LightGoldenrodYellow, Color.DarkOrange), 0.75f, 40).Spawn();
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        new ShinyCrossStar(Projectile.Center.ToRandCirclePos(), vel.ToRandVelocity(ToRadians(10f), 1.2f, 6f), RandLerpColor(Color.LightGoldenrodYellow, Color.DarkOrange), 40, RandZeroToOne, 1f, 0.40f).Spawn();
                    }
                    IsShoot = true;
                }
                else
                {
                    Projectile.AffactedByGrav();
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.07f);
                    if (Projectile.Opacity <= 0.1f)
                        Projectile.Kill();
                }
                Projectile.rotation += 0.08f * Projectile.spriteDirection;
            }
            Projectile.netUpdate = true;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (!IsShoot)
            {
                for (int i = 0; i < 8; i++)
                {
                    SB.Draw(tex, Projectile.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.Lerp(Color.Transparent, Color.White, Helper.GetAniProgress(0)).ToAddColor(), Projectile.rotation, tex.ToOrigin(), Projectile.scale * 1.05f, se, 0);
                }
            }
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Black, Color.White,Helper.GetAniProgress(0)) * Projectile.Opacity, Projectile.rotation, tex.ToOrigin(), Projectile.scale, se, 0);
            return false;
        }
    }
}
