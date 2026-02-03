using ContinentOfJourney.Items;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class SuicideKnife : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Useable;
        public override string Texture => GetAsset(AssetCategory.Useable);
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = Item.useAnimation = 15;
            Item.scale *= 1.2f;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.useTurn = true;
            Item.knockBack = 12f;
            Item.damage = 100;
        }
        public override bool? UseItem(Player player)
        {
            HJScarletMethods.KillPlayer(player);
            return true;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            
            if(player.itemAnimation % 5 == 0)
            {
                for (int i = 0; i < 5; i++)
                    new ShinyOrbParticle(new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y) + Main.rand.NextVector2Square(0, hitbox.Width / 2), RandVelTwoPi(1f, 2.4f), Color.Orange, 40, 0.45f, affactedByGravity: true).Spawn();
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenPickaxe).
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();

        }
    }
}
