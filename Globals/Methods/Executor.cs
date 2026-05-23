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
        /// <summary>
        /// 增加处决计数。
        /// 仅在需要特殊处理时才使用此方法，例如要求射弹“回收之后”才累计处决进度的场景。
        /// </summary>
        /// <param name="proj">当前射弹实例。</param>
        /// <param name="itemID">武器（或能力）对应的物品 ID。</param>
        /// <param name="times">增加次数，默认为 1。</param>
        /// <remarks>
        /// 执行条件：<br/>
        /// 必须满足 <see cref="HJScarletPlayer.AddExecutionHit"/> 为 <c>true</c>，否则不执行任何操作。<br/>
        /// 实际累加逻辑委托给 <see cref="AddExecutionTimeDirectly"/>。
        /// </remarks>
        public static void AddExecutionTimeDelayed(this Projectile proj, int itemID, int times = 1)
        {
            if (!proj.HJScarlet().AddExecutionHit)
                return;
            proj.AddExecutionTimeDirectly(itemID, times);
        }
        /// <summary>
        /// 立刻增加处决计数。
        /// 主要用于普通命中时增加处决次数的场景
        /// </summary>
        /// <param name="proj">当前射弹实例。</param>
        /// <param name="itemID">武器（或能力）对应的物品 ID。</param>
        /// <param name="times">增加次数，默认为 1。</param>
        /// <remarks>
        /// 执行条件：<br/>
        /// <list type="bullet">
        /// <item><description><see cref="HJScarletPlayer.HasExecutionMechanic"/> 必须为 <c>true</c>；</description></item>
        /// <item><description><see cref="HJScarletPlayer.AddExecutionHit"/> 必须为 <c>false</c>（即未开启“回收后累加”模式）；</description></item>
        /// </list>
        /// 条件满足后，调用 <see cref="AddExecutionTimeDirectly"/> 执行实际累加。
        /// </remarks>
        public static void AddExecutionTimeImmediate(this Projectile proj, int itemID, int times = 1)
        {
            if (!proj.HJScarlet().HasExecutionMechanic)
                return;
            if (proj.HJScarlet().AddExecutionHit)
                return;
            proj.AddExecutionTimeDirectly(itemID, times);
        }
        /// <summary>
        /// 直接增加处决计数的核心方法。
        /// 内部包含手动处决的特殊处理：当即将达到所需次数时播放音效。
        /// <para>通常不推荐直接使用，请优先使用 <see cref="AddExecutionTimeDirectly"/> 或 <see cref="AddExecutionTimeDelayed"/></para>
        /// </summary>
        /// <param name="proj">当前射弹实例。</param>
        /// <param name="itemID">武器（或能力）对应的物品 ID。</param>
        /// <param name="times">增加次数，默认为 1。</param>
        /// <remarks>
        /// <para>逻辑细节：</para>
        /// <list type="number">
        /// <item><description>从 <see cref="HJScarletList.ExecutorWeaponDictionary"/> 获取该武器完成处决所需的总次数 <c>required</c>；</description></item>
        /// <item><description>若玩家处于手动处决模式（<see cref="HJScarletPlayer.tacticalExecution"/> 为 <c>true</c>）：<br/>
        ///     当当前累计次数恰好为 <c>required - 1</c> 时，播放 <see cref="SoundID.Item35"/> 音效（提示即将可处决）；<br/>
        ///     如果当前累计次数已达到或超过 <c>required</c>，则直接返回，不再增加。</description></item>
        /// <item><description>最后，将 <c>times</c> 累加到 <see cref="HJScarletPlayer.ExecutionListStored"/> 字典中对应的键上。</description></item>
        /// </list>
        /// <para>注意：此方法会直接修改玩家存储的处决进度，调用前请确保 <paramref name="itemID"/> 有效。</para>
        /// </remarks>
        public static void AddExecutionTimeDirectly(this Projectile proj, int itemID, int times = 1)
        {
            Player owner = Main.player[proj.owner];
            if (owner.HJScarlet().ExecutionListStored.TryGetValue(itemID, out int curExeTime) && owner.HJScarlet().tacticalExecution)
            {
                if (curExeTime == HJScarletList.ExecutorWeaponDictionary[itemID] - 1)
                    SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 0 }, owner.Center);
                if (curExeTime >= HJScarletList.ExecutorWeaponDictionary[itemID])
                    return;
            }
            if (owner.HJScarlet().ExecutionListStored.ContainsKey(itemID))
                owner.HJScarlet().ExecutionListStored[itemID] += times;
        }
        public static void InsertExecutorTooltips(this List<TooltipLine> tooltips)
        {

        }
        /// <summary>
        /// 判断玩家当前是否可以对指定武器发起处决。
        /// 此方法同时处理了手动触发处决与自动处决两种模式。
        /// </summary>
        /// <param name="owner">目标玩家。</param>
        /// <param name="itemID">武器（或处决能力对应物品）的 <see cref="Item.type"/>。</param>
        /// <returns>
        /// <c>true</c> 满足处决条件；<c>false</c> 不满足或处于禁止处决的状态。
        /// </returns>
        /// <remarks>
        /// <para>处决条件包括：</para>
        /// <list type="bullet">
        /// <item><description>玩家手持的武器必须属于 <see cref="ExecutorDamageClass"/> 伤害类型。</description></item>
        /// <item><description>根据武器 ID 从 <see cref="HJScarletList.ExecutorWeaponDictionary"/> 获取所需累计命中次数（executionTime）。</description></item>
        /// <item><description>累计命中次数记录在 <see cref="HJScarletPlayer.ExecutionListStored"/> 字典中，值达到或超过 executionTime 时满足条件。</description></item>
        /// </list>
        /// <para>处决模式分为两种：</para>
        /// <list type="number">
        /// <item><description><b>手动处决（<see cref="HJScarletPlayer.tacticalExecution"/> = true）</b>：需要额外检查 <see cref="HJScarletPlayer.tacticalExecutionInputCache"/> > 0，且 <see cref="HJScarletKeybinds.GeneralActionKeybind"/> 未按下。该模式下手动触发。</description></item>
        /// <item><description><b>普通处决</b>：只要累计命中足够即可自动触发。</description></item>
        /// </list>
        /// <para>注意：<see cref="HJScarletPlayer.ExecutionListStored"/> 中若不存在指定 itemID，会通过 <see cref="Dictionary.TryAdd"/> 添加键并初始化为 0，确保后续查询不报错。</para>
        /// <para><see cref="HJScarletPlayer.tacticalExecutionInputCache"/> 在经过这个方法后不会立刻重置为0，需要在调用这个方法之后手动重置为0 </para>
        /// </remarks>
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
                if (usPlayer.tacticalExecutionInputCache == 0)
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
        /// <summary>
        /// <para>移除指定武器的处决进度记录。</para>
        /// <para>处决进度存储在 <see cref="HJScarletPlayer.ExecutionListStored"/> 字典中，当某武器的累计次数归零时调用此方法可将该项从字典中删除，避免冗余数据。</para>
        /// <para>由于字典操作复杂度为 O(1)，无需担心频繁增删的性能问题。</para>
        /// <para>注意：重新添加处决进度会在 <see cref="AddExecutionTimeDirect"/> 等方法中自动处理，无需手动干预。</para>
        /// </summary>
        /// <param name="player">目标玩家。</param>
        /// <param name="itemType">要移除的武器（或能力）对应的物品 ID。</param>
        public static void RemoveExecutionProgress(this Player player, int itemType)
        {
            player.HJScarlet().ExecutionListStored.Remove(itemType);
        }
        /// <summary>
        /// <para>移除玩家处决位（自动使用玩家手持物品作为键）。</para>
        /// <para>这是一个重载方法，内部调用 <see cref="RemoveExecutionProgress(Player, int)"/>，并传入 <c>player.HeldItem.type</c>。</para>
        /// <para><b>潜在风险</b>：若玩家在满足处决条件时突然切换武器，此方法可能会误将原武器的处决进度移除，导致意外重置。调用时请确保当前时机安全（例如处决触发后或武器未变化时）。</para>
        /// </summary>
        /// <param name="player">目标玩家。</param>
        public static void RemoveExecutionProgress(this Player player)
        {
            player.HJScarlet().ExecutionListStored.Remove(player.HeldItem.type);
        }
    }
}
