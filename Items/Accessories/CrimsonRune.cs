using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonRune : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 5;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 25);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 1;
            player.longInvince = true;
            player.pStone = true;
            player.GetArmorPenetration<GenericDamageClass>() += 25f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CharmofMyths).
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
