using HJScarletRework.Globals.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.NPCs.Bosses.VanillaOverride
{
    public interface INPCEqualsCondition
    {
        bool IsCorrected(int type);
    }
    public class NPCEqualConditionTrue : INPCEqualsCondition
    {
        public bool IsCorrected(int type) => true;
    }
    public class NPCEqualCondition : INPCEqualsCondition
    {
        public int Type;
        public NPCEqualCondition(int type)
        {
            Type = type;
        }
        public bool IsCorrected(int type) => type == Type;
    }
    public class NPCEqualConditionRange : INPCEqualsCondition
    {
        public int[] Types;
        public NPCEqualConditionRange(IEnumerable<int> types)
        {
            Types = types.ToArray();
        }
        public NPCEqualConditionRange(params int[] types)
        {
            Types = types;
        }
        public bool IsCorrected(int type) => Types.Contains(type);
    }
    public class NPCEqualsTo
    {
        public List<INPCEqualsCondition> ConditionsList;
        public NPCEqualsTo()
        {
            ConditionsList =
            [
                new NPCEqualConditionTrue()
            ];
        }
        public NPCEqualsTo EqualsToType(int type)
        {
            ConditionsList.Add(new NPCEqualCondition(type));
            return this;
        }
        public NPCEqualsTo EqualsToTypeRange(params int[] types)
        {
            ConditionsList.Add(new NPCEqualConditionRange(types));
            return this;
        }
        public bool IsCorrected(int type) => ConditionsList.TrueForAll(t => t.IsCorrected(type));
    }
    public abstract class LostbeltModeBehaviour : GlobalNPC
    {
        public NPCEqualsTo IsEqualTo;
        public override bool InstancePerEntity => true;
        public bool ActiveLostbeltJourneyAI => HJScarletConfigServer.Instance.LostbeltJourneyTestAI;
        /// <summary>
        /// 倍击晕的时间
        /// <br>当其大于0时，会尝试在PreAI内阻断所有的AI进程</br>
        /// </summary>
        public int BreakTiming = 0;
        public bool FirstFrame = false;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && IsEqualTo.IsCorrected(entity.type);
        }
        public override void Load()
        {
            IsEqualTo = ApplyEquals();
            base.Load();
        }
        public abstract NPCEqualsTo ApplyEquals();
        public override bool PreAI(NPC npc)
        {
            if (!ActiveLostbeltJourneyAI)
                return true;
            else
            {
                if (!FirstFrame)
                {
                    OnFirstFrame(npc);
                    FirstFrame = true;
                }
                NpcAI(npc);
                return false;

            }
        }
        public virtual void OnFirstFrame(NPC npc) { }
        public virtual void NpcAI(NPC npc) { }
        public virtual void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitByItemSafe(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) { }
        public sealed override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByItem(npc, player, item, ref modifiers);
            if (!ActiveLostbeltJourneyAI)
                return;
            ModifyHitByAnything(npc, player, ref modifiers);
            ModifyHitByItemSafe(npc, player, item, ref modifiers);
        }
        public virtual void ModifyHitByProjectileSafe(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) { }
        public sealed override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
            if (!ActiveLostbeltJourneyAI)
                return;
            ModifyHitByAnything(npc, Main.player[projectile.owner], ref modifiers);
            ModifyHitByProjectileSafe(npc, projectile, ref modifiers);
        }
        public virtual void OnHitByItemSafe(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) { }
        public sealed override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(npc, player, item, hit, damageDone);
            if (!ActiveLostbeltJourneyAI)
                return;
            OnHitByItemSafe(npc, player, item, hit, damageDone);
        }
        public virtual void OnHitByProjectileSafe(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) { }
        public sealed override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
            if (!ActiveLostbeltJourneyAI)
                return;
            OnHitByProjectileSafe(npc, projectile, hit, damageDone);
        }
    }
}
