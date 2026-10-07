using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{

    [AutoloadEquip(EquipType.Head)]
    public class DiverHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemType<DiverBody>(), ItemType<DiverLegs>()];
        public override bool SetUpArmorSet => true;
        public float DamageAdd = .05f;
        public override void ExSD()
        {
            Item.defense = 6;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAdd.ToPercent());
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.HJScarlet().diverArmor = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += DamageAdd;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemType<AnglerCoin>(), 36).
                AddTile(TileType<FishmenFreeMarketTradingSystem>()).
                Register();
        }
    }
}
