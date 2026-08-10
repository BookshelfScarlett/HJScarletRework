using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;

namespace HJScarletRework.Projs.Executor
{
    public class AbyssalWorldBeam : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
