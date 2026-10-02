using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class CombatSlotAdvanced : HJScarletItemClass
    {
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
