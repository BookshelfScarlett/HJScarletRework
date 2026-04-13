using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorLauncher : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 32;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.accessory = true;
            Item.defense = 5;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyDoT = true;
            player.HJScarlet().blackKeyDefenseBuff = 0.50f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreBar, 10).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
