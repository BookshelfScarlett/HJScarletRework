using HJScarletRework.Buffs;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public bool NoGuideForBinaryStars = false;
        public bool CanDisableGuideForGrandHammer = false;
        public bool CanGiveFreeBinaryStars = false;
        public int flybackhandCloclCD = 0;
        public int flybackhandBuffTime = 0;
        public int flybackhandBuffTimeCurrent = 0;
        //用给归零针，查阅玩家当前损失的HP量
        public int flybackhandHealthRecord = 0;
        public int flybackHandManaRecord = 0;
        public int flybackInGameTimeBuff = 0;

        public bool CreationHatSet = false;
        //电表镀针的冲刺冷却
        public int galvanizedHandDashCD = 0;

        // 用于向上向下冲刺禁用羽落
        public int NoSlowFall = 0;
        public int ownerMinionHammerCount = 0;

        public int climaticHawstringLaserCounter = 0;
        #region 护甲
        public bool runeWizardExecutor = false;
        public bool cowboyExecutor = false;
        public int cowboyRevolverTimer = 0;

        public bool floretProtectorExecutor = false;
        public int floretProtectorTimer = 0;
        public bool raincoatExecutor = false;
        public bool redDragonKnight = false;

        #endregion
        #region Accessories
        public bool heartoftheCrystal = false;
        public bool loveRing = false;
        public bool isBeingLove = false;
        public int genderChangeTimer = 0;

        public bool ShadowCastAcc = false;
        public bool LifeBalloonAcc = false;
        public int LifeBalloonAccJumps;

        public bool stardustRune = false;
        public bool desterrennacht = false;
        public int stardustRuneHitHealTimer = 0;
        public int stardustRuneStaticHealTimer = 0;
        public int desterrannachtImmortalTime = 0;
        public int desterranRespawnChargeTimer = 0;

        public bool PreciousTargetAcc = false;
        public bool PreciousAimAcc = false;
        public int PreciousTargetCrtis = 10;
        public int PreciousCritsMin = 0;
        public int fakeManaContainer = 0;

        public bool defenderEmblem = false;
        public int defenderEmblemCD = 0;
        public int blackKeyHeal = 0;
        public float blackKeyDefenseBuff = 0;
        public int blackKeyTimer = 0;
        public bool blackKeyDoT = false;
        public int blackKeyReduceDefense = 0;
        #endregion

        #region Pets
        public bool WhalePet = false;
        public bool NonePet = false;
        public bool ShadowPet = false;
        public bool SquidPet = false;
        public bool WatcherPet = false;
        #endregion

        #region 处刑攻击
        public bool tacticalExecution = false;
        public int tacticalTime = 0;
        public bool executorAscension = false;
        public int tacticalPunishTime = 0;
        public int ExecutionTime = 0;
        public int bonusExecutionReduce = 0;
        public Dictionary<int, int> ExecutionListStored = new Dictionary<int, int>();
        public bool StopExecutionInit = false;

        //用于hud绘制的计时器
        public int Executor_AFKTimer = 0;
        public float Executor_BarOpacity = 0;
        public bool Executor_DrawFadeIn = false;
        public bool Executor_DrawFadeOut = false;

        public int exsanguinationBuffTime = 0;

        #endregion
        public bool terraRecipe = false;
        public int terraRecipe_EatenFoods = 0;
        public int terraRecipe_LifeMaxMultTime = 0;
        public int terraRecipe_LifeMaxIncre = 20;
        public List<int> terraRecipe_CurEat = new List<int>();
        public List<int> terraRecipe_haventEat = new List<int>();
        public override void ResetEffects()
        {
            climaticHawstringLaserCounter *= (Player.HeldItem.type == ItemType<ClimaticHawstring>()).ToInt();
            CreationHatSet = false;
            ShadowCastAcc = false;
            LifeBalloonAcc = false;
            critDamageAll = 0;
            critDamageExecutor = 0;
            bonusExecutionReduce = 0;
            ResetAcc();
            ResetPets();
            ResetArmor();
        }
        public override void UpdateDead()
        {
            ExecutionTime = 0;
            flybackhandBuffTime = 0;
            flybackhandCloclCD = 0;
            flybackhandBuffTimeCurrent = 0;
            PreciousTargetCrtis = 10;
            LifeBalloonAcc = false;
            galvanizedHandDashCD = 0;
            climaticHawstringLaserCounter = 0 ;

            desterrannachtImmortalTime = 0;
            desterranRespawnChargeTimer = 0;
            stardustRuneHitHealTimer = 0;
            defenderEmblemCD = 0;
            exsanguinationBuffTime = 0;
            tacticalTime = 0;
            tacticalPunishTime = 0;
            blackKeyTimer = 0;
            ResetAcc();
            ResetPets();
            ResetArmor();

            cowboyRevolverTimer = 0;
            floretProtectorTimer = 0;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff<HoneyRegenAlt>())
            {
                Player owner = drawInfo.drawPlayer;
                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(drawInfo.Position, owner.width + 4, owner.height + 4, Main.rand.NextBool() ? DustID.Honey : DustID.Honey2);
                    Main.dust[d].velocity = new Vector2(Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].alpha = 100;
                    Main.dust[d].scale *= 1f;
                    drawInfo.DustCache.Add(d);
                }
            }
            if (isBeingLove)
            {
                DrawLoveRingParticle(drawInfo.Position, drawInfo.drawPlayer);
            }
        }

        public void DrawLoveRingParticle(Vector2 position, Player drawPlayer)
        {
            if (Main.rand.NextBool(12))
            {
                Rectangle rec = Utils.CenteredRectangle(drawPlayer.Center, new Vector2(drawPlayer.width, drawPlayer.height));
                Vector2 pos = Main.rand.NextVector2FromRectangle(rec) + Vector2.UnitY * 20f + Vector2.UnitX * Main.rand.NextFloat(10f, 20f) * Main.rand.NextBool().ToDirectionInt();
                new HeartParticle(pos, Vector2.UnitY * -Main.rand.NextFloat(0.51f, 2.3f), RandLerpColor(Color.Crimson, Color.HotPink), 40, 0.08f, 0.8f, fadeIn: true).Spawn();
            }
        }
    }
}
