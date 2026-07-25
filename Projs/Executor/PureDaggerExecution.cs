using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class PureDaggerExecution : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<PureDagger>().Texture;
        public override void SetStaticDefaults()
        {
            
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
