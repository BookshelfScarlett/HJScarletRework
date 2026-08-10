using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BlazingSun : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Solar);
        }
        public override void ExSD()
        {
            Item.damage = 60;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem(true, false);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<BlazingSunHeldProj>();
            Item.shootSpeed = 12f;
        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {

            if (player.HasProj(Item.shoot))
                return;
            int damage = (int)player.GetTotalDamage<RangedDamageClass>().ApplyTo(Item.damage);
            Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, damage, Item.knockBack, player.whoAmI, ai0: 9);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenFury).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
