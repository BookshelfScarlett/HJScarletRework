using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class SweetStabThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<SweetStabThrown>().Texture;
        private enum Style
        {
            Attack,
            Stab
        }
        private ref float Timer => ref Projectile.ai[0];
        private Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4, 2);
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = HJScarletMethods.HasFuckingCalamity.ToInt();
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            switch (AttackType)
            {
                case Style.Attack:
                    DoAttack();
                    break;
                case Style.Stab:
                    DoStab();
                    break;
            }
        }

        private void DoStab()
        {
            //粒子。
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Dust d = Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(-35f, 30f) + dir.RotatedBy(Main.rand.NextBool().ToDirectionInt() * PiOver2) * Main.rand.NextFloat(-10f,10f), Main.rand.NextBool() ? DustID.Honey2: DustID.Honey);
            d.velocity = Vector2.UnitY * -Main.rand.NextFloat(0.4f, 0.8f);
            d.noGravity = false;
            //在接触玩家时，处死射弹，并给予玩家一定程度的治疗
            //这个可以给所有玩家吃
            foreach (var needPlayer in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(needPlayer.Hitbox))
                {
                    int healAmt = HJScarletMethods.HasFuckingCalamity ? 20 : Main.rand.Next(3, 6);
                    needPlayer.Heal(healAmt);
                    needPlayer.AddBuff(BuffType<HoneyRegenAlt>(), 60);
                    //生成粒子，追加树叶音效
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 spawnPos = Projectile.Center - dir * 50;
                        new Leafs(spawnPos + Main.rand.NextVector2Circular(10f, 10f) + dir * (i * 10), needPlayer.velocity.SafeNormalize(Vector2.UnitY) * 10f, Color.Green, 120, 0, 1, 0.1f, Main.rand.NextFloat(1f, 1.4f)).Spawn();
                    }
                    SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
                    Projectile.Kill();
                }
            }
        }

        private void DoAttack()
        {
            Timer += 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer > 10)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity.X *= 1f;
                if (Projectile.velocity.Y < 30f)
                    Projectile.velocity.Y += 0.17f;
            }
            //我要往你身上滴蜡
            //后面再搞一个水滴的粒子在这
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6f, 6f) + Vector2.UnitY * 10f, Main.rand.NextBool() ? DustID.Honey2: DustID.Honey);
            d.velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(PiOver2 / 2) * Main.rand.NextBool().ToDirectionInt()) * -Main.rand.NextFloat(2.4f, 2.8f);
            d.noGravity = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.AddBuff(BuffID.Honey, 360);
            
        }
        public override bool PreKill(int timeLeft)
        {
            Vector2 vel = AttackType == Style.Stab ? Vector2.UnitY * -Main.rand.NextFloat(0.4f, 0.8f) : Projectile.SafeDir() * 8f;
            //生成粒子，追加树叶音效
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center +  Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextBool().ToDirectionInt() * PiOver2) * Main.rand.NextFloat(-15f, 15f) + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(-35f, 30f), Main.rand.NextBool() ? DustID.Honey2: DustID.Honey);
                d.velocity = vel;
                d.noGravity = AttackType==Style.Attack;
            }
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackType == Style.Stab)
                return false;
            AttackType = Style.Stab;
            Projectile.rotation = oldVelocity.ToRotation();
            //将矛刺入墙体内
            Projectile.position += oldVelocity.SafeNormalize(Vector2.UnitX) * 10;
            //刷新持续时间，并做掉速度
            Projectile.timeLeft = 150;
            Projectile.velocity = Vector2.Zero;
            //不要处死他
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White * 0.5f, rotFix: PiOver4);
            Projectile.DrawProj(Color.White * lightColor.A, drawTime: 4,rotFix: PiOver4);
            return false;
        }
    }
    public abstract class ThrownSpearProjClass : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public new string LocalizationCategory => "Projs.Friendly.Melee";
        public string ProjPath => "HJScarletRework/Assets/Texture/Projs/";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
            ExSSD();
        }
        public virtual void ExSSD() { }
        public override void SetDefaults()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.noEnchantmentVisuals = true;
            Projectile.friendly = true;
            ExSD();
        }
        public virtual void ExSD() { }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
    }

}
