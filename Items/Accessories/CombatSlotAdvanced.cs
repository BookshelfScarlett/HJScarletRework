using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class CombatSlotAdvanced : HJScarletItemClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.accessory = true;
            Item.value = Item.sellPrice(10, 0, 0, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().combatSlot2 = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CombatSlot>().
                AddIngredient<UniversalCube>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
