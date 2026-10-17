using HJScarletRework.Globals.Executor;
using HJScarletRework.Projs.ParryShield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Database.List
{
    public partial class HJScarletList : ModSystem
    {
        /// <summary>
        /// 主要用于Boss方面
        /// </summary>
        public static Dictionary<int, string> DownedBossConditionList = [];
        /// <summary>
        /// 代行者处决叠层的核心机制，该字典用于存储每个代行者武器对应的总处决要求次数<br></br>
        /// <see langword="Key"/>即键为代行者的武器ID，而对应的<see langword="Value"/>即值为该武器对应的处决要求
        /// </summary>
        public static Dictionary<int, int> ExecuteRequests = [];
        /// <summary>
        /// 代行者武器的类型<br></br>
        /// <see langword="Key"/>键位代行者的武器ID，对应的<see langword="Value"/>值为武器对应的类型
        /// </summary>
        public static Dictionary<int, ExecutorWeaponType> ExecutorTypes = [];
        public static Dictionary<int, string> VanityItemDictionary = [];
        /// <summary>
        /// 格挡盾的类型<br></br>
        /// <see langword="Key"/>键为格挡盾的物品DI，对应的<see langword="Value"/>值为格挡盾的对应射弹ID
        /// </summary>
        public static Dictionary<int, int> ParryShieldDictionary = [];
        public void LoadDictionary()
        {
            ParryShieldDictionary = new Dictionary<int, int>()
            {
                {ItemID.CobaltShield, ProjectileType<CobaltParryShield>()},
                {ItemID.ObsidianShield, ProjectileType<ObsidianParryShield>()},
                {ItemID.AnkhShield, ProjectileType<AnkhParryShield>()},
                {ItemID.SquireShield, ProjectileType<SquiresParryShield>()},
                {ItemID.HuntressBuckler, ProjectileType<HuntresssBucklerParryShield>()},
                {ItemID.BouncingShield, ProjectileType<SergeantUnitedParryShield>()},
                {ItemID.HeroShield, ProjectileType<HeroParryShield>()},
                {ItemID.FrozenShield, ProjectileType<FrozenParryShield>()},
                {ItemID.PaladinsShield, ProjectileType<PaladinsParryShield>()},
                {ItemID.EoCShield, ProjectileType<EoCParryShield>()}
            };
        }

    }
}
