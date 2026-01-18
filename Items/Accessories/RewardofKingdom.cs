using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class RewardofKingdom : HJScarletItems
    {
        public override AssetCategory GetAssetCategory => AssetCategory.Equip;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;

        }
        public float MeleeStat = 0.10f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeStat.ToPercent());
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().Player_RewardofWarrior = true;
            player.HJScarlet().Player_RewardofKingdom = true;
            player.GetDamage<MeleeDamageClass>() += MeleeStat;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeStat;
            player.GetCritChance<MeleeDamageClass>() += MeleeStat * 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RewardofWarrior>().
                AddIngredient<EssenceofTime>(5).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile<FinalAnvil>().
                Register();

        }
    }
}
