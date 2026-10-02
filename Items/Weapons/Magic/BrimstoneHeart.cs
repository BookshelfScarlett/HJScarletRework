using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class BrimstoneHeart : HJScarletWeaponoutItemClass
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public bool StopKilling = false;
        public override void ExSSD()
        {
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Donator);
        }
        public override void ExSD()
        {
            Item.damage = 124;
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Donator;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.HJScarlet().OwnerName = "苏利";
            Item.knockBack = 2;
            Item.mana = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 60;
            Item.shoot = ProjectileType<BrimstoneHeartHeldProj>();
            Item.shootSpeed = 16f;
            Item.crit = 20;
        }
        public override void UpdateInventory(Player player)
        {
            if (player.name == Item.HJScarlet().OwnerName)
            {
                Item.SetNameOverride("永恒燃烧的爱");
            }
            base.UpdateInventory(player);
        }
        public override bool CanRightClick()
        {
            return Main.keyState.PressingShift();
        }
        public override void RightClick(Player player)
        {
            player.HJScarlet().brimstoneHeartKilling = !player.HJScarlet().brimstoneHeartKilling;
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
            if (!player.HJScarlet().brimstoneHeartKilling)
            {
                player.buffImmune[BuffID.Regeneration] = true;
                if (player.lifeRegen > 0)
                    player.lifeRegen /= 2;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ObsidianRose).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
