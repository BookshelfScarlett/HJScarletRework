using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class PerfectNightMinion : HJScarletProj
    {
        public override string Texture => GetInstance<PerferctNightProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
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
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}
