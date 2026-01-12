using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class JudgementMainProj: ThrownHammerProj
    {
        
        private enum DoType
        {
            IsShooted,
            IsReturning,
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        protected override BoomerangDefault BoomerangStat => new(
            returnTime: 30,
            acceleration: 0.7f,
            returnSpeed: 12f,
            killDistance: 1800
        );
        //没啥必要，我写这个纯因为觉得长单词麻烦
        internal ref bool Update => ref Projectile.netUpdate;
        public override string Texture => GetInstance<Judgement>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            //气笑了
            Projectile.width = Projectile.height = 66;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 300;
            Projectile.scale *= 1.1f;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.White);
            DoGeneric();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Stealth && !ModProj.IsHitOnEnablFocusMechanicProj)
                ModProj.IsHitOnEnablFocusMechanicProj = true;
            TargetIndex = target.whoAmI;
            float vol = Owner.HasProj<JudgementLock>() ? 0.4f : 0.7f;
            if (Projectile.numHits % 3 == 0)
            {
                NormalShootPunishmentStar(target);
                SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
                SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.4f, MaxInstances = 1 }, target.Center);
            }
        }
        //手动绘制这个射弹，我不想用你灾的绘制方式
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }

        #region AI方法合集
        private void DoGeneric()
        {
            Projectile.rotation += 0.2f;
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.HallowedWeapons, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
        private void NormalShootPunishmentStar(NPC target)
        {
            Vector2 center = target.Center;
            float randsRad = Pi;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            int div = 3;
            //已有挂载射弹，将生成位置更新在玩家身上
            if (Owner.ownedProjectileCounts[ProjectileType<JudgementLock>()] > 0)
            {
                center = Owner.Center;
                randsRad = PiOver2;
                dir = -(target.Center - center).SafeNormalize(Vector2.UnitX);
                div = 4;
            }
            else
                center.CirclrDust(36, Main.rand.NextFloat(1.2f, 1.4f), Main.rand.NextBool() ? DustID.GemDiamond : DustID.HallowedWeapons, 3);
            for (int i = 1 ; i < 3; i++)
            {
                Vector2 velocity = dir.RotatedBy(Main.rand.NextFloat(-randsRad / div, randsRad / div)) * 8f;
                Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, velocity, ProjectileType<JudgementPunishStar>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                star.timeLeft = 100;
                star.penetrate = 1;
            }
        }
        //返程AI
        private void DoReturning()
        {
            //返程时执行类似回旋镖的AI
            Projectile.HomingTarget(Owner.Center, 1800f, BoomerangStat.ReturnSpeed, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //无潜伏属性，处死射弹
                if (!Stealth)
                {
                    if (ModProj.IsHitOnEnablFocusMechanicProj)
                        ModPlayer.FocusStrikeTime += 1;
                    Update = true;
                }
                //其余情况下，根据情况进行潜伏攻击
                else
                {
                    //音效
                    SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { MaxInstances = 0, Pitch = 0.5f}, Projectile.Center);
                    //当前没有任何挂载锤，则正常进入挂载状态
                    if (!Owner.HasProj<JudgementLock>())
                    {
                        Projectile.Center.CirclrDust(24, 3f, DustID.HallowedWeapons, 10);
                        Projectile lockHammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<JudgementLock>(), Projectile.damage, 0f, Owner.whoAmI);
                        lockHammer.ai[1] = TargetIndex;
                        //处死射弹。
                    }
                    //否则，执行其他AI
                    else
                    {
                        Owner.Center.CirclrDust(24, 3f, DustID.GemRuby, 10);
                        //锤子本身会在进入这个AI逻辑后处死
                        Update = true;
                        //追加射弹，然后处死锤子
                        DoAddition();
                    }
                }
                //无论如何都直接处死射弹
                Projectile.Kill();
            }
        }
        
        
        private void DoShooted()
        {
            AttackTimer += 1;
            //满足返程时间，返回
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                //重置计时器
                AttackTimer = 0;
                //切换攻击模组
                AttackType = DoType.IsReturning;
                //网络同步
                Update = true;
            }
        }
        private void DoAddition()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(Main.rand.NextFloat(PiOver4)));
                    Vector2 spawnSpeed = dir * 12f;
                    float ai1 = target.whoAmI;
                    Projectile hammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, spawnSpeed, ProjectileType<JudgementPunishStar>(), Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner, 0f, ai1);
                    hammer.ai[2] = 1f;
                    Update = true;
                }
            }
        }
        #endregion
    }
}