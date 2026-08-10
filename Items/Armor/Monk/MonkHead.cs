using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Shinobi;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Monk
{
    [AutoloadEquip(EquipType.Head)]
    public class MonkHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemType<MonkBody>(), ItemType<MonkLegs>()];
        public override bool SetUpArmorSet => true;
        public override void SetStaticDefaults()
        {
            Type.ShimmerEach<ShinobiHead>();
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 8;
        }
        public float Damage = .15f;
        public int Slots = 2;
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue().ToFormatValue(Slots);
            player.HJScarlet().monkExecutor = true;
            player.maxTurrets += Slots;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
        }
    }
}
