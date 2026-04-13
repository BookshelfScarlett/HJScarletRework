using HJScarletRework.Core.Keybinds;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Players;
using System.Configuration;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static bool AddFocusHitNoFocusProj<T>(this Projectile proj) where T : ModProjectile
        {
            Player owner = Main.player[proj.owner];
            return proj.HJScarlet().AddFocusHit && !owner.HasProj<T>();
        }
        public static void AddExecutionTime(this Projectile proj,  int itemID, int times = 1)
        {
            if (!proj.HJScarlet().AddFocusHit)
                return;
            Player owner = Main.player[proj.owner];
            if (owner.HJScarlet().ExecutionListStored.ContainsKey(itemID))
                owner.HJScarlet().ExecutionListStored[itemID]+=times;
        }
        public static void AddExecutionTimePass(this Projectile proj,  int itemID, int times = 1)
        {
            if (proj.HJScarlet().AddFocusHit)
                return;
            Player owner = Main.player[proj.owner];
            if (owner.HJScarlet().ExecutionListStored.ContainsKey(itemID))
                owner.HJScarlet().ExecutionListStored[itemID]+=times;
        }

        public static bool CheckExecution(this Player owner, int itemID, int executionTime)
        {
            HJScarletPlayer usPlayer = owner.HJScarlet();
            //检查玩家手持的武器
            Item item = owner.HeldItem;
            if (item.DamageType != ExecutorDamageClass.Instance)
                return false;
            if (usPlayer.tacticalExecution)
            {
                if (!usPlayer.CanExecution)
                    return false;
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                    return false;
                //无论啥情况，这里都要直接设置为否
                usPlayer.CanExecution = false;
                if (usPlayer.ExecutionListStored.TryGetValue(itemID, out int value))
                {
                    bool isTrue = value >= executionTime;
                    if(isTrue)
                    {
                        if (usPlayer.tacticalTime == 0 && usPlayer.tacticalPunishTime == 0)
                        {
                            usPlayer.tacticalTime = GetSeconds(10);
                            usPlayer.tacticalPunishTime = GetSeconds(1);
                        }
                    }
                    return isTrue;
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
