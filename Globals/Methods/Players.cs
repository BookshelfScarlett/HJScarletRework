using HJScarletRework.Buffs;
using HJScarletRework.Globals.Database.IDSets;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        /// <summary>
        /// 计算防御力加成，返回增加的防御力数值
        /// <br><paramref name="multiplier"/>为比率，如果低于1则返回0</br>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="multiplier"></param>
        /// <param name="noClamp">是否不进行 clamp 操作</param>
        /// <returns></returns>
        public static int DefenseMultiplier(this Player owner, float multiplier, bool noClamp = false)
        {
            float ratios = multiplier - 1f;
            if (ratios <= 0f && !noClamp)
                ratios = 0f;
            return (int)(owner.statDefense * ratios);
        }
        public static bool IsHolding<T>(this Player player) where T : ModItem => IsHolding(player, ItemType<T>());
        public static bool IsHolding(this Player player, int itemID) => player.HeldItem.type == itemID;
        public static bool CanUseHoldout(this Player player, int itemID) => !player.dead && !player.CCed && player.IsHolding(itemID);
        public static bool CanUseHoldout<T>(this Player player) where T : ModItem => !player.dead && !player.CCed && player.IsHolding(ItemType<T>());
        public static bool IsInInventory(this Player player) => Main.hoverItemName != "";
        public static bool IsInwater(this Player player) => Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
        /// <summary>
        /// 计算玩家对指定伤害类型的基础伤害加成比例（例如返回 0.25 表示伤害提升了 25%）。
        /// </summary>
        /// <param name="player">要计算的玩家实例</param>
        /// <param name="originalDamage">基础伤害值（通常为物品或弹幕的原始伤害）</param>
        /// <param name="damageClass">伤害类型（如近战、远程、魔法等）</param>
        /// <returns>额外伤害比例，即实际伤害相对于原始伤害的增幅（0 表示无加成，0.5 表示加成 50%）。</returns>
        public static float GetDamageBonusRatio(this Player player, int originalDamage, DamageClass damageClass) => (player.GetTotalDamage(damageClass).ApplyTo(originalDamage) - originalDamage) / (float)originalDamage;
        /// <summary>
        /// 计算玩家对指定伤害类型的基础伤害加成比例（例如返回 0.25 表示伤害提升了 25%）。
        /// 这是一个重载方案，可以让你输入指定的目标伤害，无视damageclass的加成规则
        /// </summary>
        /// <param name="player">要计算的玩家实例</param>
        /// <param name="originalDamage">基础伤害值（通常为物品或弹幕的原始伤害）</param>
        /// <param name="damageClass">伤害类型（如近战、远程、魔法等）</param>
        /// <returns>额外伤害比例，即实际伤害相对于原始伤害的增幅（0 表示无加成，0.5 表示加成 50%）。</returns>

        public static float GetDamageBonusRatio(int targetDamage, int originalDamage) => ((float)targetDamage - originalDamage) / (float)originalDamage;
        public static void ApplyNoKnockbackBuff(this Player player, int frame) => player.AddBuff(BuffType<AntiKnockbackBuff>(), frame);
        public static bool CountAsDebuff(this Player player, int buffType)
        {
            if (Main.debuff[buffType] && !ScarletBuffIDSets.IsStatueBuff[buffType])
                return true;
            return false;
        }
        public static void Suicide(this Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp = false, bool skipPrekill = false)
        {
            if (player.creativeGodMode || player.dead)
                return;

            player.StopVanityActions();

            bool playSound = true;
            bool genGore = true;
            if (!PlayerLoader.PreKill(player, dmg, hitDirection, pvp, ref playSound, ref genGore, ref damageSource) && !skipPrekill)
                return;

            if (pvp)
                player.pvpDeath = true;

            if (player.whoAmI == Main.myPlayer)
                Main.NotifyOfEvent(GameNotificationType.SpawnOrDeath);

            if (player.pvpDeath)
                player.numberOfDeathsPVP++;
            else
                player.numberOfDeathsPVE++;

            player.lastDeathPostion = player.Center;
            player.lastDeathTime = DateTime.Now;
            player.showLastDeath = true;
            bool overFlowing;
            long coinsOwned = Utils.CoinsCount(out overFlowing, player.inventory);
            if (Main.myPlayer == player.whoAmI)
            {
                player.lostCoins = coinsOwned;
                player.lostCoinString = Main.ValueToCoins(player.lostCoins);
            }


            if (Main.myPlayer == player.whoAmI)
                Main.mapFullscreen = false;

            if (Main.myPlayer == player.whoAmI)
            {
                player.trashItem.SetDefaults();
                if (player.difficulty == 0 || player.difficulty == 3)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= ItemID.LargeAmethyst && player.inventory[i].type <= ItemID.LargeDiamond) || player.inventory[i].type == ItemID.LargeAmber))
                        {
                            int num = Item.NewItem(player.GetSource_Death(), (int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type);
                            Main.item[num].netDefaults(player.inventory[i].netID);
                            Main.item[num].Prefix(player.inventory[i].prefix);
                            Main.item[num].stack = player.inventory[i].stack;
                            Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[num].noGrabDelay = 100;
                            Main.item[num].favorited = false;
                            Main.item[num].newAndShiny = false;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num);

                            player.inventory[i].SetDefaults();
                        }
                    }
                }
                else if (player.difficulty == 1)
                {
                    player.DropItems();
                }
                else if (player.difficulty == 2)
                {
                    player.DropItems();
                    player.KillMeForGood();
                }
            }

            if (!playSound)
                goto postSound;

            if (Main.dontStarveWorld || Main.tenthAnniversaryWorld)
                SoundEngine.PlaySound(player.Male ? SoundID.DSTMaleHurt : SoundID.DSTFemaleHurt, player.position);
            else
                SoundEngine.PlaySound(SoundID.PlayerKilled, player.position);
        postSound:

            if (Main.tenthAnniversaryWorld)
            {
                for (int j = 0; j < 85; j++)
                {
                    int type = Main.rand.Next(139, 143);
                    int num2 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, type, 0f, -10f, 0, default(Color), 1.2f);
                    Main.dust[num2].velocity.X += (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.dust[num2].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.dust[num2].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.dust[num2].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.dust[num2].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
                    Main.dust[num2].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
                    Main.dust[num2].scale *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
                }

                for (int k = 0; k < 40; k++)
                {
                    int type2 = Main.rand.Next(276, 283);
                    int num3 = Gore.NewGore(player.GetSource_Death(), player.position, new Vector2(0f, -10f), type2);
                    Main.gore[num3].velocity.X += (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[num3].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[num3].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[num3].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[num3].scale *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
                    Main.gore[num3].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
                    Main.gore[num3].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
                }
            }

            player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            if (player.stoned || !genGore)
            {
                player.headPosition = Vector2.Zero;
                player.bodyPosition = Vector2.Zero;
                player.legPosition = Vector2.Zero;
            }

            if (!genGore)
                goto postGore;

            for (int l = 0; l < 100; l++)
            {
                if (player.stoned)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.Stone, 2 * hitDirection, -2f);
                }
                else if (player.frostArmor)
                {
                    int num4 = Dust.NewDust(player.position, player.width, player.height, DustID.IceTorch, 2 * hitDirection, -2f);
                    Main.dust[num4].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                }
                else if (player.boneArmor)
                {
                    int num5 = Dust.NewDust(player.position, player.width, player.height, DustID.Bone, 2 * hitDirection, -2f);
                    Main.dust[num5].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                }
                else
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 2 * hitDirection, -2f);
                }
            }
        postGore:

            player.mount.Dismount(player);
            player.dead = true;
            player.respawnTimer = GetRespawnTime(player, pvp);

            PlayerLoader.Kill(player, dmg, hitDirection, pvp, damageSource);

            player.immuneAlpha = 0;
            if (!ChildSafety.Disabled)
                player.immuneAlpha = 255;

            player.palladiumRegen = false;
            player.iceBarrier = false;
            player.crystalLeaf = false;
            NetworkText deathText = damageSource.GetDeathText(player.name);
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(deathText, new Color(225, 25, 25));
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(deathText.ToString(), 225, 25, 25);

            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)dmg, hitDirection, pvp);

            if (player.whoAmI == Main.myPlayer && (player.difficulty == 0 || player.difficulty == 3))
            {
                if (!pvp)
                {
                    player.DropCoins();
                }
                else
                {
                    player.lostCoins = 0L;
                    player.lostCoinString = Main.ValueToCoins(player.lostCoins);
                }
            }

            player.DropTombstone(coinsOwned, deathText, hitDirection);
            if (player.whoAmI != Main.myPlayer)
                return;

            try
            {
                WorldGen.saveToonWhilePlaying();
            }
            catch
            {
            }
        }
        private static int GetRespawnTime(Player player, bool pvp)
        {
            int num = 300;
            bool flag = false;
            if (Main.netMode != NetmodeID.SinglePlayer && !pvp)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && (Main.npc[i].boss || Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail) && Math.Abs(player.Center.X - Main.npc[i].Center.X) + Math.Abs(player.Center.Y - Main.npc[i].Center.Y) < 4000f)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (flag)
                num += 300;

            if (Main.expertMode)
                num = (int)((double)num * 1.5);

            if (flag && Main.getGoodWorld && Main.netMode != NetmodeID.SinglePlayer)
            {
                bool flag2 = false;
                for (int j = 0; j < 255; j++)
                {
                    if (j != player.whoAmI && Main.player[j].active)
                    {
                        flag2 = true;
                        break;
                    }
                }

                if (flag2)
                    num *= 2;
            }

            return num;
        }
    }
}
