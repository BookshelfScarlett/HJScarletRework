using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrescentRoseSniperSkill : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<CrescentRose>();
        public override string Texture => GetInstance<CrescentRoseHeldSniper>().Texture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
