using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class Duskfall : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.SunGod);
        }
        public override void ExSD()
        {
            Item.consumable = true;
            Item.SetUpNoUseGraphicItem(false, true);
            Item.damage = 246;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 9999;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.crit = 12;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<DuskfallProj>();
            Item.knockBack = .5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(75).
                AddIngredient<SolarFlareScoria>(5).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
