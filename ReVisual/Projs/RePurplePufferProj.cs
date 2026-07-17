using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class RePurplePufferProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<PurplePuffer>();
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualPurplePuffer;
        }
        public override void AI(Projectile projectile)
        {
            IsMyPlayer(projectile, out ReVisualPlayer vp);
            if (ShouldApplyRevisual(projectile, vp))
            {
                ApplyThis(projectile);
                return;
            }
            else
                base.AI(projectile);
        }

        public void ApplyThis(Projectile proj)
        {
            for (int i = 0; i < 2; i++)
            {
            }
            proj.rotation += 0.3f;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            IsMyPlayer(projectile, out ReVisualPlayer vp);
            if (ShouldApplyRevisual(projectile, vp))
            {
                ApplyThisDraw(projectile, ref lightColor);
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }

        public void ApplyThisDraw(Projectile projectile, ref Color lightColor)
        {
        }
    }
}
