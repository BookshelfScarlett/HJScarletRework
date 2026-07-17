using HJScarletRework.Globals.Classes;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class BoneSlapBone : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Item, ItemID.Bone);
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
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
