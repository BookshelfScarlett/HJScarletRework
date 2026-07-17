using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
        public override void ExSSD()
        {
            HJScarletList.ScarletRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.width = Item.height = 126;
            Item.damage = 98;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shootSpeed = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.UseSound = HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -.36f, PitchVariance = .1f, Volume = .45f, Variants = [2] };
            Item.useTime = Item.useAnimation = 52;
            Item.shoot = ProjectileType<GaiaStrikerProj>();
        }
        public override bool CanShoot(Player player)
        {
            return !player.HJScarlet().holdingGaiaStaff;
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
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == Mod.Name && (line.Name == "FlavorTooltipsName"))
            {
                DisasterRarity.DrawMisc(line);
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
                Register();
        }
    }
}
