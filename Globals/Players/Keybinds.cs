using HJScarletRework.Core.Keybinds;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public bool CanSwitchWeaponType = false;
        public bool CanRevisual = false;
        public bool CanExecution = false;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                bool tier1 = PiorityTier1();
                if (tier1)
                    return;
                bool tier2 = PiorityTier2();
                if (tier2) 
                    return;
            }
        }
        /// <summary>
        /// 第一批优先度梯队：物品切换，代行者手动触发的处刑攻击
        /// </summary>
        /// <returns></returns>
        private bool PiorityTier1()
        {
            bool anyPiorityTier1Active = false;
            //按情况提供硬编码
            //第一批顺序：查看武器转化
            //获取这个列表的第一个值，对比玩家手持的情况，符合则打一个标记出去
            int heldItem = Player.HeldItem.type;
            bool hasValue = WeaponSwapMaps.ContainsKey(heldItem) || ArmorMaps.Contains(heldItem) || WeaponSwapMaps.ContainsValue(heldItem);
            if (!CanSwitchWeaponType && hasValue)
            {
                //打一个标记出去
                CanSwitchWeaponType = true;
                anyPiorityTier1Active = true;
            }
            if (!CanExecution && tacticalExecution)
            {
                CanExecution = true;
                anyPiorityTier1Active = true;
            }
            return anyPiorityTier1Active;
        }
        /// <summary>
        /// 第二批优先梯队：盔甲技能，部分武器技能
        /// </summary>
        /// <returns></returns>
        private bool PiorityTier2()
        {
            return false;
        }
    }
}
