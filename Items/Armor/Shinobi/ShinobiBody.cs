using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Shinobi
{
    [AutoloadEquip(EquipType.Body)]
    public class ShinobiBody : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 26;
        }
        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
        }
        public float CritChance = 40;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritChance + "%");

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += CritChance;
        }
    }
}
