using HJScarletRework.Core.NetCode;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Localization;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Projs.General;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class PinballPurgatory : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static int PinkType = 4;
        public static int OrangeType = 3;
        public static int BlueType = 2;
        public static int WhiteType = 1;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Pink);
            Item.accessory = true;
            Item.defense = 10;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.CreateTooltipDirect(ScarletTextSets.GeneralText_BuffShow.ToLangValue(), Color.LightGray, Mod, "DetailLineName");
            string value = this.GetLocalizationKey("DetailTootlip").ToLangValue();
            if (Main.keyState.PressingAlt())
                tooltips.ReplaceAllTooltip(value, Color.White);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noFallDmg = true;
            player.HJScarlet().mayaPumper = true;
            player.HJScarlet().mayaPumperParty = true;
            if (player.HJScarlet().mayaPumperDashTime > 0)
            {
                if (player.HJScarlet().mayaPumperDashType != OrangeType && player.HJScarlet().mayaPumperDashType != WhiteType)
                    player.ApplyDash(GetInstance<CelesteDash>().Type);
                if (player.IsOwnerSide() && !player.HasProj<PinballPurgatoryTimer>())
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<PinballPurgatoryTimer>(), 0, 0, player.whoAmI);
                    proj.timeLeft = player.HJScarlet().mayaPumperDashTime;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MayaBall>().
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.Diamond, 10).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
