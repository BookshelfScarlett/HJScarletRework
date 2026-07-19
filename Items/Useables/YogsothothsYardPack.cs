using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Reaper;
using HJScarletRework.Items.Vanity;
using HJScarletRework.Items.Vanity.Yards;
using HJScarletRework.Items.Weapons.Executor;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class YogsothothsYardPack : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.OpenableBag[Type] = true;
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.White);
        }
        public override void ExSD()
        {
            Item.rare = RarityType<VanityEffectClass>();
            Item.consumable = true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.AddCommon(ItemType<CrimsonScythe>());
            itemLoot.AddCommon(ItemType<ReaperHead>());
            itemLoot.AddCommon(ItemType<ReaperBody>());
            itemLoot.AddCommon(ItemType<ReaperLegs>());
            itemLoot.AddCommon(ItemType<LeafMaidItem>());
            itemLoot.AddCommon(ItemType<RedDragonItem>());
            itemLoot.AddCommon(ItemType<CantoneseGirlItem>());
        }
    }
}
