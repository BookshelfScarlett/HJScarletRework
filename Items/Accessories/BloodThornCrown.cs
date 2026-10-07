using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodThornCrown : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static float DR = .25f;
        public static float DamageReduce = .25f;
        public static int Crit = 25;
        public static float DamageMult = .5f;
        public static int MaxHitCounter = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DR.ToPercent(), DamageReduce.ToPercent(), Crit + "%", DamageMult.ToPercent(), MaxHitCounter);
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.ScarletRed);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().bloodThronCrown = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrownofSilveryLight>(15).
                AddIngredient<TankOfThePastHallow>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
