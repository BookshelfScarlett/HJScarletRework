using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriBubble : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<TonbogiriThrown>().Texture;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public enum Style
        {
            Spawn,
            ShootLaser,
            Disapper
        }
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int SourceDamage => Projectile.originalDamage;
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.rotation += ToRadians(10f);
            //泡泡本身不允许造成任何伤害，这里在AI写死以避免任何可能的外部直接修改
            Projectile.damage *= 0;
            switch (AttackType)
            {
                case Style.Spawn:
                    DoSpawn();
                    break;
                case Style.ShootLaser:
                    DoShootLaser();
                    break;
                case Style.Disapper:
                    DoDisapper();
                    break;
            }
        }
        public void DoSpawn()
        {
            Projectile.velocity *= 0.92f;
            if (Projectile.velocity.Length() < 0.5f)
            {
                AttackType = Style.ShootLaser;
                Projectile.netUpdate = true;
                Timer *= 0f;
            }

        }
        public int Delay = 5;
        public int TotalLaser = 2;

        public void DoShootLaser()
        {
            //用这个Timer发射laser，这里只会一次发射两个
            Timer++;
            if (Projectile.GetTargetSafe(out NPC target))
                Projectile.HomingTarget(target.Center, 600f, 12f, 20f);
            else
                Projectile.Kill();
        }
        public void DoDisapper()
        {
            Projectile.scale -= 0.01f;
            Projectile.Opacity -= 0.01f;
            if (Projectile.Opacity <= 0f)
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bubble = Request<Texture2D>(GetInstance<VenomBubble>().Texture).Value;
            SB.Draw(bubble, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, bubble.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}
