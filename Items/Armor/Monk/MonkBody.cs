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
    [AutoloadEquip(EquipType.Body)]
    public class MonkBody : HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            Type.ShimmerEach<ShinobiBody>();
        }
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 22;
        }
        public int CritChance = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritChance + "%");

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += CritChance;
        }
    }
}
