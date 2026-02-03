using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class GalvanizedHandThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<GalvanizedHand>().Texture;
        public enum Style
        {
            Shoot,
            Stab
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public Vector2 StoredPosition = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(12, 2);
        }
        public override bool ShouldUpdatePosition()
        {
            return AttackType == Style.Shoot;
        }
        public override void AI()
        {
            if (AttackType == Style.Stab)
            {
                if(Projectile.GetTargetSafe(out NPC target,false))
                {
                    Projectile.Center = target.Center + StoredPosition;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == Style.Shoot)
            {
                StoredPosition = Projectile.Center - target.Center;
                AttackType = Style.Stab;
                Projectile.netUpdate = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fix = PiOver4;
            Projectile.DrawGlowEdge(Color.White, rotFix: fix);
            Projectile.DrawProj(Color.White, 6, rotFix: fix);
            return false;
        }
    }
}
