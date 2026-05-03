using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Magic;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class Corona :HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Magic;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true, false);
            Item.damage = 42;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.rare = RarityType<SolarRarity>();
            Item.knockBack = 4f;
            Item.UseSound = null;
            Item.useStyle = ItemUseStyleID.Swing;
            //会影响攻速的。
            Item.useTime = Item.useAnimation = 30;
            //仅仅一个标记作用，实际上这个东西不会实际发射射弹。
            Item.shoot = ProjectileType<CoronaHeldProj>();
            Item.shootSpeed = 18f;
            Item.mana = 54;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot) && Main.myPlayer == player.whoAmI)
                return;
            for (int i = 0; i < 16; i++)
            {
                new ShinyCrossStar(player.Center.ToRandCirclePos(8f), -Vector2.UnitY.ToRandVelocity(ToRadians(25f), 4.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false,0.2f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                new SmokeParticle(player.Center.ToRandCirclePos(10f), -Vector2.UnitY.ToRandVelocity(ToRadians(20f), 4.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriority();
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = -0.25f }, player.Center);
            int projDamage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot,0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalBall).
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
