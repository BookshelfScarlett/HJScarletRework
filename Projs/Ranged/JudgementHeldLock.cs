using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class JudgementHeldLock : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => GetInstance<JudgementMainProj>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        private ref float AttackTimer => ref Projectile.ai[0];
        private int MountedIndex = -1;
        private bool CanSpawnHolyPunishment
        {
            get => Projectile.HJScarlet().ExtraAI[0] == 1f;
            set => Projectile.HJScarlet().ExtraAI[0] = value ? 1f : 0f;
        }
        private int TargetIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private ref float Oscillation => ref Projectile.ai[2];
        private int CurrentLifeTime = -1;
        public override void ExSD()
        {
            //基类被我手动刻了一个useLocal
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 40;
            Projectile.timeLeft = 3600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale *= 1.1f;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            UpdateMountedStarProj();
            //追踪敌人，与其他
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true, 1000f))
            {
                Projectile.rotation += ToRadians(5f);
                Projectile.HomingTarget(target.Center, 1800f, 13f, 20f);
                CanSpawnHolyPunishment = true;
                Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.HallowedWeapons : DustID.GemDiamond, Projectile.velocity * 0.25f + Main.rand.NextVector2CircularEdge(5f, 5f) * 1.2f);
                d.noGravity = true;

                //更新当前的生存时间
                CurrentLifeTime = Projectile.timeLeft;
            }
            else
            {
                Oscillation += 0.025f / 3;
                //记得清零射弹当前的速度，因为下方实际上使用正弦曲线来精确控制
                Projectile.velocity *= 0;
                if (Owner.HasProj<DeathTollsHeldMinion>())
                    UpdateIfNoTargetNearbyAndHasNightmareProj();
                else
                    UpdateIfNoTargetNearby();
                CanSpawnHolyPunishment = false;
                //锁定当前的生存时间避免出现意外处死
                Projectile.timeLeft = CurrentLifeTime;
            }
        }
        private void UpdateIfNoTargetNearbyAndHasNightmareProj()
        {
            //基本的挂机状态，此处使用了正弦曲线
            Vector2 anchorPos = new Vector2(Owner.MountedCenter.X + Owner.direction * 75f, Owner.MountedCenter.Y - 100f - 60f * (MathF.Sin(Oscillation) / 9f));
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f / Projectile.extraUpdates);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = (-(Owner.MountedCenter - Projectile.Center)).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
        }
        private void UpdateIfNoTargetNearby()
        {
            //基本的挂机状态，此处使用了正弦曲线
            Vector2 anchorPos = new Vector2(Owner.MountedCenter.X - Owner.direction * 54f, Owner.MountedCenter.Y - 60f * (MathF.Sin(Oscillation) / 9f));
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f / Projectile.extraUpdates);
            float angleToWhat = ToRadians(115f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(60f); 
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.1f);
        }
        public override void OnKill(int timeLeft)
        {
            if (CanSpawnHolyPunishment)
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { MaxInstances = 0, Pitch = 0.5f}, Projectile.Center);
                //生成准备进行十字裁决的挂载射弹
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<JudgementHolyCrossMounted>(), Projectile.damage, 0f, Owner.whoAmI);
                proj.ai[0] = TargetIndex;
            }
            else
            {
                Projectile.Center.CirclrDust(36, 3f, DustID.HallowedWeapons, 12);
                //原版粒子总体够用，但我还是决定用这个光球。
                float rotArg = 360f / 36;
                for (int i = 0; i < 36; i++)
                {
                    float rot = ToRadians(i * rotArg);
                    Vector2 offsetPos = new Vector2(4f, 0f).RotatedBy(rot);
                    Vector2 dVel = new Vector2(4f, 0f).RotatedBy(rot);
                    new ShinyOrbParticle(Projectile.Center + offsetPos, dVel, Main.rand.NextBool() ? Color.Gold : Color.White,80, 1.2f).Spawn();
                }
            }
        }
        private void UpdateMountedStarProj()
        {
            //生成需要的挂载弹
            if (AttackTimer == 0)
            {
                AttackTimer = 1;
                CurrentLifeTime = Projectile.timeLeft;
                int type = ProjectileType<JudgementPunishStarMounted>();
                //生成神圣新星用的挂载弹
                if (Owner.ownedProjectileCounts[type] < 1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj.localAI[0] = Projectile.Center.X;
                    proj.localAI[1] = Projectile.Center.Y;
                    proj.ai[2] = Projectile.whoAmI;
                    MountedIndex = proj.whoAmI;
                }
                //直接返回以确保其不会执行下方的更新
                return;
            }

            //时刻更新挂载弹的位置
            if (MountedIndex != -1)
            {
                Projectile proj = Main.projectile[MountedIndex];
                proj.localAI[0] = Projectile.Center.X;
                proj.localAI[1] = Projectile.Center.Y;
                //控制挂载弹的情况
                proj.HJScarlet().ExtraAI[0] = CanSpawnHolyPunishment.ToInt();
            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //持续更新挂载单位
            TargetIndex = target.whoAmI;
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix:+PiOver4);
            Projectile.DrawProj(Color.White, rotFix: +PiOver4);
            return false;
        }
    }
}
