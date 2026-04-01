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
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                //如果玩家开启背包的情况下，则统一优先执行开启背包的硬编码顺序 
                if (Main.playerInventory)
                {
                }
                else
                {
                    //按情况提供硬编码

                    //第一个顺序：查看武器转化
                    //获取这个列表的第一个值，对比玩家手持的情况，符合则打一个标记出去
                    int heldItem = Player.HeldItem.type;
                    bool hasValue = WeaponSwapMaps.ContainsKey(heldItem) || WeaponSwapMaps.ContainsValue(heldItem);
                    if (!CanSwitchWeaponType && hasValue)
                    {
                        //打一个标记出去
                        CanSwitchWeaponType = true;
                    }

                    //第二个顺序：reVisual的标记
                    //是否允许重置特效的武器已经被类名硬编码了，所以不需要做额外的遍历
                    if (!CanRevisual)
                    {

                    }
                }
            }
        }
    }
}
