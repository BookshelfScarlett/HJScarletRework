using HJScarletRework.Globals.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonRune : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 5;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 1;
            player.longInvince = true;
            player.pStone = true;
        }
    }
    public abstract class HJScarletItems : ModItem, ILocalizedModType
    {
        public virtual ItemCategory LocalCategory { get; }
        public new string LocalizationCategory => $"Items.{LocalCategory}";
        public static string AssetPath => $"HJScarletRework/Assets/Texture/Items";
        public override string Texture => GetAsset(AssetCategory.Equip);
        public string GetAsset(AssetCategory assetCategory)
        {
            string path = $"{AssetPath}/{assetCategory}s/{GetType().Name}";
            return path;
        }
    }
}
