using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Vanity;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class ArcaeaPack : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.OpenableBag[Type] = true;
        }
        public override void ExSD()
        {
            Item.rare = ItemRarityID.Red;
            Item.consumable = true;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.IsItemName())
            {
                VanityItemRarity.DrawItemName(line, Color.Gold, Color.Lerp(Color.Gold, Color.White, 0.5f), Color.Black, RandLerpColor(Color.Gold, Color.Black));
                return false;
            }
            return true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.AddLoot<TairitsuItem>();
            itemLoot.AddLoot<HikariItem>();
        }
    }
}
