using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class JudgementHolyCrossMounted : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        private enum DoType
        {
            Begin,
            End,
            Extra
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[2];
            set => Projectile.ai[2] = (short)value;
        }
        private ref float GeneralProgress => ref Projectile.localAI[0];
        private float BeginAniTime = 15f;
        private float EndAniTime = 30f;
        private List<int> BeamList = [];
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 2;
            Projectile.friendly = true;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.noEnchantments = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            //让这个东西全局跟随转角动画
            Projectile.rotation += ToRadians(1);
            //全局获取敌对单位的位置并时刻更新。
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.Center = target.Center;

            switch (AttackType)
            {
                case DoType.Begin:
                    DoBeginAI();
                    break;
                case DoType.End:
                    DoEndAI();
                    break;
                case DoType.Extra:
                    DoExta();
                    break;
            }
        }
        private void DoBeginAI()
        {
            //这里的generalprogress会被动画受限并不断累积至1.
            GeneralProgress += EaseInOutExpo(AttackTimer / BeginAniTime);
            GeneralProgress = Clamp(GeneralProgress, 0f, 1f);
            AttackTimer++;
            if (GeneralProgress >= 1f)
            {
                AttackType = DoType.End;
                AttackTimer = 0f;
                Projectile.netUpdate = true;

            }
        }
        private void DoEndAI()
        {
            //此处需要逆转进程，方便predraw内的放缩绘制
            GeneralProgress -= EaseInCubic(AttackTimer / EndAniTime);
            GeneralProgress = Clamp(GeneralProgress, 0f, 1f);
            AttackTimer++;
            if (GeneralProgress <= 0f)
            {
                SpawnHolyJudement();
                //延后更新
                AttackType = DoType.Extra;
                AttackTimer = 0f;
                Projectile.netUpdate = true;
            }
        }
        private void SpawnHolyJudement()
        {
            //原版粒子总体够用，但我还是决定用这个光球。
            float rotArg = 360f / 36;
            for (int j = 0; j <= 1; j++)
            {
                for (int i = 0; i < 36; i++)
                {
                    float rot = ToRadians(i * rotArg);
                    Vector2 offsetPos = new Vector2(12f + j * 12, 0f).RotatedBy(rot);
                    Vector2 dVel = new Vector2(4f + 4 * j, 0f).RotatedBy(rot);
                    float scale = 0.15f;
                    new HRShinyOrb(Projectile.Center + offsetPos, dVel, Main.rand.NextBool() ? Color.Gold : Color.White, 80, 0f, 1f, scale).Spawn();
                    new HRShinyOrb(Projectile.Center + offsetPos, dVel, Color.White, 80, 0f, 1f, scale * 0.25f).Spawn();
                }
            }
            //处死时释放
            for (int i = 0; i < 4; i++)
            {
                //666我还要存他们的数组信息来着
                float curRadians = ToRadians(ToDegrees(Projectile.rotation) + 90f * i);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, curRadians.ToRotationVector2() * 16f, ProjectileType<JudgementHolyCross>(), (int)(Projectile.damage * 0.35f), 0f, Projectile.owner);
                proj.ai[1] = 16f;
                //这里必须得保留，确保初始生成的射线角度是正确的
                proj.rotation = curRadians;
                proj.localAI[0] = Projectile.Center.X;
                proj.localAI[1] = Projectile.Center.Y;
                BeamList.Add(proj.whoAmI);
            }

        }
        private void DoExta()
        {
            //等待一段时间后自然死亡。
            //遍历所有可能存在的射弹
            int[] array = [.. BeamList];
            for (int i = 0; i < array.Length;i++)
            {
                //这里写的一堆shit本质上是为了更新四个射线的中心点。
                Main.projectile[array[i]].localAI[0] = Projectile.Center.X;
                Main.projectile[array[i]].localAI[1] = Projectile.Center.Y;
            }
            AttackTimer += 1f;
            if (AttackTimer > 10f && Owner.ownedProjectileCounts[ProjectileType<JudgementHolyCross>()] < 1)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //这里的放缩会被lerp进行一次总控。
            Vector2 dynamicBackgroundScale = Vector2.Lerp(Vector2.Zero, new Vector2(1.4f,1.4f), GeneralProgress) * Projectile.scale * 1.1f;
            Vector2 dynamicBloomScale = Vector2.Lerp(Vector2.Zero, new Vector2(0.8f,0.8f), GeneralProgress) * Projectile.scale * 1.1f;
            Vector2 ori = HJScarletTexture.Particle_HRStar.Origin;
            //最后我们实际绘制他。
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SB.Draw(HJScarletTexture.Particle_HRStar.Value, Projectile.Center - Main.screenPosition, null, Color.Yellow, Projectile.rotation, ori, dynamicBackgroundScale, SpriteEffects.None, 0.1f);
            SB.Draw(HJScarletTexture.Particle_HRStar.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, ori, dynamicBloomScale, SpriteEffects.None, 0.1f);

            SB.End();
            SB.BeginDefault();
            return false;
        }
    }
}
