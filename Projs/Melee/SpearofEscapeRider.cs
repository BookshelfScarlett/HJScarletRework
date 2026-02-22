using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class SpearofEscapeRider : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<SpearOfEscape_2>().Texture;
        public bool IsHit = false;
        public ref float SignForRightClick => ref Projectile.ai[0];
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(16, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            //由于特殊的攻击模组，这把武器硬性设定为无限穿透
            //你可以用他的特殊攻击来做一些可以的玩法，我也不阻止
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 50;
            Projectile.localNPCHitCooldown = 20;
        }
        public override void AI()
        {
            //每帧都Clamp掉这东西的生命值不让他超过最大300的硬上限
            if (Projectile.timeLeft > 50)
                Projectile.timeLeft = 50;
            Projectile.rotation = Projectile.velocity.ToRotation();
            //如果启用右键功能，带着玩家飞过去
            if (SignForRightClick ==1f)
            {
                Owner.Center = Projectile.Center;
                Owner.velocity = Projectile.velocity;
            }
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            DrawParticles();
        }
        private void DrawParticles()
        {
             //速度本身太快了，这里的粒子需要尽可能堆量
            for (int k = 0; k < 3; k++)
            {
                float GeneralScaleMul = 1.1f * RandZeroToOne;
                int GetLifeTime() => Main.rand.Next(8, 16);
                Vector2 posOffset = Projectile.SafeDirByRot() * 10f;
                Vector2 offset = Projectile.SafeDirByRot() * 55f - posOffset * k;
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(4f) - offset, 0.5f, RandLerpColor(Color.Orange, Color.OrangeRed), GetLifeTime(), Main.rand.NextFloat(0.1f, 0.12f) * GeneralScaleMul, RandRotTwoPi).Spawn();
                for (int i = 0; i < 2; i++)
                {
                    new Fire(Projectile.Center.ToRandCirclePos(8f) - offset, -Projectile.velocity / 8f, RandLerpColor(Color.Black, Color.Orange), GetLifeTime(), RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.2f * GeneralScaleMul).SpawnToPriorityNonPreMult();
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f) - offset, Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), GetLifeTime(), RandRotTwoPi, 1f, 0.3f * GeneralScaleMul, ToRadians(10f)).Spawn();
                }
            }

        }
        public override bool PreKill(int timeLeft)
        {
            SoundEngine.PlaySound(HJScarletSounds.SpearofEscape_Boom, Projectile.Center);
            int boomDamage = SignForRightClick == 1f ? Projectile.damage : (int)(Projectile.damage * 2f);
            if (HJScarletMethods.HasFuckingCalamity)
                boomDamage *= 3;

            if (Projectile.IsMe())
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<SpearofEscapeBoom>(), boomDamage, Projectile.knockBack);

            if (SignForRightClick == 1f)
            {
                //务必在这里把玩家弹开，我们不给无敌帧了
                Owner.velocity = Projectile.velocity.ToRandVelocity(ToRadians(15f), 12f, 18f) * -1;
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 70, Owner.velocity.ToSafeNormalize().ToRotation(), ToRadians(40f));
            }
            if (IsHit)
            {
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 70, Projectile.velocity.ToSafeNormalize().ToRotation(), ToRadians(40f));
                return true;
            }
            //biu biu biu
            float totalCounts = 8;
            if (HJScarletMethods.HasFuckingCalamity)
                totalCounts = 16;
            if (Projectile.IsMe())
            {
                for (float i = 0; i < totalCounts; i++)
                {
                    float rotArgs = ToRadians(360f / totalCounts * i);
                    Vector2 dir = (Projectile.rotation + rotArgs + Main.rand.NextFloat(ToRadians(-15f), ToRadians(15f))).ToRotationVector2() * Main.rand.NextFloat(6f, 10f);
                    if (SignForRightClick == 1f)
                        dir += Owner.velocity.ToRandVelocity(ToRadians(10f), 8f, 12f) * -1;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ProjectileType<SpearofEscapeMissile>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    ((SpearofEscapeMissile)proj.ModProjectile).DontUseMouseHoming = true;
                }
            }

            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Oiled, GetSeconds(5));
            IsHit = true;
            float totalCounts = 8;
            if (HJScarletMethods.HasFuckingCalamity)
                totalCounts = 16;

            for (float i = 0; i < totalCounts; i++)
            {
                float rotArgs = ToRadians(360f / totalCounts * i);
                Vector2 dir = (Projectile.rotation + rotArgs + Main.rand.NextFloat(ToRadians(-15f), ToRadians(15f))).ToRotationVector2() * Main.rand.NextFloat(10f, 13f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, dir, ProjectileType<SpearofEscapeMissile>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                ((SpearofEscapeMissile)proj.ModProjectile).AttackType = SpearofEscapeMissile.Style.Direct;
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float rotFix = ToRadians(135);
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            //烟雾。
            DrawSmoke(drawPos);
            SB.EnterShaderArea();
            //实际尾焰轨迹
            DrawTrail(drawPos);
            SB.EndShaderArea();
            //绘制辉光效果
            Projectile.DrawProj(Color.White,drawTime:1, rotFix: rotFix);
            return false;
        }

        private void DrawGlowStar(Vector2 drawPos)
        {
            Texture2D sharpTear = HJScarletTexture.Specific_RocketTrail.Value;
            Rectangle cutSource = sharpTear.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            float basicRot = Projectile.rotation - ToRadians(30) - ToRadians(90);
            Vector2 basicScale = new Vector2(3.1f, 4f); 
            Vector2 starPos = drawPos - Projectile.SafeDirByRot() * 60f;
            for (float i = 1; i > 0; i -= 0.1f)
            {
                Vector2 starScale = basicScale * new Vector2((1 - i) * .8f, i * .6f) * Projectile.scale;
                Color smoke = Color.Lerp(Color.Orange, Color.Lerp(Color.Orange, Color.OrangeRed, i), i) * 0.4f;
                SB.Draw(sharpTear, starPos, cutSource, smoke, basicRot + ToRadians(60f), ori, starScale, 0, 0);
                SB.Draw(sharpTear, starPos, cutSource, smoke, basicRot, ori, starScale, 0, 0);
            }
            
        }

        private void DrawSmoke(Vector2 drawPos)
        {
            //虽然我不想承认，但是这里Trail的绘制方法确实是用的starShape
            Texture2D starShape = HJScarletTexture.Specific_RocketTrail.Value;
            Rectangle cutSource = starShape.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            //设定缩放大小
            Vector2 baseScale = new Vector2(2.2f, 10.2f);

            for (float k = 1; k > 0; k -= 0.05f)
            {
                Vector2 scale3 = baseScale * new Vector2((1 - k) * .30f, .5f * k);
                float colorRatios = Clamp(k, 0.2f, 1f);
                SB.Draw(starShape, drawPos - Projectile.rotation.ToRotationVector2() * 45f, cutSource, Color.Black, Projectile.rotation - PiOver2, ori, scale3, 0, 0);
            }

        }

        private void DrawTrail(Vector2 drawPos)
        {
            //虽然我不想承认，但是这里Trail的绘制方法确实是用的starShape
            Texture2D starShape = HJScarletTexture.Specific_RocketTrail.Value;
            Rectangle cutSource = starShape.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            //设定缩放大小
            Vector2 baseScale = new Vector2(5.2f, 10.2f);
            Effect shader = HJScarletShader.VolcanoEruptingShader;
            shader.Parameters["uBaseColor"].SetValue(Color.Orange.ToVector4() * 0.5f);
            shader.Parameters["uTargetColor"].SetValue(Color.OrangeRed.ToVector4());
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 25);
            shader.Parameters["uIntensity"].SetValue(0.3f);
            shader.Parameters["uNoiseScale"].SetValue(new Vector2(9f, 5f));
            shader.Parameters["UseColor"].SetValue(true);
            shader.Parameters["uFadeRange"].SetValue(0.3f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Noise_Misc2.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;
            //绘制，然后叠图。
            //这里一共会画20次
            //是的没错，然后又有一堆火箭，所以理论来说会有总共超过500多次的绘制
            //希望电脑没事。
            for (float k = 1; k >= 0f; k -= 0.05f)
            {
                Vector2 scale3 = baseScale * new Vector2((1 - k) * .30f, .5f * k);
                float colorRatios = Clamp(k, 0.2f, 1f);
                shader.Parameters["uColorFactor"].SetValue(colorRatios);
                SB.Draw(starShape, drawPos - Projectile.rotation.ToRotationVector2() * 55f, cutSource, Color.OrangeRed * colorRatios, Projectile.rotation - PiOver2, ori, scale3, 0, 0);
            }
            DrawGlowStar(drawPos);
        }
    }
}
