using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Projs.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class PandorasBurger : HJScarletWeaponoutItemClass
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Supporter);
        }
        public override void ExSD()
        {
            Item.damage = 114514;
            Item.useTime = Item.useAnimation = 40;
            Item.shoot = ProjectileType<PandorasBurgerHeldProj>();
            Item.shootSpeed = 16f;
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Supporter;
            Item.HJScarlet().OwnerName = "锯角";
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            //float ratios = HJScarletMethods.GetDamageBonusRatio(114514, );
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Burger, 5).
                AddIngredient<SwordmasterBadge>().
                AddIngredient<BullseyeBadge>().
                AddIngredient<ArchmageBadge>().
                AddIngredient<CounsellorBadge>().
                AddIngredient<ExecutorBadge>().
                AddIngredient<EssenceofBright>(30).
                AddIngredient<EssenceofDarkness>(30).
                AddIngredient<EssenceofDeath>(30).
                AddIngredient<EssenceofLife>(30).
                AddIngredient<EssenceofMatter>(30).
                AddIngredient<EssenceofNothingness>(30).
                AddIngredient<EssenceofTime>(30).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
