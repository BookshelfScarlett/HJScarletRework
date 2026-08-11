using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    internal class WeHaveBookshelfHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<WeHaveBookshelf>().Texture;
        public override int OriginalItemID => ItemType<WeHaveBookshelf>();
        public AnimationStruct Helper = new AnimationStruct(2);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1.2f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
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
