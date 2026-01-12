using ContinentOfJourney;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public int FocusStrikeTime = 0;
        public bool NoGuideForBinaryStars = false;
        public bool CanDisableGuideForGrandHammer = false;
        public bool CanGiveFreeBinaryStars = false;
        public int FlybackBuffTime = 0;
        public int CurrentFullFlyBackTime = 0;
        //用给归零针，查阅玩家当前损失的HP量
        public int CurrentLostHP = 0;
        public int CurrentLostMana = 0;
        public int FlybackHitBuffTimer = 0;
        #region Accessories
        public bool Player_RewardofWarrior = false;
        public bool Player_RewardofKingdom = false;
        public int RewardofWarriorHitCD = 0;
        public int RewardofWarriorCounter  = 0;
        public int KingdomDefenseTime = 0;
        public int RewardLevel = 0;
        #endregion
        public override void PostUpdateMiscEffects()
        {
            UpdateTimer();
            if(Player_RewardofKingdom && RewardofWarriorCounter > 0)
            {
                Player.statDefense += KingdomDefenseTime;
            }
            //归零针buff
            if (FlybackHitBuffTimer > 0 && (Player.HeldItem.type == ItemType<FlybackHandThrown>() || ModLoader.HasMod(HJScarletMethods.CalamityMod)))
            {
                //白天上午与夜间前半夜：给予15%近战伤害加成/15防御力加成
                if (HJScarletMethods.TerrariaCurrentHour <= 6)
                {
                    if (Main.dayTime)
                        Player.GetDamage<MeleeDamageClass>() += 0.15f;
                    else
                        Player.statDefense += 15;
                }
                //白天下午与夜间后半夜：给予15近战速度加成/10%伤害减免
                else
                {
                    if(Main.dayTime)
                        Player.GetAttackSpeed<MeleeDamageClass>() += 0.15f;
                    else
                        Player.endurance += 0.1f;
                }
            }
        }

        private void UpdateTimer()
        {
            if (FlybackBuffTime > 0)
                FlybackBuffTime--;
            
            if (FlybackBuffTime == 0)
                CurrentFullFlyBackTime = 0;

            if (RewardofWarriorCounter> 0)
                RewardofWarriorCounter-= 0;

            if (RewardofWarriorCounter == 0)
                KingdomDefenseTime--;

            if (KingdomDefenseTime < 0)
                KingdomDefenseTime = 0;

            if (FlybackHitBuffTimer > 0)
                FlybackHitBuffTimer--;
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
        }
        public void GlobalOnHitNPCWithSomething(NPC target,  NPC.HitInfo hit, int damageDone)
        {
            //帝国的荣耀相关buff都写在里面了
            if (!target.friendly && target.lifeMax >= 5 && target.CanBeChasedBy() && Player_RewardofWarrior)
            {
                RewardofWarriorCounter = 180;
                //给予5帧短暂的cd
                if (RewardofWarriorHitCD == 0)
                {
                    RewardofWarriorHitCD = 5;
                    RewardLevel += 1;
                }
                
                if (RewardLevel > 30)
                {
                    if (!Player.HasBuff<RewardsofWarriorBuff>())
                    {
                        Player.AddBuff(BuffType<RewardsofWarriorBuff>(), 300);
                        RewardLevel = 0;
                    }
                    else
                    {
                        RewardLevel = 30;
                    }
                }
                //如果佩戴了上位饰品，在下方进行防御力递增
                if (Player_RewardofKingdom)
                {
                    //这里的Counter必须得启用，每次过来都会刷新
                    //顶多就是续一个180秒，持续攻击这一块
                    if (KingdomDefenseTime < 30)
                        KingdomDefenseTime += 1;
                }
                
            }

        }
        public override void UpdateDead()
        {
            FocusStrikeTime = 0;
            FlybackBuffTime = 0;
            CurrentFullFlyBackTime = 0;
        }
        public override void PostUpdate()
        {
            UpdateNetPacket();
        }
        public override void SaveData(TagCompound tag)
        {
            HammerTagSave(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            HammerTagLoad(tag);
        }
        private void HammerTagSave(TagCompound tag)
        {
            tag.Add(nameof(NoGuideForBinaryStars), NoGuideForBinaryStars);
            tag.Add(nameof(CanDisableGuideForGrandHammer), CanDisableGuideForGrandHammer);
            tag.Add(nameof(CanGiveFreeBinaryStars), CanGiveFreeBinaryStars);
        }

        private void HammerTagLoad(TagCompound tag)
        {
            NoGuideForBinaryStars = tag.GetBool(nameof(NoGuideForBinaryStars));
            CanDisableGuideForGrandHammer = tag.GetBool(nameof(CanDisableGuideForGrandHammer));
            CanGiveFreeBinaryStars = tag.GetBool(nameof(CanGiveFreeBinaryStars));
        }
        public override bool OnPickup(Item item)
        {
            return base.OnPickup(item);
        }
        /// <summary>
        /// 梦魇锤投掷微光转为弑神锤的引导
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool StopGodHammerShimemrGuide(Item item)
        {
            bool downedAnyGods = DownedBossSystem.downedMatterGod || DownedBossSystem.downedLifeGod || DownedBossSystem.downedTimeGod;
            if (item.type == ItemType<BinaryStars>() && downedAnyGods)
            {
                NoGuideForBinaryStars = true;
                return true;
            }
            /*
            if (item.type == ItemType<ThunderHammer>() && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
            {
                CanDisableGuideForGrandHammer = true;
                return true;
            }
            */
            return false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff<HoneyRegenAlt>())
            {
                Player owner = drawInfo.drawPlayer;
                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustDirect(drawInfo.Position, owner.width + 4, owner.height + 4, Main.rand.NextBool() ? DustID.Honey : DustID.Honey2);
                    d.velocity = new Vector2(Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    d.alpha = 100;
                    d.scale *= 1f;
                }
            }
        }
    }
}
