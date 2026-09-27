using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class StellaHeldProj : HJScarletRangedWeaponoutClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<Stella>().Texture;
        public override float RecoilPower => 0;
        public override float RecoilWeaponPullbackRatios => 0;
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Vector2 HoldoutOffset => base.HoldoutOffset;
        public override bool HoldoutEdgeEnable => false;
        public override int OriginalItemID => ItemType<Stella>();
        public override int ProjExtraUpdates => 5;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        protected override void UpdateWeaponUsing()
        {
            base.UpdateWeaponUsing();
        }
        protected override void UpdateRecoil()
        {

        }
        protected override void UpdateGlobalReset()
        {

        }
        protected override void PreAttack()
        {
            base.PreAttack();
        }
        protected override void OnAttack()
        {
            base.OnAttack();
        }
    }
}
