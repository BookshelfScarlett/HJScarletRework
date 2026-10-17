using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.GlowMushroom
{
    [AutoloadEquip(EquipType.Body)]
    public class GlowMushroomBody : HJScarletArmor
    {
        public float Damage = .05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        //public override void SetStaticDefaults()
        //{
        //    //ArmorIDs.Body.Sets.
        //}
        public override void ExSD()
        {
            Item.defense =5;
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += Damage;
            Lighting.AddLight(player.Center, Color.White.ToVector3() * 0.6f);

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GlowingMushroom, 25).
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
