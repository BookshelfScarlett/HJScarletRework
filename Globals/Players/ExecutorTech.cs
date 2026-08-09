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
        /// <summary>
        /// 支援道具-投刀类的支援效果标记
        /// <br>这个标记主要直接存储的是Projectile.type，即射弹类型</br>
        /// <br>会在<see cref="ResetEffects"/>常态重置为-1,在<see cref="PostUpdate"/>的位置进行实际的效果添加（包括对应的射弹生成）</br>
        /// <br>一般对于投掷小刀而言，需要在每个标记射弹初始化的时候传入这个type，然后在AI里面去检查这个index是否与当前的射弹相同</br>
        /// </summary>
        public int KnifeMarkIndex = -1;
        /// <summary>
        /// 总的武器增强Timer
        /// <br>只有这个值为0，才会更新下方的index，下方的index则用于确认是否与玩家当前手持的index为同一个</br>
        /// <br>因此，你可以通过这个东西来多地操作一些需要特殊增强的武器。</br>
        /// </summary>
        public int GeneralWeaponBuffTimer = 0;
        public int GeneralWeaponIndex = 0;
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
