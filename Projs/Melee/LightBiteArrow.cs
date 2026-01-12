using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteArrow : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => GetInstance<LightBite_2>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7, 4);
            Main.projFrames[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.ArmorPenetration = 25;
        }
        public override bool? CanDamage() => Timer > 10;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + PiOver4;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].scale = 1.2f;
            Timer++;
            if (Timer > 45)
            {
                Projectile.penetrate = 1;
                if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex,true))
                    Projectile.HomingTarget(target.Center, 9999f, 12f, 20f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }
        public override bool PreKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Torch);
                dust.noGravity = true;
                dust.scale = 2;
                dust.velocity *= 4f;
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition - Projectile.velocity * k * 0.5f;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(tex, drawPos, null, color, Projectile.rotation, tex.Size()/2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            return false;
        }
    }
}
