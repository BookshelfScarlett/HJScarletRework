using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class GaiaStriker : ExecutorWeaponClass
    {
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        /// <summary>
        /// 调整这个以修改普通攻击血弹的爆裂次数
        /// 需注意的是血弹总会有两枚用于治疗玩家
        /// </summary>
        public static int BloodBulletCount = 8;
        /// <summary>
        /// 处决进程要求实际上为18次，因为每个非治疗性的血弹都会叠加
        /// </summary>
        public override int ExecutionProgress => 18 * (BloodBulletCount - 2);
        /// <summary>
        ///<para>盖亚重锤普通攻击时，每个爆开的血弹提供的治疗量</para>
        ///<para>处决攻击下默认取两倍</para>
        /// </summary>
        public static int BloodBulletHealNormal = 1;
        /// <summary>
        /// 盖亚宝箱爆开时，每个爆开的血弹提供的治疗量
        /// </summary>
        public static int BloodBulletHealLootChest = 100;
        /// <summary>
        /// 盖亚重锤仆从死亡时，每个爆开的血弹提供的治疗量
        /// </summary>
        public static int BloodBulletHealMinionDead = 5;
        public static int BloodBulletHealMinionDeadEarly = 10;
        public override void ExSSD()
        {
            HJScarletList.ScarletRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.width = Item.height = 16;
            Item.damage = 98;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shootSpeed = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.UseSound = HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -.36f, PitchVariance = .1f, Volume = .30f, Variants = [2] };
            Item.useTime = Item.useAnimation = 62;
            Item.shoot = ProjectileType<GaiaStrikerProj>();
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj<GaiaStrikerHeldProj>();
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Type, out Texture2D itemDrawFrame, out Rectangle frame);
            Vector2 ori = frame.Size() / 2f;
            Vector2 pos = Item.Bottom - Main.screenPosition - new Vector2(0, ori.Y);
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(itemDrawFrame, pos + (TwoPi / 16f * i).ToRotationVector2() * 2, null, Color.Crimson.ToAddColor(), rotation, ori, scale, 0, 0);
            spriteBatch.Draw(itemDrawFrame, pos, null, Color.White, rotation, ori, scale, 0, 0);
            return false;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == Mod.Name && (line.Name == "FlavorTooltipsName"))
            {
                DisasterRarity.DrawFlavorRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void AddRecipes()
        {

            CreateRecipe().
                AddIngredient<DreamingLight>().
                AddIngredient<TheJudgement>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                AddCondition(HJScarletCraftingConditions.AnyAfterCrafting).
                Register();
        }
    }
}
