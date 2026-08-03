using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        /// <summary>
        /// <para>是否允许手动处决模式</para>
        /// <para>用于标识玩家全局的处决能力类型</para>
        /// </summary>
        public bool tacticalExecution = false;
        /// <summary>
        /// <para>临时手动处决形态切换标志（与 <see cref="tacticalExecution"/> 无关）。</para>
        /// <para>该字段专门用于处决攻击时动态切换玩家的手持形态（包括手持射弹）。 </para>
        /// <para>当玩家按下处决键且满足条件时，此字段会被设为 <c>true</c>，并在处决动画结束后需由调用者手动重置为 <c>false</c></para>
        /// <para>此开关不依赖任何装备，适用于需要临时改变攻击形态的场景（例如使用特殊射弹替换普通投掷物）</para>
        /// </summary>
        public bool tacticalExecutionManual = false;
        /// <summary>
        /// 手动处决模式的预输入帧
        /// </summary>
        public int tacticalExecutionInputCache = 0;
        /// <summary>
        /// 触发处决时这里的值会设置为true，以在某些地方提供加成效果
        /// </summary>
        public bool isExecutionStrikeTriggered = false;
        /// <summary>
        /// 处决字段存储的核心字典。
        /// <br><see langword="Key"/>键为武器的ID，而<see langword="Value"/>值为当前键值下的处决进程</br>
        /// </summary>
        public Dictionary<int, int> ExecutionListStored = new Dictionary<int, int>();
        public bool hasSendExecutionTint = false;
        public int hasSendExecutionTintTimer = 0;
        public bool CanExecutionStrike = false;
        public void OnEnterWorldReset()
        {
            //每次进入世界的时候初始化这个列表
            ExecutionListStored = new Dictionary<int, int>(HJScarletList.ExecuteRequests);
            for (int i = 0; i < ExecutionListStored.Count; i++)
                ExecutionListStored[i] = 0;
        }
        public void ResetExecutorCheck()
        {
            if (Player.HeldItem.IsWeapon())
            {
                CanExecutionStrike = Player.CheckExecution(Player.HeldItem.type);
                if (CanExecutionStrike)
                    hasSendExecutionTint = false;
            }
        }
    }
}
