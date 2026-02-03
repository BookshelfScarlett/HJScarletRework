using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class CreationHat : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Armor;
        public override string Texture => GetAsset(AssetCategory.Armor);
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
        }
        public int DefenseCount = 110;
        public int MaxMana = 40;
        public float ManaDamage = 0.3f;
        public float ManaCrits = 50;
        public float ManaCost = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseCount, MaxMana, ManaDamage.ToPercent(), ManaCrits, ManaCost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.defense = 10;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.DiamondRobe;
        }
        public override void UpdateArmorSet(Player player)
        {
            string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBonus").ToLangValue();
            player.setBonus += value.ToFormatValue(DefenseCount, MaxMana, ManaDamage.ToPercent(), ManaCost.ToPercent(), $"{(int)ManaCrits}%");
            player.statDefense += DefenseCount;
            player.statManaMax2 += MaxMana;
            player.GetDamage<MagicDamageClass>() += ManaDamage;
            player.GetCritChance<MagicDamageClass>() += ManaCrits;
            player.manaCost -= ManaCost;
            player.HJScarlet().CreationHatSet = true;
            
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyMagicHat, 1).
                AddIngredient<CubistBar>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
