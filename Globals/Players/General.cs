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

        public bool CreationHatSet = false;

        #region Accessories
        public bool Player_RewardofWarrior = false;
        public bool Player_RewardofKingdom = false;
        public int RewardofWarriorHitCD = 0;
        public int RewardofWarriorCounter  = 0;
        public int KingdomDefenseTime = 0;
        public int RewardLevel = 0;
        #endregion
        #region Pets
        public bool WhalePet = false;
        public bool NonePet = false;
        public bool ShadowPet = false;
        public bool SquidPet = false;
        public bool WatcherPet = false;
        #endregion
        public override void ResetEffects()
        {
            CreationHatSet = false;
            UnloadPets();
        }

        private void UnloadPets()
        {
            WhalePet = false;
            NonePet = false;
            ShadowPet = false;
            SquidPet = false;
            WatcherPet = false;
        }

        public override void UpdateDead()
        {
            FocusStrikeTime = 0;
            FlybackBuffTime = 0;
            CurrentFullFlyBackTime = 0;
            UnloadPets();
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
