using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class ProvidenceHolyWater : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.White);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.healMana = 10;
            Item.consumable = false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override void GetHealMana(Player player, bool quickHeal, ref int healValue)
        {
            healValue = player.HJScarlet().providenceHolyWaterHealMana + 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().drawUseableItemIcon = Type;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.IsAir || item is null)
                    continue;
                bool isWeapon = item.damage < 1 && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.healMana > 0;
                if (isWeapon && item.type != Type)
                {
                    item.HJScarlet().purePrismLegalTarget = true;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GreaterManaPotion, 30).
                AddIngredient(ItemID.Star, 30).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
