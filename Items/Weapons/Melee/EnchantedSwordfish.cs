using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EnchantedSwordfish : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
            ItemID.Sets.Spears[Type] = true;
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.damage = 154;
            Item.SetUpNoUseGraphicItem(true);
            Item.UseSound = SoundID.Item1;
            Item.knockBack = .5f;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 2.5f;
            Item.shoot = ProjectileType<EnchantedSwordfishHeldProj>();
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool? UseItem(Player player)
        {
            // Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }
            return null;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Swordfish).
                AddIngredient(ItemID.FallenStar, 300).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
