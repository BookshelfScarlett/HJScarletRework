using Terraria.ID;
using Terraria;
using HJScarletRework.Globals.Methods;
using Terraria.Localization;
using Terraria.ModLoader;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;

namespace HJScarletRework.Items.Accessories
{
    public class RewardofWarrior : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;

        }
        public float MeleeStat = 0.10f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeStat.ToPercent());
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().Player_RewardofWarrior = true;
            player.GetDamage<MeleeDamageClass>() += MeleeStat;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeStat;
            player.GetCritChance<MeleeDamageClass>() += MeleeStat * 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WarriorEmblem, 1).
                AddIngredient(ItemID.DestroyerEmblem, 1).
                AddTile(TileID.MythrilAnvil).
                Register();

        }
    }
}
