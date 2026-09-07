using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Items.Weapons.Magic;

namespace HJScarletRework.Projs.Magic
{
    public class BrimstoneHeartHeldProj : HJScarletFloatingBook
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override int OriginalItemID => ItemType<BrimstoneHeart>();
        public override string Texture => GetInstance<BrimstoneHeart>().Texture;
        public override void HandleLeftAttack()
        {
            base.HandleLeftAttack();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            return false;
        }
    }
}
