using HJScarletRework.Buffs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class AzureFrostmarkProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + "Proj_" + nameof(AzureFrostmark);
        public enum Style
        {
            Shoot,
            SpinAndFade
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public float Speed = 0f;
        public float SpinMoveTime = 45f;
        public bool HitTarget = false;
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(8, 2);
        }

        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Ice);
            if (!Projectile.HJScarlet().FirstFrame)
            {
                Speed = Projectile.velocity.Length();
                Projectile.originalDamage = Projectile.damage;
                if (HJScarletMethods.HasFuckingCalamity)
                {
                    Projectile.localNPCHitCooldown = 30;
                    Projectile.tileCollide = false;
                }
            }
            //天塌下来了我也要用我的shinyorb和smoke
            Projectile.Opacity = Clamp(Projectile.Opacity, 0f, 1f);
            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.SpinAndFade:
                    DoSpinAndFade();
                    break;
            }
        }

        private void DoShoot()
        {
            Timer++;
            Projectile.Opacity += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            float ratio = Timer / (SpinMoveTime) ;
            float reverseRatio = 1 - ratio;
            Vector2 dustVel = Projectile.velocity.ToRandVelocity(ToRadians(25f)) * Main.rand.NextFloat(1.2f, 1.6f) * -3f * reverseRatio;
            //timer会同时总控射弹的速度，这里需要随射弹尽量延展粒子生成来避免过度集中
            Vector2 extendedPos = Main.rand.NextFloat(ratio * 30f) * Projectile.SafeDir();
            Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(2f) + extendedPos;
            Color smokeColor = RandLerpColor(Color.DeepSkyBlue, Color.WhiteSmoke) * Clamp(ratio, 0.8f, 1f);
            new SmokeParticle(spawnPos, dustVel, smokeColor, 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.14f, 0.18f)).Spawn();

            //钝角。
            spawnPos = Projectile.Center.ToRandCirclePosEdge(6f) + extendedPos;
            new ShinyOrbParticle(spawnPos, dustVel, RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue) * reverseRatio, 40, 0.6f).Spawn();
            new ShinyOrbParticle(spawnPos, dustVel, RandLerpColor(Color.White, Color.WhiteSmoke) * reverseRatio, 40, 0.2f).Spawn();
            if (Timer < SpinMoveTime)
                Projectile.velocity = Projectile.SafeDir() * Speed * (1f - Timer / (SpinMoveTime + 1f));
            else
            {
                //重置属性然后跳转
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.damage = Projectile.originalDamage;
                AttackType = Style.SpinAndFade;
                Timer *= 0f;
            }
        }
        //这段真的是沉浸于自己的世界了
        private void DoSpinAndFade()
        {
            if (Timer == 12)
                SpawnEnergyBall();
            Timer++;
            Projectile.Opacity -= 0.1f;
            if (Projectile.Opacity <= 0.1f && Timer > 12f)
            {
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                Projectile.Kill();
            }
        }
        private void SpawnEnergyBall()
        {
            if (!Projectile.IsMe())
                return;
            //这里需要遍历一遍所取位置是否处于wall里面。如果是则回退直到适合为止
            Vector2 projDir = Projectile.SafeDirByRot();
            Vector2 spawnPos = Projectile.Center;
            bool ifHitWall = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
            if (ifHitWall)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 fixedPos = Projectile.position - projDir * i * 10f;
                    bool isStillHitWall = Collision.SolidCollision(fixedPos, Projectile.width, Projectile.height);
                    //如果当前的位置已经合适，将spawnPos重设
                    if (!isStillHitWall)
                    {
                        spawnPos = fixedPos;
                        break;
                    }
                }
            }
            //这里主要是为了对准矛上的柄。不过如果射弹本身就处于物块检测内，那就没法子了
            Vector2 posOffset = projDir * 30f * (!ifHitWall).ToInt();
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 dir = projDir.RotatedBy(Main.rand.NextFloat(ToRadians(10f), ToRadians(15f)) * i + Main.rand.NextFloat(ToRadians(-5f), ToRadians(5f)));
                Projectile ball= Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), posOffset + spawnPos, dir * 8f, ProjectileType<AzureFrostmarkEnergy>(), Projectile.damage / 2, Projectile.knockBack);
                ball.ai[0] = 13f * HitTarget.ToInt();
                //天王老子来了我都要用自己的粒子
                //不服憋着
                for (int j = 0; j < 15; j++)
                {
                    //最大的原因是白天过曝看不到
                    //这里的光球粒子需要携带一个暗色的烟出来
                    Vector2 pos = posOffset + spawnPos.ToRandCirclePos(8f);
                    Vector2 vel = -dir * Main.rand.NextFloat(8f);
                    //由于粒子绘制原因。shinyobr的速度需要更短
                    new ShinyOrbParticle(pos, vel * 0.8f, RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue), 40, 0.7f).Spawn();
                    new SmokeParticle(pos, vel, RandLerpColor(Color.DeepSkyBlue, Color.Gray), 40, 0.5f, 1f, 0.14f).SpawnToPriorityNonPreMult();
                }
            }
            new CrossGlow(spawnPos + posOffset, Color.SkyBlue, 40, 1f, 0.25f).Spawn();
            new CrossGlow(spawnPos + posOffset, Color.White, 40, 0.5f, 0.2f).Spawn();
            for (int i = 0; i < 15; i++)
                new SmokeParticle(posOffset + spawnPos.ToRandCirclePosEdge(4f), RandDirTwoPi * 1f, RandLerpColor(Color.DeepSkyBlue, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f).SpawnToPriorityNonPreMult();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackType == Style.SpinAndFade)
                return false;
            AttackType = Style.SpinAndFade;
            Projectile.rotation = oldVelocity.ToRotation();
            //将矛刺入墙体内
            Projectile.position += oldVelocity.SafeNormalize(Vector2.UnitX) * 10;
            //刷新持续时间，并做掉速度
            Projectile.velocity = Vector2.Zero;
            Timer = 0;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitTarget = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int length = Projectile.oldPos.Length;
            float rot = Projectile.rotation;
            for (int i = length - 1; i >= 0; i--)
            {
                if (AttackType == Style.SpinAndFade)
                    continue;
                float rads = (float)i / length;
                Color lerpColor = Color.Lerp(Color.DeepSkyBlue, Color.Lerp(Color.SkyBlue, Color.AliceBlue, rads * 0.7f), rads).ToAddColor(0) * Clamp(Projectile.velocity.Length(), 0f, 1f) * 0.50f;
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, lerpColor, rot + PiOver4, ori, 1f, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.SkyBlue.ToAddColor(0) * Projectile.Opacity, rot + PiOver4, ori, 1f, 0, 0);
            SB.Draw(projTex, drawPos, null, Color.WhiteSmoke * Projectile.Opacity, rot + PiOver4, ori, 1f, 0, 0);
            return false;
        }
    }
}
