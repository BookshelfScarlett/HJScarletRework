using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Localization;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.NPCs.Enemy;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class SuicideKnife : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
        }
        public bool PsychoMode = false;
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = Item.useAnimation = 45;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.useTurn = true;
            Item.shoot = ProjectileType<SuicideKnifeInvisProj>();
            Item.knockBack = 12f;
            Item.damage = 2;
            Item.HJScarlet().CanDrawIcon = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!PsychoMode)
            {
                NetworkText local = Mod.GetLocalization(ScarletTextSets.CustomDeath.SuicidePath).ToNetworkText();
                player.Suicide(PlayerDeathReason.ByCustomReason(local), 99999, 0, true, true);
                SoundEngine.PlaySound(SoundID.PlayerKilled, position);
            }
            return false;
        }
        public override bool CanRightClick()
        {
            return Main.keyState.PressingShift();
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
        }
        public override void RightClick(Player player)
        {
            PsychoMode = !PsychoMode;
            if (PsychoMode)
            {

                string suffix = this.GetLocalizedValue("AltDisplayName");
                Item.SetNameOverride(suffix);
                Item.noMelee = false;
                Item.noUseGraphic = false;
            }
            else
            {
                string suffix = this.GetLocalizedValue("DisplayName");
                Item.SetNameOverride(suffix);
                Item.SetUpNoUseGraphicItem();

            }
        }
        public override void UseItemFrame(Player player)
        {
            if (!PsychoMode)
                return;
            if (player.itemAnimation == 1)
            {
                NetworkText local = Mod.GetLocalization(ScarletTextSets.CustomDeath.SuicidePath).ToNetworkText();
                player.Suicide(PlayerDeathReason.ByCustomReason(local), 99999, 0, true, true);

            }
        }
        public override void UpdateInventory(Player player)
        {
            if (Main.zenithWorld)
                Item.damage = 777777;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.zenithWorld)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    TooltipLine line = tooltips[i];
                    if (line.Name == "Damage" && line.Mod == "Terraria")
                    {
                        string suffix = "";
                        int space = tooltips[i].Text.IndexOf(' ');
                        if (space >= 0)
                            suffix = tooltips[i].Text.Substring(space);

                        string fakeDisplayDamage = "77777";
                        tooltips[i].Text = $"{fakeDisplayDamage}{suffix}";
                        break;
                    }
                }
            }
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {

        }
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (PsychoMode)
            {
                NetworkText local = Mod.GetLocalization(ScarletTextSets.CustomDeath.SuicidePath).ToNetworkText();
                target.KillMe(PlayerDeathReason.ByCustomReason(local), 99999, 0, true);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.IronBar, 10).
                AddTile(TileID.WorkBenches).
                Register();

        }
    }
}
