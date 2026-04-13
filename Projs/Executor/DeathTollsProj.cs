using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
namespace HJScarletRework.Projs.Executor
{
    public class DeathTollsProj : ThrownHammerProj
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
            Projectile.extraUpdates = 3;
        }
        public override void AI()
        {
            Projectile.rotation += 0.12f;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            DrawTrailingDust();
            AttackAIHanlder();
        }

        public void AttackAIHanlder()
        {
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

        private void DoStealth()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex))
                Projectile.HomingTarget(target.Center, 600f, 24f, 20f);

            //如果超出了玩家屏幕范围，且玩家仍然没有仆从锤，生成仆从锤
            if (Projectile.TooAwayFromOwner(1200f) && !Owner.HasProj<DeathTollsMinion>(out int projID) && Projectile.IsMe())
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
            if (!Projectile.HJScarlet().FirstFrame)
            {
                //压制音量，这里由仆从锤的射线声作为主导
                SoundStyle pickSound = HJScarletSounds.GalvanizedHand_Toss with { Pitch = -0.45f, PitchVariance = 0.1f, MaxInstances = 2, Volume = 0.95f };
                SoundEngine.PlaySound(pickSound, Owner.Center);
            }
            AttackTimer += 1;
            if (Projectile.MeetMaxUpdatesFrame(AttackTimer, 12))
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
                    Projectile.AddExecutionTime(ItemType<DeathTolls>());
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
            bool hasMinion = Owner.HasProj<DeathTollsMinion>(out int minionID);
            //普攻
            if (!Stealth)
            {
                //下面这个会扔到一个统一的管理里面。
            
                if (Projectile.numHits % 2 == 0)
                {
                    for (int i = 0; i < 16; i++)
                        new ShinyCrossStar(target.Center.ToRandCirclePos(7f), RandVelTwoPi(0f, 8f), RandLerpColor(Color.Violet, Color.DarkViolet), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.44f, 0.70f), false).Spawn();
                    for (int i = 0; i < 8; i++)
                        new TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(6f), 1f, RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.45f, RandRotTwoPi).Spawn();
                    for (int i = 0; i < 18; i++)
                        new SmokeParticle(target.Center.ToRandCirclePos(4), RandVelTwoPi(1f, 8f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, .81f, 0.21f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
                }
            }
                    int counts = 1 + hasMinion.ToInt();
                    for (int i = 0; i < counts; i++)
                        NightmareArrowDrop(target, Projectile.damage / 2);

            if (AttackType != DoType.IsStealth)
                return;

            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit, Projectile.Center);
            //优先生成挂载射弹
            if (!hasMinion)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, minionID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { Volume = 0.75f }, Projectile.Center);
            }
            else if (!Owner.HasProj<DeathTollsExecution>(out int projID))
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
            Projectile spark = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ProjectileType<DeathTollsArrow>(), flareDamage, 1.1f, Projectile.owner);
            if (target.CanBeChasedBy())
                ((DeathTollsArrow)spark.ModProjectile).CurTarget = target;
        }

        private void DrawTrailingDust()
        {
            if (Projectile.IsOutScreen())
                return;

            if (Projectile.FinalUpdate())
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(32f), Projectile.velocity.ToRandVelocity(0.1f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0.65f).Spawn();
            if (Projectile.FinalUpdateNextBool(3))
                new EmptyRing(Projectile.Center.ToRandCirclePos(36), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0.2f, 1, altRing: Main.rand.NextBool()).SpawnToNonPreMult();


        }
    }
}