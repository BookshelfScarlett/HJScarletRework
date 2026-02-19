using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class BamboomerangProj : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => GetInstance<Bamboomerang>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(4, 2);
        public enum Style
        {
            Attack,
            Return
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public int HitCounter = 0;
        public override void ExSD()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation += ToRadians(20f);
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 30), DustID.GrassBlades);
            d.scale *= Main.rand.NextFloat(1.1f, 1.3f);
            d.velocity = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1.2f, 1.8f);
            d.noGravity = true;
            if (AttackType == Style.Attack)
            {
                Timer++;
                //如果有了可用的目标单位，开始执行追踪逻辑
                if (Projectile.GetTargetSafe(out NPC target, false))
                    Projectile.HomingTarget(target.Center, 600f, 18f, 30f, 10f);
                else if (Timer > 20f)
                {
                    AttackType = Style.Return;
                    Projectile.penetrate = -1;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.HomingTarget(Owner.MountedCenter, 1800f, 18f, 10f);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                    Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 30), Main.rand.NextBool() ? DustID.GrassBlades : DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(1.1f, 1.35f);
                d.velocity = Projectile.SafeDir().RotatedBy(Main.rand.NextFloat(PiOver4) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(4.2f, 4.6f);
            }

             if (AttackType != Style.Attack)
                return;
            HitCounter += 1;
            //在onhit这尝试搜索下一个目标单位
            float searchDistance = 600f;
            NPC nextTarget = null;
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = tar != target && tar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                //别穿墙搜索
                if(legalTar && distPerTar < searchDistance && Collision.CanHit(Projectile.Center, 1, 1, tar.Center, 1, 1))
                {
                    searchDistance = distPerTar;
                    nextTarget = tar;
                }
            }
            if (nextTarget == null || HitCounter > 2)
            {
                AttackType = Style.Return;
                //返程，也别让射弹中途意外处死
                Projectile.penetrate = -1;
                Projectile.netUpdate = true;
                Projectile.tileCollide = false;
                return;
            }
            //最后将当前敌对单位扔到全局的index里面
            Projectile.HJScarlet().GlobalTargetIndex = nextTarget.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
