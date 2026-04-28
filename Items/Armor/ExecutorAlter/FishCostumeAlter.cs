using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class FishCostumeHelmet : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeMask;
        public static int Defense = 6;
        public override string SetupName => "FishCostume";
        public override bool SetUpArmorSet => true;
        public override int[] ArmorSlots => [ItemID.FishCostumeMask, ItemID.FishCostumeShirt, ItemID.FishCostumeFinskirt];
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            player.setBonus += "\n" + armorCategory.ToLangValue();
            player.HJScarlet().fishExecutor = true;
            player.HJScarlet().NoSlowFall = 10;
            player.ignoreWater = true;
            //bro.
            player.noFallDmg = true;
            //某些不知道的原因，这样可以检测玩家是否按下了跳跃
            //非常奇怪。
            if (player.controlJump)
            {
                if (player.releaseJump)
                {
                    SoundEngine.PlaySound(SoundID.DoubleJump with { MaxInstances = 0 }, player.Center);
                    for (int i = 0; i < 16; i++)
                    {
                        Dust d = Dust.NewDustPerfect(player.ToRandRec(), DustID.GemSapphire);
                        d.velocity = player.velocity.ToSafeNormalize() * Main.rand.NextFloat(1.2f, 2.4f);
                        d.scale = 1f;
                        d.noGravity = true;
                    }
                }
            }
            //是的，无限飞。
            player.wingTime = player.wingTimeMax;
            //从原版代码复制过来的
            player.gravity = 0.43f;
            player.HJScarlet().maxFallspeedModify += 10;
            Player.jumpHeight = 60;
            Player.jumpSpeed = 10.0f;
            player.moveSpeed += 0.5f;
            if (!player.wet)
            {
                player.wet = true;
                player.wetCount = 10;
            }
            if (Collision.LavaCollision(player.position, player.width, player.height))
            {
                if (player.miscCounter % 5 == 0)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), Color.OrangeRed, -5);
                    player.statLife -= 5;
                }
            }
            else
            {
                if (Main.rand.NextBool())
                {
                    Dust d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Water);
                    d.velocity = Vector2.UnitY;
                }
            }
        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Bass, 5).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 5).
                AddTile(TileID.Hellforge).
                DisableDecraft().
                Register();

        }
    }
    public class FishCostumeChestplate : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeShirt;
        public static int Defense = 12;
        public override string SetupName => "FishCostume";
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Bass, 12).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 12).
                AddTile(TileID.Hellforge).
                DisableDecraft().
                Register();

        }
    }
    public class FishCostumeLegs : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeFinskirt;
        public static int Defense = 4;
        public override string SetupName => "FishCostume";
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void AddRecipes()
        {
            Recipe.Create(ApplyArmor).
                AddIngredient(ItemID.Bass, 8).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 8).
                AddTile(TileID.Hellforge).
                DisableDecraft().
                Register();

        }

    }
}
