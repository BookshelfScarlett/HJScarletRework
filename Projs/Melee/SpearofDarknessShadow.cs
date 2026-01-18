using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class SpearofDarknessShadow : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<SpearofDarknessThrown>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        private int IdlePosIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Osci = 0f;
        private int StrikeTime = 260;
        private int CanDamageTime = 0;

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public bool AlreadyHit = false;
        public override void AI()
        {
            CanDamageTime +=1;
            SpawnDarkParticle();
            if (!AlreadyHit)
                UpdateIdlePos();
            else
                UpdateHit();

        }
        public override bool? CanDamage() => CanDamageTime > 50;
        private void UpdateHit()
        {
            //固定频率
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.94f;
            Projectile.Opacity -= 0.02f;
            if (Projectile.Opacity == 0f)
            {
                Projectile.Kill();
                return;
            }
        }

        private void SpawnDarkParticle()
        {
            if (Projectile.Opacity < Main.rand.NextFloat(0.42f))
                return;
            //火焰
            Color Firecolor2 = Color.Lerp(Color.Purple, Color.DarkViolet, Main.rand.NextFloat(0, 1));
            Vector2 fireOffset = Projectile.rotation.ToRotationVector2() * 20f + Main.rand.NextVector2Circular(4f, 4f);
            if (Main.rand.NextBool(4))
                new Fire(Projectile.Center - fireOffset, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 1.2f, Firecolor2, 30, Main.rand.NextFloat(TwoPi), 1, 0.1f * Projectile.Opacity).SpawnToPriorityNonPreMult();

            //挥发性粒子
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11);
            Color Firecolor = Color.Lerp(Color.Black, Color.DarkViolet, Main.rand.NextFloat(0, 1));
            new TurbulenceShinyOrb(spawnPos, 1f, Firecolor, 40, 0.20f * Projectile.Opacity, Main.rand.NextFloat(TwoPi)).SpawnToNonPreMult();
        }

        #region 挂载状态
        private void UpdateIdlePos()
        {
            if (Osci == 0f)
                SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { MaxInstances = 0, Pitch = Main.rand.NextFloat(0.3f, 0.7f), Volume = Main.rand.NextFloat(0.4f, 0.5f) });
            if (!Projectile.GetTargetSafe(out NPC target, true, 600))
            {
                //记得处死
                Projectile.Kill();
                return;
            }
            if (Projectile.timeLeft < StrikeTime)
            {
                //锁住生命值让其确保能攻击到目标
                Projectile.HomingTarget(target.Center, 600f, 20f, 20f);
                Projectile.extraUpdates = 2;
                return;
            }

            //递增的值越大，矛的摆动幅度越大
            Osci += 0.025f;
            //基本的挂机状态，此处使用了正弦曲线来让矛常规上下偏移
            //这里的位置通过相对位置的硬编码实现
            //第一步，取基本向上为方向
            Vector2 anchorPos = new Vector2(target.Center.X, target.Center.Y - 170f);
            //第二步，依据射弹情况来更新实际需要的位置
            anchorPos = anchorPos.RotatedBy(PiOver4 * (IdlePosIndex - 1),target.Center);
            //第三步计算更新位置，此处需要先计算单位向量
            Vector2 signalDir = (target.Center - anchorPos).SafeNormalize(Vector2.Zero);
            //而后，实际更新位置
            anchorPos = anchorPos + signalDir * (MathF.Sin(Osci) / 9f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f);
            //计算矛需要的朝向。
            float angleToWhat = (target.Center - Projectile.Center).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让矛朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.1f);

        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.Black, Color.DarkOrchid, rads) with { A = 0 }) * 0.9f * Projectile.Opacity * (1 - rads);
                SB.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter(), null, drawColor * Projectile.Opacity, Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            Projectile.DrawGlowEdge(Color.Purple  * Projectile.Opacity, drawTime: 16, posMove: 2.5f,rotFix: PiOver4);
            Projectile.DrawProj(Color.Black * Projectile.Opacity, rotFix: PiOver4, useOldPos:true);

            return false;
        }
    }
}
