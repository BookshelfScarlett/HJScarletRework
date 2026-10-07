using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace HJScarletRework.Items.Weapons.Executor.Firearm
{
    public class Exsanguination : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 300;
        public override float ExecutionStrikeDamageMult => 1;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override bool BlockTextboxDetail()
        {
            return RangerMode;
        }
        public bool RangerMode = false;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
            ScarletItemIDSets.ForceToAutomaticExecute[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 24;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<ExsanguinationHeldProj>();
            Item.shootSpeed = 12f;
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Developer;
            Item.HJScarlet().OwnerName = "绯色书架 ScarletShelf";
        }
        public override bool CanRightClick()
        {
            return Main.keyState.PressingShift();
        }
        public override void RightClick(Player player)
        {
            RangerMode = !RangerMode;
            if (RangerMode)
                Item.DamageType = DamageClass.Ranged;
            else
                Item.DamageType = ExecutorDamageClass.Instance;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(RangerMode), RangerMode);
        }
        public override void LoadData(TagCompound tag)
        {
            RangerMode = tag.GetBool(nameof(RangerMode));
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(RangerMode);
        }
        public override void NetReceive(BinaryReader reader)
        {
            RangerMode = reader.ReadBoolean();
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.CreateHoldShiftRightClickTooltip();
        }
        public override void HoldItem(Player player)
        {
            int heldProjType = ProjectileType<ExsanguinationHeldProj>();
            if (RangerMode)
            {
                Item.DamageType = DamageClass.Ranged;
                Item.useAmmo = AmmoID.Bullet;
            }
            else
            {
                Item.DamageType = ExecutorDamageClass.Instance;
                Item.useAmmo = AmmoID.None;
            }
            if (player.HasProj(heldProjType))
                return;
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            if (RangerMode)
                projDamage = (int)player.GetTotalDamage<RangedDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, heldProjType, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Megashark).
                AddIngredient<UniversalCube>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
