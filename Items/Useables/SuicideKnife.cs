using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.NPCs.Enemy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class SuicideKnife : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = Item.useAnimation = 15;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetUpNoUseGraphicItem();
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.useTurn = true;
            Item.shoot = ProjectileType<SuicideKnifeInvisProj>();
            Item.knockBack = 12f;
            Item.damage = 7777;
        }
        public override bool? UseItem(Player player)
        {
            return true;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {

            if (player.itemAnimation % 5 == 0)
            {
                for (int i = 0; i < 5; i++)
                    new ShinyOrbParticle(new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y) + Main.rand.NextVector2Square(0, hitbox.Width / 2), RandVelTwoPi(1f, 2.4f), Color.Orange, 40, 0.45f, affactedByGravity: true).Spawn();
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.IronBar,10).
                AddTile(TileID.WorkBenches).
                Register();

        }
    }
}
