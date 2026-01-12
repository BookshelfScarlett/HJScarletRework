using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DialecticsSpiningBlock : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.Specific_AimLabBox.Path;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.5f;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.scale += 0.1f;
            Projectile.scale = Clamp(Projectile.scale, 0f, 1f);
            Projectile.rotation += ToRadians(1f);
            if (Owner.active && Owner.HeldItem.type == ItemType<DialecticsThrown>())
                Projectile.timeLeft = 2;
            if(Projectile.GetTargetSafe(out NPC target, false))
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center, 0.5f);
                target.HJScarlet().Dialectics_Mark = true;
                //常驻1s
                if (target.HJScarlet().Dialectics_Timer < 5)
                    Projectile.Kill();
                
            }
            else
            {
                //单位不合法的时候立刻处死出去
                Projectile.Kill();
            }
        }
        public override bool PreKill(int timeLeft)
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            return base.PreKill(timeLeft);
        }
        public override bool? CanDamage() => false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            SB.Draw(projTex, drawPos, null, Color.LightBlue, Projectile.rotation, ori, Projectile.scale * 0.8f, 0, 0);
            Tex2DWithPath cube = HJScarletTexture.Texture_WhiteCube;
            SB.Draw(cube.Value, drawPos + dir.RotatedBy(PiOver2) * 30f, null, Color.SkyBlue, Projectile.rotation, cube.Origin, new Vector2(0.2f, 2f) * Projectile.scale * 0.8f, 0, 0);
            SB.Draw(cube.Value, drawPos + dir.RotatedBy(Pi) * 30f, null, Color.SkyBlue, Projectile.rotation + PiOver2, cube.Origin, new Vector2(0.2f, 2f) * Projectile.scale * 0.8f, 0, 0);
            SB.Draw(cube.Value, drawPos + dir.RotatedBy(PiOver2) * -30f, null, Color.SkyBlue, Projectile.rotation, cube.Origin, new Vector2(0.2f, 2f) * Projectile.scale * 0.8f, 0, 0);
            SB.Draw(cube.Value, drawPos + dir.RotatedBy(Pi) * -30f, null, Color.SkyBlue, Projectile.rotation + PiOver2, cube.Origin, new Vector2(0.2f, 2f) * Projectile.scale * 0.8f, 0, 0);
            return false;
        }
    }
}
