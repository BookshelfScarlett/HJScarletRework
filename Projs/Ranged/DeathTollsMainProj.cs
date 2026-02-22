using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace HJScarletRework.Projs.Ranged
{
    public class DeathTollsMainProj: ThrownHammerProj
    {
        internal ref bool Update => ref Projectile.netUpdate;
        //攻击枚举
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        protected override BoomerangDefault BoomerangStat => new(
            //不准修改这个returnTime低于35
            returnTime: 35,
            returnSpeed: 26f,
            acceleration: 1.5f,
            killDistance: 1800
        );
        public override string Texture => GetInstance<DeathTolls>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            //夜明后的锤子应该上4eu了
            Projectile.height = Projectile.width = 66;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center))
                DrawTrailingDust();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
                case DoType.IsStealth:
                    DoStealth();
                    break;
            }
        }
        public override void PostAI()
        {
            if (Owner.HasProj<DeathTollsHeldMinion>())
            {
                Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Color Firecolor = RandLerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center + Main.rand.NextVector2Circular(8,8), fireVelocity * 4.5f, Firecolor, Main.rand.Next(60,90), Main.rand.NextFloat(TwoPi), 1f, Main.rand.NextFloat(0.20f,0.25f)).SpawnToPriorityNonPreMult();
            }
        }
        private void DoStealth()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex))
            {
                Projectile.extraUpdates = 5;
                Projectile.HomingTarget(target.Center, 600f, 24f, 20f);
            }
            else
                Projectile.extraUpdates = 4;

            //如果超出了玩家屏幕范围，且玩家仍然没有仆从锤，生成仆从锤
            if (Projectile.TooAwayFromOwner(1200f) && !Owner.HasProj<DeathTollsHeldMinion>(out int projID) && Projectile.IsMe())
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projID, Projectile.damage, 0f, Projectile.owner);
                SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss, Owner.Center);
                //而后，杀死射弹。
                Projectile.Kill();
            }
        }
        //首次投掷出去时的AI
        private void DoShooted()
        {
            if (Projectile.HJScarlet().FirstFrame)
            {
                //压制音量，这里由仆从锤的射线声作为主导
                SoundStyle pickSound = Owner.HasProj<DeathTollsHeldMinion>() ? HJScarletSounds.DeathsToll_Toss with { Pitch = 0.4f, Volume = 0.2f, MaxInstances = 0 } : SoundID.Item103;
                SoundEngine.PlaySound(pickSound, Owner.Center);
            }
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                AttackType = DoType.IsReturning;
                Update = true;
            }
        }
        //返程AI
        private void DoReturning()
        {
            Projectile.HomingTarget(Owner.Center, 1800f, BoomerangStat.ReturnSpeed, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //当前有任何挂载锤，所有的攻击都会直接在返回后杀掉弹幕
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    if (ModProj.IsHitOnEnablFocusMechanicProj)
                        ModPlayer.FocusStrikeTime += 1;
                }
                else
                {
                    Update = true;
                    AttackType = DoType.IsStealth;
                    Projectile.velocity *= -1;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            target.AddBuff(BuffID.ShadowFlame, 360);
            bool hasMinion = Owner.HasProj<DeathTollsHeldMinion>(out int minionID);
            //普攻
            if (!Stealth)
            {
                //下面这个会扔到一个统一的管理里面。
                if (!ModProj.IsHitOnEnablFocusMechanicProj && ModProj.UseFocusStrikeMechanic)
                    ModProj.IsHitOnEnablFocusMechanicProj = true;

                if (Projectile.numHits % 2 == 0)
                {
                    int counts = 1 + hasMinion.ToInt();
                    for (int i = 0; i < counts; i++)
                        NightmareArrowDrop(target, Projectile.damage / 2);
                }
            }
            if (AttackType != DoType.IsStealth)
                return;

            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit, Projectile.Center);
            //优先生成挂载射弹
            if (!hasMinion)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, minionID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss, Projectile.Center);
            }
            else if (!Owner.HasProj<DeathTollsCloneProj>(out int projID))
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, projID, Projectile.damage, 0f, Projectile.owner);
            }
            //然后直接处死这个射弹
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.DarkMagenta, 6);
            Projectile.DrawProj(Color.White, offset: 0.7f);
            return false;
        }
        private void NightmareArrowDrop(NPC target, int flareDamage)
        {
            //这下面一长串都是为了处理……生成的
            //返程写的挺fuck的
            float xDist = Main.rand.NextFloat(10f, 100f) * Main.rand.NextBool().ToDirectionInt();
            float yDist = Main.rand.NextFloat(800f, 1000f);
            Vector2 srcPos = target.Center + new Vector2(xDist, -yDist);
            //在滞留所有的射弹
            Projectile spark = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ProjectileType<DeathTollsNightmareArrow>(), flareDamage, 1.1f, Projectile.owner);
            spark.ai[2] = target.whoAmI;
            spark.localAI[0] = xDist;
            spark.localAI[1] = yDist;
        }
         
        private void DrawTrailingDust()
        {
            if (Stealth && Main.rand.NextBool(2) && AttackType == DoType.IsStealth)
                return;
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11);
            Color Firecolor = RandLerpColor(Color.Purple, Color.DarkViolet);
            new TurbulenceGlowOrb(spawnPos, 0.8f, Firecolor, 40, 0.32f, Projectile.velocity.SafeNormalize(Vector2.UnitX).ToRotation()).Spawn();
            bool drawBlack = Main.rand.NextBool();
            Color glowColor = drawBlack ? Color.Black : RandLerpColor(Color.Violet, Color.DarkViolet);
            Vector2 glowDustVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-PiOver4 / 2f, PiOver4 / 2f))* 4f;
            new ShinyOrbParticle(spawnPos, glowDustVelocity, glowColor, 40, 0.8f, drawBlack ? BlendStateID.Alpha : BlendStateID.Additive).Spawn();
        }
    }
}