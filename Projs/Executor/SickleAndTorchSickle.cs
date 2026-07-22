using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.Executor
{
    public class SickleAndTorchSickle : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<SickleAndTorch>();
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
