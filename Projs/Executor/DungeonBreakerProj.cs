using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonBreakerProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<DungeonBreaker>().Texture;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
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
            return base.PreDraw(ref lightColor);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}
