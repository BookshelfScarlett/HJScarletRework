using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Shinobi
{
    [AutoloadEquip(EquipType.Head)]
    public class ShinobiHead : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 10;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus += "\n" + Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBonus").ToLangValue();
            player.HJScarlet().shinobiExecutor = true;
            player.HJScarlet().monkExecutor = true;
            player.setMonkT3 = true;
            player.maxTurrets += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<ShinobiHead>() && body.type == ItemType<ShinobiBody>() && legs.type == ItemType<ShinobiLegs>();
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.40f;
        }

    }
}
