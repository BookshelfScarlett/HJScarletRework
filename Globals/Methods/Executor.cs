using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void AddExecutionTimePostHit(this Projectile proj, int itemID, int times = 1)
        {
            if (!proj.HJScarlet().AddExecutionHit)
                return;
            proj.AddExecutionTime(itemID, times);
        }
        public static void AddExecutionTimePreHit(this Projectile proj, int itemID, int times = 1)
        {
            if (!proj.HJScarlet().HasExecutionMechanic)
                return;
            if (proj.HJScarlet().AddExecutionHit)
                return;
            proj.AddExecutionTime(itemID, times);
        }
        public static void AddExecutionTime(this Projectile proj, int itemID, int times = 1)
        {
            Player owner = Main.player[proj.owner];
            if(owner.HJScarlet().ExecutionListStored.TryGetValue(itemID, out int curExeTime) && owner.HJScarlet().tacticalExecution)
            {
                if (curExeTime == HJScarletList.ExecutorWeaponDictionary[itemID] -1)
                SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 0}, owner.Center);
                if (curExeTime >= HJScarletList.ExecutorWeaponDictionary[itemID])
                    return;
            }
            if (owner.HJScarlet().ExecutionListStored.ContainsKey(itemID))
                owner.HJScarlet().ExecutionListStored[itemID] += times;
        }
        public static void InsertExecutorTooltips(this List<TooltipLine> tooltips)
        {

        }
        public static bool CheckExecution(this Player owner, int itemID)
        {
            owner.HJScarlet().ExecutionListStored.TryAdd(itemID, 0);
            HJScarletPlayer usPlayer = owner.HJScarlet();
            //检查玩家手持的武器
            Item item = owner.HeldItem;
            if (!item.DamageType.CountsAsClass<ExecutorDamageClass>())
                return false;
            int executionTime = HJScarletList.ExecutorWeaponDictionary[itemID];
            if (usPlayer.tacticalExecution)
            {
                if (!usPlayer.CanExecution)
                    return false;
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                    return false;
                //无论啥情况，这里都要直接设置为否
                if (usPlayer.ExecutionListStored.TryGetValue(itemID, out int value))
                {
                    bool canExe = value >= executionTime;
                    return canExe;
                }
                else
                    return false;
            }
            else
            {
                if (usPlayer.ExecutionListStored.TryGetValue(itemID, out int value))
                    return value >= executionTime;
                else
                    return false;
            }
        }
        public static void RemoveSlot(this Player player, int curItemType)
        {
            player.HJScarlet().ExecutionListStored.Remove(curItemType);
        }
    }
}
