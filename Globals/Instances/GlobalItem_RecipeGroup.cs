using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletRecipeGroup : ModSystem
    {
        public static RecipeGroup AnyCopperBar;
        public static RecipeGroup AnyMagicHat;
        public static RecipeGroup AnyMechBossSoul;
        public override void Load()
        {
            CreateRecipeGroup(ref AnyCopperBar,  nameof(AnyCopperBar), ItemID.CopperBar, ItemID.TinBar);
            CreateRecipeGroup(ref AnyMagicHat, nameof(AnyMagicHat), ItemID.MagicHat, ItemID.WizardHat, ItemID.WizardsHat);
            CreateRecipeGroup(ref AnyMechBossSoul, nameof(AnyMechBossSoul), ItemID.SoulofFright, ItemID.SoulofSight, ItemID.SoulofMight);
        }
        public override void Unload()
        {
            AnyCopperBar = null;
            AnyMagicHat = null;
            AnyMechBossSoul = null;
        }
        public static void CreateRecipeGroup(ref RecipeGroup rg, string name, params int[] AllItem)
        {
            Func<string> creator = () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(AllItem[0])}";
            rg = new RecipeGroup(creator, AllItem);
            RecipeGroup.RegisterGroup("ScarletRework:" + name, rg);
        }

    }
}
