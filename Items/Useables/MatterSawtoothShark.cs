using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class MatterSawtoothShark : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.Matter);
            ItemID.Sets.IsChainsaw[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.damage = 888;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 15;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<MatterSawtoothSharkHeldProj>();
            Item.tileBoost = 10;
            Item.axe = 55;
            Item.knockBack = .5f;
            Item.UseSound = SoundID.Item23;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
    }
}
