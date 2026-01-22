using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static HJScarletRework.Projs.Ranged.AetherfireSmasherName;

namespace HJScarletRework.Projs.Ranged
{
    //Todo：按下鼠标右键后应当刷新一次生命值
    public class AetherfireSmasherMain: ThrownHammerProj
    {
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        internal ref bool Update => ref Projectile.netUpdate;
        public override string Texture => GetInstance<AetherfireSmasher>().Texture;
        #region 基础数值
        protected override BoomerangDefault BoomerangStat => new
        (
            returnTime: 40,
            returnSpeed: 28f,
            acceleration: 1.2f,
            killDistance: 1000
        );
        //总潜伏攻击时长为五秒
        private int StealthTotalTime => 60 * Projectile.extraUpdates;
        //挂载锤子的攻击频率：5 * 额外更新
        private int HangingHitCooldown => 5 * Projectile.extraUpdates;
        private bool CanSpawnVolcano = true;
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = Projectile.height = 66;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            DoGeneric();
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

        private void DirectlySpawnEruptionFireBall(float initTime, int eu = 1, int totalCount = 6, bool stealth = true)
        {
            for (int i = 0; i < totalCount; i++)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-PiOver4 / 4, PiOver4 / 4)) * Main.rand.NextFloat(14f, 18f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ProjectileType<Aetherfireball>(), Projectile.damage, Projectile.knockBack);
                proj.timeLeft = 480;
                proj.ai[0] = initTime;
                proj.extraUpdates = eu;
            }
        }
        //终 极 史 山
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Stealth && !ModProj.IsHitOnEnablFocusMechanicProj && ModProj.UseFocusStrikeMechanic)
                ModProj.IsHitOnEnablFocusMechanicProj = true;
            target.AddBuff(BuffID.Daybreak, 500);
            //攻击的敌怪传入
            SoundEngine.PlaySound(SoundID.Item89 with { MaxInstances = 0, Pitch = 0.8f }, Projectile.Center);
            TargetIndex = target.whoAmI;
            if (AttackType == DoType.IsStealth && Projectile.timeLeft < 15 && CanSpawnVolcano)
            {
                Vector2 center = new Vector2(target.Center.X, target.Center.Y + 30f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Vector2.Zero, ProjectileType<AetherfireVolcano>(), Projectile.damage, Projectile.knockBack);
                proj.ai[1] = target.whoAmI;
                CanSpawnVolcano = false;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Color lerpColor = Color.Lerp(baseColor, targetColor, Projectile.velocity.Length() / 26); 
            Projectile.DrawGlowEdge(lerpColor);
            Projectile.DrawProj(Color.White, offset:0.52f);
            return false;   
        }
        
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackType = DoType.IsReturning;
                AttackTimer = 0;
                Update = true;
            }
        }
        private void DoReturning()
        {
            Projectile.HomingTarget(Owner.Center, 3600f, BoomerangStat.ReturnSpeed, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //不是潜伏攻击，返回处死射弹
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    if (ModProj.IsHitOnEnablFocusMechanicProj)
                        ModPlayer.FocusStrikeTime += 1;
                    return;
                }
                else
                {
                    //二级锤子有极其高频率的攻击方式
                    AttackType = DoType.IsStealth;
                    Update = true;
                    Projectile.localNPCHitCooldown = HangingHitCooldown;
                    Projectile.timeLeft = StealthTotalTime;
                    DirectlySpawnEruptionFireBall(18f);
                }
            }
        }
        private void DoStealth()
        {
            bool available = Projectile.GetTargetSafe(out NPC target, TargetIndex,true, 1800f);
            if (available)
                Projectile.HomingTarget(target.Center,9999f, 20f, 20f);
        }
        private void DoGeneric()
        {
            Projectile.rotation += 0.2f;
            if (Stealth)
                DrawTrailingDust();
            else
                DrawNormalDust();
        }
        private void DrawNormalDust()
        {
            if (Main.rand.NextBool(Projectile.extraUpdates + 1))
            {
                PickTagColor(out Color baseColor, out Color targetColor);
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.height / 2, Projectile.width / 2);
                Vector2 glowDustVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-PiOver4 / 4f, PiOver4 / 4f)) * 4f;
                Dust d = Dust.NewDustPerfect(spawnPos, PickTagDust, glowDustVelocity);
                d.scale *= 1.2f;
                d.noGravity = true;
                Color glowColor = RandLerpColor(baseColor, targetColor);
                new ShinyOrbParticle(spawnPos, glowDustVelocity, glowColor, 40, 0.5f, BlendStateID.Additive, glowCenter: true).Spawn();
            }
        }
        public void DrawTrailingDust()
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            //故意不采用循环，因为要稍微处理圆弧状态粒子，但是我技术力不够，先放着了
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 speedValue = direction * 3f;
            Vector2 spawnPosition = Projectile.Center + direction.RotatedBy(PiOver2) * 8f;
            Vector2 realVel = speedValue.RotatedBy(radians: PiOver2);
            ShinyOrbParticle shinyOrbParticle = new(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle.Spawn();

            spawnPosition = Projectile.Center + direction.RotatedBy(-PiOver2) * 8f;
            realVel = speedValue.RotatedBy(-PiOver2);
            ShinyOrbParticle shinyOrbParticle2 = new(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle2.Spawn();
        }
        private short PickTagDust
        {
            get 
            {
                short Pick = Owner.name.SelectedName() switch
                {
                    NameType.TrueScarlet => DustID.CrimsonTorch,
                    NameType.WutivOrChaLost => DustID.YellowTorch,
                    NameType.Emma => DustID.HallowedTorch,
                    NameType.SherryOrAnnOrKino => DustID.BlueTorch,
                    NameType.Shizuku => DustID.WhiteTorch,
                    NameType.SerratAntler => DustID.DemonTorch,
                    NameType.Hanna => DustID.JungleTorch,
                    _ => DustID.OrangeTorch,
                };
                return Pick;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.Red;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = new Color(255, 178, 36);
                    targetColor = Color.Gold;
                    break;
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                //锯角 - 紫
                case NameType.SerratAntler:
                    baseColor = Color.Purple;
                    targetColor = Color.DarkViolet;
                    break;
                //Kino - 蓝
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;
                //绿
                case NameType.Hanna:
                    baseColor = Color.Green;
                    targetColor = Color.LimeGreen;
                    break;
                default:
                    baseColor = Color.OrangeRed;
                    targetColor = Color.Orange;
                    break;
            }
        }

    }
}