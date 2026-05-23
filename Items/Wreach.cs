using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
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
            //foreach (var keys in HJScarletList.ExecutorWeaponDictionary.Keys)
            //{
            //    player.QuickSpawnItem(source, keys);
            //    Main.NewText(HJScarletList.ExecutorWeaponDictionary[keys]);
            //}
            //Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ProjectileType<MoltenDaggerMark>(), 0, knockback, player.whoAmI);
            //Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ProjectileType<GhostDaggerMark>(), 0, knockback, player.whoAmI);
            Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<FrostHammerIceSpike>(), 0, knockback, player.whoAmI);
        //    Stopwatch.StartNew();
        //    // 在需要测量的代码之前创建并启动 Stopwatch
        //    Stopwatch sw = Stopwatch.StartNew();
        //    // 这里放置你要测量延迟的代码
        //    for (int i = 0; i < 3000; i++)
        //    {
        //        //ECSMethod.NewParticle(GetInstance<HRShinyOrbECS>().Type, 40, position.ToRandCirclePosEdge(300), RandVelTwoPi(1, 3), Color.White,scale: 1f,blendstate:Microsoft.Xna.Framework.Graphics.BlendState.Additive);
        //        //new HRShinyOrb(position, RandVelTwoPi(1, 3), Color.White, 40, 1f).Spawn();
        //        {
        //            ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
        //            {
        //                p.Position = position.ToRandCirclePosEdge(300);
        //                p.Velocity = RandVelTwoPi(1f, 3f);
        //                p.DrawColor = Color.White;
        //                p.Scale = 1f;
        //                p.Opacity = 1;
        //                p.Lifetime = 40;
        //                p.GlowCenterMult = 0.5f;
        //            });
        //        }
        //    }

        //// 停止计时
        //sw.Stop();

        //    // 输出经过的时间（毫秒）
        //    Main.NewText($"执行耗时: {sw.ElapsedMilliseconds} ms");
        //    // 更高精度输出
        //    Main.NewText($"精确耗时: {sw.Elapsed.TotalMilliseconds:F4} ms");
        //    Main.NewText(ScarletParticleManager.ParticleAdditive.Count);
            return false;
            //Vector2 ownerMW = player.LocalMouseWorld();
            //添加需要的攻击单位
        }
    }

}
