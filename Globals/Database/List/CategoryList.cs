using ContinentOfJourney.Items;
using ContinentOfJourney.NPCs.Boss_TheLifebringer;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Database.List
{
    public partial class HJScarletList : ModSystem
    {
        public static List<int> LegalFoodList = [];
        public static List<string> LegalFoodListName = [];
        public static List<int> SummonWeaponList = [];
        public static List<string> SummonWeaponFullName = [];

        public static HashSet<int> OresHashSet = [];
        public static HashSet<int> BarsHashSet = [];
        public static HashSet<int> ExecutorWhip = [];
        /// <summary>
        /// 减益列表，这个列表自动添加<see langword="并且仅建议用于敌对单位"/>
        /// <br>因为会把如药水疾病与魔力病这一类效果全部加入到这个表内</br>
        /// </summary>
        public static HashSet<int> DebuffListTarget = [];
        public static List<int> PandorasBurgerWeaponList = [];
        public override void Load()
        {
            for (int i = 0; i < LegalFoodList.Count; i++)
            {
                string name = ItemID.Search.GetName(LegalFoodList[i]);
                LegalFoodListName.Add(name);
            }
            DownedBossConditionList = new Dictionary<int, string>()
            {
                { NPCID.EaterofWorldsHead, "DownedEvilBoss" },
                { NPCID.Golem, "DownedGolem" },
                { NPCID.Plantera,"DownedPlantera" },
                { NPCType<TheLifebringerHead>(),"DownedLifeGods"},
            };
            DebuffListTarget = new HashSet<int>()
            {
                BuffID.Bleeding,
                BuffID.Poisoned,
                BuffID.Venom,
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Frostburn2,
                397,
                BuffID.Ichor,
                BuffID.BrokenArmor,
                BuffID.Shimmer,
                BuffID.Stinky,
                BuffID.Wet,
                BuffID.Slimed,
                BuffID.Confused,
                BuffID.Midas,
                BuffID.BetsysCurse,
                BuffID.ShadowFlame,
                BuffID.Oiled,
                BuffID.BoneJavelin,
                BuffID.Daybreak,
                BuffID.StardustMinionBleed,
                BuffID.BloodButcherer,
                BuffID.DryadsWardDebuff
            };
        }
        public override void PostSetupContent()
        {
            string[] heldProjNameMaybe = ["Hold", "Held", "Spear", "Drill", "Shortsword"];
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                //食物处理
                bool isFood = item.buffType == BuffID.WellFed || item.buffType == BuffID.WellFed2 || item.buffType == BuffID.WellFed3 || ItemID.Sets.IsFood[item.type];
                if (isFood && !LegalFoodList.Contains(item.type))
                {
                    LegalFoodList.Add(item.type);
                    ModItem modItem = item.ModItem;
                    if (modItem is not null)
                    {
                        LegalFoodListName.Add(modItem.FullName);
                    }
                    else
                    {
                        string vanillaName = ItemID.Search.GetName(i);
                        LegalFoodListName.Add(vanillaName);
                    }
                }

                //oreType和barType的处理
                string name = item.GetType().Name.ToLower();
                bool isOre = name.Contains("ore") && item.createTile != -1;
                bool isBar = name.Contains("bar") && item.createTile != -1;
                if (isOre && !OresHashSet.Contains(item.type))
                    OresHashSet.Add(item.type);
                if (isBar && !BarsHashSet.Contains(item.type))
                    BarsHashSet.Add(item.type);

                //处理召唤武器。
                bool isSummonWeapon = item.damage > 0 && item.DamageType.CountsAsClass<SummonDamageClass>() && item.shoot != ProjectileID.None;
                Projectile proj = ContentSamples.ProjectilesByType[item.shoot];
                if (isSummonWeapon && proj.minion && !proj.sentry && proj.minionSlots > 0)
                {
                    SummonWeaponList.Add(item.type);
                    if (i > VanillaMaxItem)
                        SummonWeaponFullName.Add(item.ModItem.FullName);
                }
                //排除掉所有召唤武器
                bool isWeapon = item.damage > 0 && !item.DamageType.CountsAsClass<SummonDamageClass>() && item.shoot != ProjectileID.None;
                Projectile proj2 = ContentSamples.ProjectilesByType[item.shoot];
                if (isWeapon)
                {
                    string internalName = proj2.GetType().Name;
                    bool shouldBanned = heldProjNameMaybe.Any(i => internalName.Contains(i, System.StringComparison.OrdinalIgnoreCase));
                    if (!shouldBanned)
                    {
                        PandorasBurgerWeaponList.Add(item.type);
                    }
                }
            }
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                ModBuff buff = BuffLoader.GetBuff(i);
                if (buff is null)
                    continue;
                if (Main.debuff[buff.Type])
                    DebuffListTarget.Add(buff.Type);
            }
            LoadDictionary();
        }
        public override void Unload()
        {
            LegalFoodList = null;
            LegalFoodListName = null;
            DownedBossConditionList = null;
            SummonWeaponFullName = null;

            OresHashSet.Clear();
            BarsHashSet.Clear();
            OresHashSet = null;
            BarsHashSet = null;
        }
    }
}
