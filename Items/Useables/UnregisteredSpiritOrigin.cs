using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class UnregisteredSpiritOrigin : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.White);
        }
        public override void ExSD()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 1;
            Item.master = true;
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().drawUseableItemIcon = Type;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.IsAir || item is null)
                    continue;
                bool isWeapon = item.damage > 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0 && !item.IsACoin && item.ammo == AmmoID.None;
                bool isAccessory = (item.accessory || item.defense > 0) && item.pick == 0 && item.axe == 0 && item.hammer == 0 && !item.IsACoin && item.ammo == AmmoID.None && !item.vanity;
                bool isTreasureBag = ItemID.Sets.BossBag[item.type];
                if (isWeapon || isAccessory || isTreasureBag)
                {
                    item.HJScarlet().purePrismLegalTarget = true;
                }
            }
        }
    }
}
