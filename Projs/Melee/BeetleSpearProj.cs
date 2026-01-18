using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class BeetleSpearProj : ThrownSpearProjClass
    {
        public override string Texture => $"{ProjPath}Proj_{nameof(BeetleSpear)}";
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sharpTear = HJScarletTexture.Particle_SharpTear;
            Vector2 rotDir = Projectile.SafeDirByRot();
            float rotArgsTime = 16f;
            for (float i = 0;i<rotArgsTime;i++)
            {
                Vector2 ringPos = rotDir.RotatedBy(ToRadians(360f / rotArgsTime * i)) * 12f;
                //SB.Draw(sharpTear,ringPos,null,)
            }
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
