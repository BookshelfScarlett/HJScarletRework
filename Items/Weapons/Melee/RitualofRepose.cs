using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Core.NetSync;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class RitualofRepose : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.Hallowed);
            ScarletItemIDSets.GrantsBoosterAfterSon[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 1546;
            Item.SetUpNoUseGraphicItem(false);
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<RitualofReposeProj>();
            Item.shootSpeed = 16f;
        }
        public override Color MainTooltipColor => Color.LightGoldenrodYellow;
        public override bool CanUseItem(Player player)
        {
            return !(player.HasProj<RitualofReposeRest>());
        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            player.longInvince = true;
            ScreenDarknessSystem.AddScreenDarkness(0.80f, 10, 2, 5, easeOut: EaseInCubic, holdCondition: () => Main.LocalPlayer.IsHolding(Type));
            if (player.HasProj(Item.shoot) || player.HasProj<RitualofReposeRest>())
                if (!player.IsOwnerSide())
                    return;
            if (player.HasProj(Item.shoot))
                return;
            int dmg = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.MountedCenter, Vector2.Zero, Item.shoot, dmg, Item.knockBack, player.whoAmI);

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient<SpearOfEscape>().
                AddIngredient<EssenceofNothingness>(10).
                AddIngredient<EssenceofDeath>(10).
                AddTile(FinalAnvilTile).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient<SpearofEscapeThrown>().
                AddIngredient<EssenceofNothingness>(10).
                AddIngredient<EssenceofDeath>(10).
                AddTile(FinalAnvilTile).
                Register();

        }
    }
}
