using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class CreationHat : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
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
        public override void ExSD()
        {
            Item.width = 38;
            Item.height = 22;
            Item.defense = 10;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            List<int> robes =
            [
                ItemID.AmethystRobe,
                ItemID.TopazRobe,
                ItemID.AmberRobe,
                ItemID.EmeraldRobe,
                ItemID.SapphireRobe,
                ItemID.RubyRobe,
                ItemID.DiamondRobe,
                ItemType<OnyxRobe>()
            ];
            return robes.Contains(body.type);
        }
        public override void UpdateArmorSet(Player player)
        {
            string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBonus").ToLangValue();
            player.setBonus += "\n" + value.ToFormatValue(DefenseCount, MaxMana, ManaDamage.ToPercent(), ManaCost.ToPercent(), $"{(int)ManaCrits}%");
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
