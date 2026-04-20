using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class FloretProtectorHelmetAlter: AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorHelmet;
        public static int Defense = 40;
        public override string SetupName => "FloretProtector";
        public override ArmorType Category => ArmorType.Helmet;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.4f;
            player.GetCritChance<ExecutorDamageClass>() += 40f;
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            bool correctedArmor = head.type == ItemID.FloretProtectorHelmet && body.type == ItemID.FloretProtectorChestplate && legs.type == ItemID.FloretProtectorLegs;

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
            item.rare = ItemRarityID.Red;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            player.statLifeMax2 += 100;
            player.HJScarlet().floretProtectorExecutor = true;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            string armorCategory2 = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.BuffDetail");
            string value;
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                value = armorCategory2;
            else
                value = armorCategory;
            player.setBonus += "\n" + value.ToLangValue();
        }
    }
    public class FlorectProtectorChestplateAlter: AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorChestplate;
        public static int Defense = 60;
        public override ArmorType Category => ArmorType.Chestplate;
        public override string SetupName => "FloretProtector";
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.10f;
            player.noKnockback = true;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Red;
        }
    }
    public class FlorectProtectorLegsAlter: AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorLegs;
        public static int Defense = 30;
        public override string SetupName => "FloretProtector";
        public override ArmorType Category => ArmorType.Legs;
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.05f;
            player.GetCritChance<ExecutorDamageClass>() += 5;
            player.moveSpeed += 0.20f;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Red;
        }
    }
}
