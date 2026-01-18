using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletRecipeGroup : ModSystem
    {
        public static RecipeGroup AnyCopperBar;
        public override void Load()
        {
            CreateRecipeGroup(ref AnyCopperBar,  nameof(AnyCopperBar), ItemID.CopperBar, ItemID.TinBar);
        }
        public override void Unload()
        {
            AnyCopperBar = null;
        }
        public static void CreateRecipeGroup(ref RecipeGroup rg, string name, params int[] AllItem)
        {
            Func<string> creator = () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(AllItem[0])}";
            rg = new RecipeGroup(creator, AllItem);
            RecipeGroup.RegisterGroup("ScarletRework:" + name, rg);
        }

    }
}
