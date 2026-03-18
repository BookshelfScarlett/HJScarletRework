using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class JungleMadnessProj: HJScarletProj
    {
        public override string Texture => GetInstance<JungleMadness>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
