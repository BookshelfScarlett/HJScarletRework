using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class CowboyHelmet : AlterVanillaArmor
    {
        public static int Defense = 5;
        public override int ApplyArmor => ItemID.CowboyHat;
        public override string SetupName => "Cowboy";
        public override ArmorType Category => ArmorType.Helmet;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.10f;
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            bool correctedArmor = head.type == ItemID.CowboyHat && body.type == ItemID.CowboyJacket && legs.type == ItemID.CowboyPants;

            if (correctedArmor)
            {
                bool correctedState = head.HJScarlet().EnableExecutorVersion && body.HJScarlet().EnableExecutorVersion && legs.HJScarlet().EnableExecutorVersion;
                if (correctedState)
                    return ArmorSetName;
            }
            return base.IsArmorSet(head, body, legs);
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Orange;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            player.HJScarlet().cowboyExecutor = true;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            player.setBonus += "\n" + armorCategory.ToLangValue();
        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Silk, 10).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 5).
                AddTile(TileID.Sawmill).
                DisableDecraft().
                Register();
        }
    }
    public class CowboyChestplate : AlterVanillaArmor
    {
        public static int Defense = 7;
        public override int ApplyArmor => ItemID.CowboyJacket;
        public override string SetupName => "Cowboy";
        public override ArmorType Category => ArmorType.Chestplate;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += 10;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Silk, 15).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 10).
                AddTile(TileID.Sawmill).
                DisableDecraft().
                Register();
        }

    }
    public class CowboyLegs : AlterVanillaArmor
    {
        public static int Defense = 3;
        public override int ApplyArmor => ItemID.CowboyPants;
        public override string SetupName => "Cowboy";
        public override ArmorType Category => ArmorType.Legs;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.moveSpeed += 0.15f;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Silk, 5).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 5).
                AddTile(TileID.Sawmill).
                DisableDecraft().
                Register();
        }

    }
}
