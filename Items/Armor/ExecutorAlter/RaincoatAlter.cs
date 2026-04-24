using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class RaincoatHelmet : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.RainHat;
        public static int Defense = 4;
        public override string SetupName => "Raincoat";
        public override ArmorType Category => ArmorType.Helmet;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += 5f;
        }
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override bool SetUpArmorSet => true;
        public override int[] ArmorSlots => [ApplyArmor, ItemID.RainCoat];
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            player.HJScarlet().raincoatExecutor = true;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            player.setBonus += "\n" + armorCategory.ToLangValue();


        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.Sawmill).
                DisableDecraft().
                Register();
            
        }
    }
    public class RaincoatChestplate : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.RainCoat;
        public static int Defense = 8;
        public override string SetupName => "Raincoat";
        public override ArmorType Category => ArmorType.Chestplate;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }
        public override void ExSD(Item item)
        {
            item.defense = Defense; 
        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Silk, 15).
                AddTile(TileID.Sawmill).
                DisableDecraft().
                Register();
        }
    }
}
