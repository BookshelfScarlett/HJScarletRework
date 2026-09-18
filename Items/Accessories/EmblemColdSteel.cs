using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class EmblemColdSteel : HJScarletItemClass
    {
        public float Damage = .10f;
        public int Crit = 10;
        public static float MaxDamageMult = .75f;
        public override void SetStaticDefaults()
        {
            Type.ShimmerTo(ItemType<EmblemFirearm>());
        }
        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%",MaxDamageMult.ToPercent());
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HeldItem.CheckExecuteTypes(ExecutorWeaponType.ColdSteel))
            {
                player.GetDamage<ExecutorDamageClass>() += Damage;
                player.GetCritChance<ExecutorDamageClass>() += Crit;
            }
        }
    }
}
