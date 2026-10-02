using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class HandofGodsSystem : ModSystem
    {
        public override void Load()
        {
            On_Player.PlaceThing_Tiles += new On_Player.hook_PlaceThing_Tiles(PlaceThing_Tiles);
            On_Player.PlaceThing_Walls += new On_Player.hook_PlaceThing_Walls(PlaceThing_Walls);
        }

        private void PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player player)
        {
            if (!player.HJScarlet().handOfGods)
            {
                orig.Invoke(player);
                return;
            }
            Tile tileSafely = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            ushort num = tileSafely.WallType;
            tileSafely.WallType = WallID.Wood;
            orig.Invoke(player);
            tileSafely.WallType = num;
        }

        private void PlaceThing_Walls(On_Player.orig_PlaceThing_Walls orig, Player player)
        {

            if (!player.HJScarlet().handOfGods)
            {
                orig.Invoke(player);
                return;
            }
            Tile tileSafely = Framing.GetTileSafely(Player.tileTargetX - 1, Player.tileTargetY);
            bool hasTile = tileSafely.HasTile;
            tileSafely.HasTile = true;
            orig.Invoke(player);
            tileSafely.HasTile = hasTile;
        }
    }
    public class HandofGods : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public float TileSpeed = .5f;
        public float WallSpeed = .5f;
        public float PickSpeed = .5f;
        public int BlockRange = 9999;
        public float PickRange = 50;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateWhite);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TileSpeed.ToPercent(), WallSpeed.ToPercent(), PickSpeed.ToPercent(), BlockRange);
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().handOfGods = true;
            player.blockRange += BlockRange;
            player.tileSpeed += TileSpeed;
            player.wallSpeed += WallSpeed;
            player.pickSpeed -= PickSpeed;
            player.dangerSense = true;
            player.nightVision = true;
            player.detectCreature = true;
            Lighting.AddLight((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f), 2f, 2f, 2f);
            Lighting.AddLight(player.Center, Color.White.ToVector3() * 3);
        }
        public override void UpdateVanity(Player player)
        {
            player.dangerSense = true;
            player.nightVision = true;
            player.detectCreature = true;
            Lighting.AddLight((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f), 2f, 2f, 2f);
            Lighting.AddLight(player.Center, Color.White.ToVector3() * 3);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HandOfCreation).
                AddIngredient<PrisonSearchlight>().
                AddIngredient<AbyssCore>().
                AddIngredient<EssenceofMatter>(15).
                AddTile<FountainofMatter>().
                Register();
        }
    }
}
