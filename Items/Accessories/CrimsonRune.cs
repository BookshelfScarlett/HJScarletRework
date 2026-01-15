using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonRune : HJScarletItems
    {
        public override ItemCategory ItemCate => ItemCategory.Accessories;
        public override string Texture => HJScarletItemProj.Equip_CrimsonRune.Path;
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
        public virtual ItemCategory ItemCate { get; }
        public new string LocalizationCategory => $"Items.{ItemCate}";
    }
}
