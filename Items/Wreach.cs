using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Requirement;
using HJScarletRework.Projs.Magic;
using HJScarletRework.Projs.ParryShield;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items
{
    public class Wreach : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override string Texture => HJScarletItemProj.Wreach.Path;
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 20;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<CoronaFireball>();
            Item.shootSpeed = 17f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        private IReadOnlyList<TooltipLine> AlterTooltip = null;
        private float LineY = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            AlterTooltip = tooltips;
            base.ModifyTooltips(tooltips);
        }
        public override void HoldItem(Player player)
        {
            if (Main.mouseLeft)
            {
                ScreenZoomSystem.UseTexture = true;
                LineY = Lerp(LineY, 1f, 0.1f);
                ScreenZoomSystem.ZoomIn(SmoothStep(0, 0.37f, LineY));
            }
            else
            {
                LineY = Lerp(LineY, 0, .1f);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Stopwatch.StartNew();
            Stopwatch sw = Stopwatch.StartNew();
            sw.Stop();
            // 输出经过的时间（毫秒）
            //Main.NewText($"执行耗时: {sw.ElapsedMilliseconds} ms");
            //// 更高精度输出
            //Main.NewText($"精确耗时: {sw.Elapsed.TotalMilliseconds:F4} ms");
            return false;
            //Vector2 ownerMW = player.LocalMouseWorld();
            //添加需要的攻击单位
        }
    }

}

namespace HJScarletRework.Items.Weapons.Requirement
{
    // 根据你实际需要调整类名、成员和访问级别
    public class WeaponRequirement
    {
        // 占位构造／方法
        public WeaponRequirement() { }
    }
}
