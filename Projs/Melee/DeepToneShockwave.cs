using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneShockwave : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public int MountedProjIndex { get; set; }
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 100;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 0;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Timer += 0.1f;
            if(Timer > 3f)
            {
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0)
                    Projectile.Kill();
            }
            Projectile.ExpandHitboxBy(100 * (int)(Timer + 1f), 100 * (int)(Timer + 1));
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ring = HJScarletTexture.Texture_BloomShockwave.Value;
            SB.EnterShaderArea();
            SB.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, ring.Size() / 2, Projectile.scale * Timer, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
