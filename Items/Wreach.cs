using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.Magic;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items
{
    public class Wreach : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
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
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Stopwatch.StartNew();
            Stopwatch sw = Stopwatch.StartNew();
            //foreach(var proj2 in Main.ActiveProjectiles)
            //{
            //    if (Main.myPlayer != player.whoAmI)
            //        continue;
            //    if (!proj2.minion)
            //        continue;
            //    if (proj2.owner != player.whoAmI)
            //        continue;
            //    //proj2.Kill();
            //    proj2.active = false;
            //}
            //float curSlots = player.maxMinions - player.slotsMinions;
            //List<Item> hasList = [];
            //SoundEngine.PlaySound(HJScarletSounds.Misc_Spell);
            //while (curSlots >= 1)
            //{
            //    int itemID = Main.rand.NextFromCollection(HJScarletList.SummonWeaponList);
            //    Item item = ContentSamples.ItemsByType[itemID];

            //    Projectile proj = ContentSamples.ProjectilesByType[item.shoot];

            //    if (curSlots >= proj.minionSlots && !hasList.Contains(item))
            //    {
            //        ItemLoader.Shoot(item, player, source, position, velocity, proj.type, item.damage, knockback);
            //        curSlots -= proj.minionSlots;
            //        hasList.Add(item);
            //    }
            //}
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<StarofHopeStar>(), 123, knockback, player.whoAmI);
            //((TerraFlamethrowerTank)proj.ModProjectile).BeginTargetRotation = player.ToMouseVector2().ToRotation();
            //proj.rotation = RandRotTwoPi;
            //proj.HJScarlet().ExecutionStrike = true;
            // 在需要测量的代码之前创建并启动 Stopwatch
            // 这里放置你要测量延迟的代码
            //for (int i = 0; i < 3000; i++)
            //{
            //    ECSParticle.ShinyCrossStarECS(position.ToRandCirclePosEdge(300), RandVelTwoPi(1.3f, 2.2f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1f, 5f);
            //}

            // 停止计时

            sw.Stop();
            // 输出经过的时间（毫秒）
            Main.NewText($"执行耗时: {sw.ElapsedMilliseconds} ms");
            // 更高精度输出
            Main.NewText($"精确耗时: {sw.Elapsed.TotalMilliseconds:F4} ms");
            return false;
            //Vector2 ownerMW = player.LocalMouseWorld();
            //添加需要的攻击单位
        }
    }

}
