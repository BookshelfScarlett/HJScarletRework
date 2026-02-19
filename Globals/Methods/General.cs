using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static HJScarletPlayer HJScarlet(this Player player) => player.GetModPlayer<HJScarletPlayer>();
        public static HJScarletGlobalProjs HJScarlet(this Projectile proj) => proj.GetGlobalProjectile<HJScarletGlobalProjs>();
        public static HJScarletGlobalNPCs HJScarlet(this NPC npc) => npc.GetGlobalNPC<HJScarletGlobalNPCs>();
        public static HJScarletGlobalItem HJScarlet(this Item item) => item.GetGlobalItem<HJScarletGlobalItem>();
        public static bool HasProj<T>(this Player player) where T : ModProjectile => HasProj(player, ProjectileType<T>());
        public static bool HasProj(this Player player, int projID) => player.ownedProjectileCounts[projID] > 0;

        /// <summary>
        /// 重载一个out传参，输出你判定的拥有的proj的ID以方便后续可能需要的计算，或者别的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="player"></param>
        /// <param name="ProjID"></param>
        /// <returns></returns>
        public static bool HasProj<T>(this Player player, out int ProjID) where T : ModProjectile
        {
            ProjID = ProjectileType<T>();
            return HasProj<T>(player);
        }
        public static bool OutOffScreen(Vector2 pos)
        {
            if (pos.X < Main.screenPosition.X - Main.screenWidth / 2)
                return true;

            if (pos.Y < Main.screenPosition.Y - Main.screenHeight / 2)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth * 1.5f)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight * 1.5f)
                return true;

            return false;
        }
        public static bool OutOffScreen(Vector2 pos, float areamult = 1f)
        {
            float halfwidth = Main.screenWidth / 2;
            float halfheight = Main.screenHeight / 2;
            if (pos.X < Main.screenPosition.X - halfwidth * areamult)
                return true;

            if (pos.Y < Main.screenPosition.Y - halfheight * areamult)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth + halfwidth * areamult)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight + halfheight * areamult)
                return true;

            return false;
        }
        public static Vector2 LocalMouseWorld(this Player player) => player.HJScarlet().SyncedMouseWorld;
        public static bool PressLeftAndRightClick(this Player player)
        {
            return player.HJScarlet().MouseLeft && player.HJScarlet().MouseRight;
        }
        public static bool JustPressLeftClick(this Player player)
        {
            return player.HJScarlet().MouseLeft && !player.HJScarlet().MouseRight;
        }

        public static bool JustPressRightClick(this Player player)
        {
            return !player.HJScarlet().MouseLeft && player.HJScarlet().MouseRight;
        }
        public static bool GetImmnue(this Player player, int cooldownSlot, int frames, bool blink = false)
        {
            if (!((cooldownSlot < 0) ? (player.immuneTime < frames) : (player.hurtCooldowns[cooldownSlot] < frames)))
            {
                return false;
            }

            player.immune = true;
            player.immuneNoBlink = !blink;
            if (cooldownSlot < 0)
            {
                player.immuneTime = frames;
            }
            else
            {
                player.hurtCooldowns[cooldownSlot] = frames;
            }

            return true;
        }
        /// <summary>
        /// 快速生成一个简单明了的圆形粒子组
        /// </summary>
        /// <param name="dPos"></param>
        /// <param name="dCounts"></param>
        /// <param name="dScale"></param>
        /// <param name="dType"></param>
        /// <param name="dSpeed"></param>
        /// <param name="dPosOffset"></param>
        /// <param name="dGrav"></param>
        /// <param name="dAlpha"></param>
        public static void CirclrDust(this Vector2 dPos, int dCounts, float dScale, int dType, int dSpeed, float dPosOffset = 0f, bool dGrav = true, int dAlpha = 255)
        {
            float rotArg = 360f / dCounts;
            for (int i = 0; i < dCounts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotArg);
                Vector2 offsetPos = new Vector2(dPosOffset, 0f).RotatedBy(rot);
                Vector2 dVel = new Vector2(dSpeed, 0f).RotatedBy(rot);
                Dust d = Dust.NewDustPerfect(dPos + offsetPos, dType, dVel);
                d.noGravity = dGrav;
                d.velocity = dVel;
                d.scale = dScale;
                d.alpha = dAlpha;
            }
        }
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
        public static void ShimmerEach(this int type, int shimmerType)
        {
            ItemID.Sets.ShimmerTransformToItem[type] = shimmerType;
            ItemID.Sets.ShimmerTransformToItem[shimmerType] = type;
        }
        public static void ShimmerEach<T>(this int type) where T : ModItem => ShimmerEach(type, ItemType<T>());
        public static Vector2 ToMouseVector2(this Player player, Vector2? safeValue = null)
        {
            Vector2 safe = safeValue ?? Vector2.UnitX;
            return (player.LocalMouseWorld() - player.MountedCenter).SafeNormalize(safe);
        }
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float speed = 1f) => srcVel.ToSafeNormalize().RotatedBy(Main.rand.NextFloat(randRads) * Main.rand.NextBool().ToDirectionInt()) * speed;
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float minSpeed, float maxSpeed)
        {
            return srcVel.ToRandVelocity(randRads, Main.rand.NextFloat(minSpeed, maxSpeed));
        }
        public static Vector2 ToSafeNormalize(this Vector2 srcVel, Vector2? what = null) => srcVel.SafeNormalize(what ?? Vector2.UnitX);
        
        public static string ToPercent(this float value)
        {
            float value2 = value * 100f;
            return $"{(int)value2}%";
        }
        public static int SetAxePower(this int percent) => percent / 5;
        /// <summary>
        /// 将玩家的鼠标向量获取的值限制在1080p屏幕内，类似于天顶剑
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector2 ToClampMouseVector2(this Player player)
        {
            Vector2 mouseWorld = player.LocalMouseWorld();
            mouseWorld.X = mouseWorld.X >= player.MountedCenter.X ? MathF.Min(mouseWorld.X, player.MountedCenter.X + 960f) : MathF.Max(mouseWorld.X, player.MountedCenter.X - 960f);
            mouseWorld.Y = mouseWorld.Y >= player.MountedCenter.Y ? MathF.Min(mouseWorld.Y, player.MountedCenter.Y + 540f) : MathF.Max(mouseWorld.Y, player.MountedCenter.Y - 540f);
            return mouseWorld;
        }
        public static int TerrariaCurrentHour
        {
            get
            {
                //获取当前泰拉时间，即Main.Time
                double time = Main.time;

                //假定当前是晚上，则将时间进行校准，出于某些原因，夜晚的时间必须得直接增加54000来表示
                if (!Main.dayTime)
                    time += 54000D;

                //内部时间单位以“刻”表示，而60刻=1秒，因此这里3600刻为1小时，我们将当前的量拆分
                time /= 3600D;

                //泰拉瑞亚的夜晚起始于晚上7点半，因此需要用这个值去指针
                time -= 19.5;

                //假如过度校准，直接自增24小时，让时钟转移一圈避免负值
                if (time < 0D)
                    time += 24D;

                //而后，我们开始常识性地获取分钟
                int intTime = (int)time;
                double deltaTime = time - intTime;

                //将当前的时间转化为一个实际的分钟值
                deltaTime = (int)(deltaTime * 60D);

                //然后，将24个整数小时转为12小时
                if (intTime > 12)
                    intTime = 12;
                return intTime;
            }
        }
        public static bool IsItemName(this DrawableTooltipLine line) => line.Name == "ItemName" && line.Mod == "Terraria";
        /// <summary>
        /// 杀了玩家
        /// </summary>
        public static void KillPlayer(Player Player)
        {
            var source = Player.GetSource_Death();
            Player.lastDeathPostion = Player.Center;
            Player.lastDeathTime = DateTime.Now;
            Player.showLastDeath = true;
            int coinsOwned = (int)Utils.CoinsCount(out bool flag, Player.inventory, new int[0]);
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.lostCoins = coinsOwned;
                Player.lostCoinString = Main.ValueToCoins(Player.lostCoins);
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Main.mapFullscreen = false;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.trashItem.SetDefaults(0, false);
                if (Player.difficulty == PlayerDifficultyID.SoftCore || Player.difficulty == PlayerDifficultyID.Creative)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (Player.inventory[i].stack > 0 && ((Player.inventory[i].type >= ItemID.LargeAmethyst && Player.inventory[i].type <= ItemID.LargeDiamond) || Player.inventory[i].type == ItemID.LargeAmber))
                        {
                            int droppedLargeGem = Item.NewItem(source, (int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, Player.inventory[i].type, 1, false, 0, false, false);
                            Main.item[droppedLargeGem].netDefaults(Player.inventory[i].netID);
                            Main.item[droppedLargeGem].Prefix(Player.inventory[i].prefix);
                            Main.item[droppedLargeGem].stack = Player.inventory[i].stack;
                            Main.item[droppedLargeGem].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[droppedLargeGem].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[droppedLargeGem].noGrabDelay = 100;
                            Main.item[droppedLargeGem].favorited = false;
                            Main.item[droppedLargeGem].newAndShiny = false;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, droppedLargeGem, 0f, 0f, 0f, 0, 0, 0);
                            }
                            Player.inventory[i].SetDefaults(0, false);
                        }
                    }
                }
                else if (Player.difficulty == PlayerDifficultyID.MediumCore)
                {
                    Player.DropItems();
                }
                else if (Player.difficulty == PlayerDifficultyID.Hardcore)
                {
                    Player.DropItems();
                    Player.KillMeForGood();
                }
            }
                SoundEngine.PlaySound(SoundID.PlayerKilled, Player.Center);
            Player.headVelocity.Y = Main.rand.Next(-40, -10) * 0.1f;
            Player.bodyVelocity.Y = Main.rand.Next(-40, -10) * 0.1f;
            Player.legVelocity.Y = Main.rand.Next(-40, -10) * 0.1f;
            Player.headVelocity.X = Main.rand.Next(-20, 21) * 0.1f + 2 * 0;
            Player.bodyVelocity.X = Main.rand.Next(-20, 21) * 0.1f + 2 * 0;
            Player.legVelocity.X = Main.rand.Next(-20, 21) * 0.1f + 2 * 0;
            if (Player.stoned)
            {
                Player.headPosition = Vector2.Zero;
                Player.bodyPosition = Vector2.Zero;
                Player.legPosition = Vector2.Zero;
            }
            for (int j = 0; j < 100; j++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain, 2 * 0, -2f, 0, default, 1f);
            }
            Player.mount.Dismount(Player);
            Player.dead = true;
            Player.respawnTimer = 600;
            if (Main.expertMode)
            {
                Player.respawnTimer = (int)(Player.respawnTimer * 1.5);
            }
            Player.immuneAlpha = 0;
            Player.palladiumRegen = false;
            Player.iceBarrier = false;
            Player.crystalLeaf = false;

            if (Player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }
    }
}
