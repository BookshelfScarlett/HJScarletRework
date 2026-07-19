using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Vanity;
using HJScarletRework.Items.Vanity.Arceca;
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
            Item.rare = RarityType<VanityEffectClass>();
            Item.consumable = true;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                VanityEffectClass.DrawItemName(line, new VanityData(Color.Gold, Color.Lerp(Color.Gold, Color.White, 0.5f), Color.Black), Color.Gold, Color.Black);
                return false;
            }
            if (line.Mod == "Terraria")
            {
                if (line.Name == "Tooltip3")
                {
                    VanityData vanityData = new VanityData(
                        Color.Lerp(Color.White, Color.DarkRed, 0.35f),
                        Color.Lerp(Color.White, Color.IndianRed, 0.95f),
                        Color.White);
                    VanityEffectClass.DrawMisc(line, vanityData, Color.White, Color.IndianRed);

                    return false;
                }

                if (line.Name == "Tooltip4")
                {
                    VanityData vanityData =
                         new VanityData(Color.RoyalBlue, Color.Lerp(Color.White, Color.DeepSkyBlue, 0.65f), Color.Black);

                    VanityEffectClass.DrawMisc(line, vanityData, Color.DeepSkyBlue, Color.Black);
                    return false;

                }
                if (line.Name == "Tooltip5")
                {
                    VanityData vanityData = new VanityData(
                        Color.Gold, Color.Lerp(Color.Gold, Color.White, .5f), Color.Black);
                    VanityEffectClass.DrawMisc(line, vanityData, Color.Gold, Color.Black);
                    return false;
                }
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
