using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Placables.FishingCrate;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using HJScarletRework.Items.Weapons.Executor.Thrown;
using HJScarletRework.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public class HJScarletRecipeGroup : ModSystem
    {
        public static string AnyCopperBar;
        public static string AnyMagicHat;
        public static string AnyMechBossSoul;
        public static string AnyLunarPickaxe;
        public static string AnySpearofDarkness;
        public static string AnyEvilHammer;
        public static string AnyLifeCrystal;
        public static string AnyGoldCritter;
        public static string AnyGoldBar;
        public static string AnyEvilScale;
        public static string AnyEvilBar;
        public static string AnyCobaltBar;
        public static string AnyGoldSword;
        public static string AnyPostPlantEmblem;
        public static string AnyTitaniumBar;
        public static string AnyBiomeKey;
        #region Crates
        public static string AnyIceCrate;
        public static string AnyJungleCrate;
        public static string AnyDungeonCrate;
        public static string AnyDarkCrate;
        public static string AnyLivingCrate;
        public static string AnyClassEmblem;
        #endregion
        public override void AddRecipeGroups()
        {
            int[] goldList =
            [
                ItemID.GoldBird,
                ItemID.GoldGoldfish,
                ItemID.GoldGrasshopper,
                ItemID.GoldFrog,
                ItemID.GoldBunny,
                ItemID.GoldMouse,
                ItemID.GoldWorm,
                ItemID.GoldButterfly,
                ItemID.GoldLadyBug,
                ItemID.GoldWaterStrider,
                ItemID.GoldenCarp,
                ItemID.GoldDragonfly,
                ItemID.GoldSeahorse,
                ItemID.SquirrelGold //relogic我草拟吗
            ];
            AnyCopperBar = CreateRecipeGroup(nameof(AnyCopperBar), ItemID.CopperBar, ItemID.TinBar);
            AnyMagicHat = CreateRecipeGroup(nameof(AnyMagicHat), ItemID.WizardHat, ItemID.MagicHat, ItemID.WizardsHat);
            AnyMechBossSoul = CreateRecipeGroup(nameof(AnyMechBossSoul), ItemID.SoulofFright, ItemID.SoulofSight, ItemID.SoulofMight);
            AnyLunarPickaxe = CreateRecipeGroup(nameof(AnyLunarPickaxe), ItemID.SolarFlarePickaxe, ItemID.VortexPickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe);
            AnyIceCrate = CreateRecipeGroup(nameof(AnyIceCrate), ItemID.FrozenCrate, ItemID.FrozenCrateHard);
            AnyJungleCrate = CreateRecipeGroup(nameof(AnyJungleCrate), ItemID.JungleFishingCrate, ItemID.JungleFishingCrateHard);
            AnyDungeonCrate = CreateRecipeGroup(nameof(AnyDungeonCrate), ItemID.DungeonFishingCrate, ItemID.DungeonFishingCrateHard);
            AnySpearofDarkness = CreateRecipeGroup(nameof(AnySpearofDarkness), ItemType<SpearofDarknessThrown>(), ItemType<SpearOfDarkness>());
            AnyEvilHammer = CreateRecipeGroup(nameof(AnyEvilHammer), ItemType<TheDefiler>(), ItemType<FleshGrinder>());
            AnyLifeCrystal = CreateRecipeGroup(nameof(AnyLifeCrystal), ItemID.LifeCrystal, ItemID.HeartLantern);
            AnyGoldCritter = CreateRecipeGroup(nameof(AnyGoldCritter), goldList);
            AnyGoldBar = CreateRecipeGroup(nameof(AnyGoldBar), ItemID.GoldBar, ItemID.PlatinumBar);
            AnyEvilScale = CreateRecipeGroup(nameof(AnyEvilScale), ItemID.ShadowScale, ItemID.TissueSample);
            AnyEvilBar = CreateRecipeGroup(nameof(AnyEvilBar), ItemID.DemoniteBar, ItemID.CrimtaneBar);
            AnyCobaltBar = CreateRecipeGroup(nameof(AnyCobaltBar), ItemID.CobaltBar, ItemID.PalladiumBar);
            AnyGoldSword = CreateRecipeGroup(nameof(AnyGoldSword), ItemID.GoldBroadsword, ItemID.PlatinumBroadsword);
            AnyPostPlantEmblem = CreateRecipeGroup(nameof(AnyPostPlantEmblem), ItemType<EmblemColdSteel>(), ItemType<EmblemThrown>());
            AnyTitaniumBar = CreateRecipeGroup(nameof(AnyTitaniumBar), ItemID.TitaniumBar, ItemID.AdamantiteBar);
            AnyDarkCrate = CreateRecipeGroup(nameof(AnyDarkCrate), ItemType<MazeCrate>(), ItemType<MistyCrate>(), ItemType<ShadowCrate>());
            AnyBiomeKey = CreateRecipeGroup(nameof(AnyBiomeKey), ItemID.CorruptionKey, ItemID.CrimsonKey, ItemID.FrozenKey, ItemID.JungleKey, ItemID.HallowedKey, ItemID.DungeonDesertKey);
            AnyLivingCrate = CreateRecipeGroup(nameof(AnyLivingCrate), ItemType<LivingCrate>(), ItemType<MembraneCrate>());
            AnyClassEmblem = CreateRecipeGroup(nameof(AnyClassEmblem), ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem);
        }
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                CopyTheRecipe(recipe, ItemID.RocketLauncher, ItemType<Sundowner>());
                CopyTheRecipe(recipe, ItemID.FlintlockPistol, ItemType<TheCompanion>());
            }
        }
        public void CopyTheRecipe(Recipe recipe, int keyIngredient, int oriResultItem)
        {
            if (recipe.HasIngredient(keyIngredient) && !recipe.HasResult(oriResultItem))
            {
                Recipe c = recipe.Clone();
                c.RemoveIngredient(keyIngredient);
                c.AddIngredient(oriResultItem);
                c.DisableDecraft();
                c.Register();
            }
        }
        public override void Unload()
        {
            AnyCopperBar = null;
            AnyMagicHat = null;
            AnyMechBossSoul = null;
            AnyLunarPickaxe = null;
            AnyIceCrate = null;
            AnyDungeonCrate = null;
            AnyJungleCrate = null;
            AnySpearofDarkness = null;
            AnyEvilHammer = null;
            AnyLifeCrystal = null;
            AnyGoldCritter = null;
            AnyGoldBar = null;
            AnyEvilScale = null;
            AnyEvilBar = null;
            AnyCobaltBar = null;
            AnyGoldSword = null;
            AnyPostPlantEmblem = null;
            AnyTitaniumBar = null;
            AnyDarkCrate = null;
            AnyLivingCrate = null;
            AnyClassEmblem = null;
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
