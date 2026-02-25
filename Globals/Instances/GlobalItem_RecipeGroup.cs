using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletRecipeGroup : ModSystem
    {
        public static string AnyCopperBar;
        public static string AnyMagicHat;
        public static string AnyMechBossSoul;
        public static string AnyLunarPickaxe;
        public static string AnyIceCrate;
        public override void AddRecipeGroups()
        {
            AnyCopperBar = CreateRecipeGroup(nameof(AnyCopperBar), ItemID.CopperBar, ItemID.TinBar);
            AnyMagicHat = CreateRecipeGroup(nameof(AnyMagicHat), ItemID.WizardHat, ItemID.MagicHat, ItemID.WizardsHat);
            AnyMechBossSoul = CreateRecipeGroup(nameof(AnyMechBossSoul), ItemID.SoulofFright, ItemID.SoulofSight, ItemID.SoulofMight);
            AnyLunarPickaxe = CreateRecipeGroup(nameof(AnyLunarPickaxe), ItemID.SolarFlarePickaxe, ItemID.VortexPickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe);
            AnyIceCrate = CreateRecipeGroup(nameof(AnyIceCrate), ItemID.FrozenCrate, ItemID.FrozenCrateHard);
        }
        public override void Unload()
        {
            AnyCopperBar = null;
            AnyMagicHat = null;
            AnyMechBossSoul = null;
            AnyLunarPickaxe = null;
            AnyIceCrate = null;

        }
        public static string CreateRecipeGroup(string name, params int[] AllItem)
        {
            Func<string> getName = () => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(AllItem[0]);
            RecipeGroup rec = new RecipeGroup(getName, AllItem);
            string realName = "Scarlet:" + name;
            RecipeGroup.RegisterGroup(realName, rec);
            return realName;
        }

    }
}
