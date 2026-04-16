using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
/// <summary>
/// Credit from LAP
/// </summary>

namespace HJScarletRework.Globals.Systems
{
    public struct DashDamageInfo(int damage, float knockBack, DamageClass dc)
    {
        public int Damage = damage;
        public float KnockBack = knockBack;
        public DamageClass damageClass = dc;
    }

    public class ScarletDashPlayer : ModPlayer
    {
        public static List<PlayerDashClass> DashCollection = [];
        public int CurDashID;
        /// <summary>
        /// 这个覆盖后必须执行完此次冲刺才会重置
        /// </summary>
        public int OverideCurDashID = -1;
        // 冲刺计时
        public int DashTime = 0;
        // 冲刺冷却
        public int DashDelay = 0;
        public int VanillaDashInput;
        public int BeginDirection;
        public bool BeginDash;
        // 记录每个NPC的whoami和对应的冷却
        public int[] NPCImmuneTime = new int[Main.maxNPCs];
        public override void ResetEffects()
        {
            CheckNPCImmuneTime();
            OtherReset();
        }
        public override void PostUpdateRunSpeeds()
        {
            if (DashCollection.Count == 0)
                return;
            if (CurDashID == -1 && OverideCurDashID == -1)
                return;
            // 这两个原版源码判了
            if (Player.grappling[0] == -1 && !Player.tongued)
            {
                int Index = CurDashID;
                if (OverideCurDashID != -1)
                    Index = OverideCurDashID;
                PlayerDashClass ActiveDash = DashCollection[Index];
                // 监测是否开始冲刺
                HandleDashBegin(out bool ThisCanDash);
                if (!ActiveDash.PreDash(Player))
                    return;
                if (ThisCanDash)
                {
                    // 如果开始冲刺，赋值并应用起始效果
                    ActiveDash.OnDashStart(Player);
                    DashTime = ActiveDash.DashTime(Player);
                    Player.SetImmuneTimeForAllTypes(ActiveDash.ImmuneTime(Player));
                }
                if (DashTime > 0)
                {
                    if (!ActiveDash.UseCustomDashSpeed)
                    {
                        float PlayerXVel = BeginDirection * Vector2.UnitX.X * ActiveDash.DashSpeed(Player);
                        if (MathF.Abs(Player.velocity.X) < MathF.Abs(PlayerXVel))
                            Player.velocity.X = MathHelper.Lerp(PlayerXVel * ActiveDash.DashEndSpeedMult(Player), PlayerXVel, ActiveDash.DashAmount(Player, DashTime, ActiveDash.DashTime(Player)));
                    }
                    else
                        ActiveDash.ModifyDashSpeed(Player);
                    ActiveDash.UpdateDash(Player);
                    BeginDash = true;
                    CheckNPCHit(ActiveDash);
                }
                if (BeginDash && DashTime == 0)
                {
                    ActiveDash.OnDashEnd(Player);
                    DashDelay = ActiveDash.DashDelay(Player);
                    BeginDash = false;
                    OverideCurDashID = -1;
                }
            }
        }
        public void HandleDashBegin(out bool CanDash)
        {
            bool canDash = false;
            CanDash = canDash;
            if (DashTime > 0 || DashDelay > 0 || BeginDash) // 冲刺或CD时时始终不可再次冲刺
                return;
            BeginDirection = Player.direction;
            // 原版的双击冲刺判定
            bool vanillaLeftDashInput = Player.controlLeft && Player.releaseLeft;
            bool vanillaRightDashInput = Player.controlRight && Player.releaseRight;
            if (vanillaRightDashInput)
            {
                if (VanillaDashInput > 0)
                {
                    BeginDirection = 1;
                    canDash = true;
                    VanillaDashInput = 0;
                }
                else
                    VanillaDashInput = 15;
            }
            else if (vanillaLeftDashInput)
            {
                if (VanillaDashInput < 0)
                {
                    BeginDirection = -1;
                    canDash = true;
                    VanillaDashInput = 0;
                }
                else
                    VanillaDashInput = -15;
            }
            CanDash = canDash;
        }
        public void CheckNPCImmuneTime()
        {
            for (int i = 0; i < NPCImmuneTime.Length; i++)
            {
                if (NPCImmuneTime[i] > 0)
                    NPCImmuneTime[i]--;
            }
        }
        public void OtherReset()
        {
            if (DashTime > 0)
                DashTime--;
            if (DashDelay > 0)
                DashDelay--;
            if (VanillaDashInput < 0)
                VanillaDashInput++;
            else if (VanillaDashInput > 0)
                VanillaDashInput--;
            CurDashID = -1;
        }
        public void CheckNPCHit(PlayerDashClass ActiveDash)
        {
            Rectangle hitArea = new Rectangle((int)(Player.position.X + Player.velocity.X * 0.5 - 4f), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[n.type])
                    continue;
                if (NPCImmuneTime[n.whoAmI] > 0)
                    return;
                if (!ActiveDash.CanHitNPC(Player, n))
                    continue;
                if (!n.dontTakeDamage && !n.friendly)
                {
                    if (ActiveDash.Colliding(hitArea, n.Hitbox) && (n.noTileCollide || Player.CanHit(n)))
                    {
                        int npcPreDamageHP = n.life;
                        DashDamageInfo dashDamageInfo = ActiveDash.DashDamageInfo(Player);
                        ActiveDash.ModifyDashDamage(Player, ref dashDamageInfo);
                        int dashDamage = (int)Player.GetTotalDamage(dashDamageInfo.damageClass).ApplyTo(dashDamageInfo.Damage);
                        float dashKB = Player.GetTotalKnockback(dashDamageInfo.damageClass).ApplyTo(dashDamageInfo.KnockBack);
                        bool crit = Main.rand.Next(100) < Player.GetTotalCritChance(dashDamageInfo.damageClass);
                        Player.ApplyDamageToNPC(n, dashDamage, dashKB, Player.direction, crit, dashDamageInfo.damageClass, true);
                        Player.SetImmuneTimeForAllTypes(ActiveDash.DashHitImmuneTime(Player));
                        NPCImmuneTime[n.whoAmI] = ActiveDash.DashHitCoolDown(Player);
                        int npcPostDamageHP = n.life;
                        ActiveDash.OnHitNPC(Player, n, npcPreDamageHP - npcPostDamageHP);
                    }
                }
            }
        }
    }
    public class PlayerDashClass : ModType
    {
        public int Type;
        /// <summary>
        /// 他为True时会使用模版计算速度，否则会调用ModifyDashSpeed来完全接管速度计算
        /// </summary>
        public bool UseCustomDashSpeed;
        /// <summary>
        /// 这个冲刺的来源
        /// </summary>
        public IEntitySource Source;
        /// <summary>
        /// 如果你需要点什么允许冲刺的条件的话，复写这个。
        /// </summary>
        public virtual bool PreDash(Player player) => true;
        public virtual bool CanHitNPC(Player player, NPC target) => true;
        public virtual DashDamageInfo DashDamageInfo(Player player) => new(50, 3, DamageClass.Default);
        /// <summary>
        /// 这个冲刺给予的无敌时间
        /// </summary>
        public virtual int ImmuneTime(Player player) => 12;
        /// <summary>
        /// 这个冲刺撞击敌人后给予的无敌时间
        /// </summary>
        public virtual int DashHitImmuneTime(Player player) => 12;
        /// <summary>
        /// 冲刺的持续时间
        /// </summary>
        public virtual int DashTime(Player player) => 12;
        /// <summary>
        /// 冲刺的冷却
        /// </summary>
        public virtual int DashDelay(Player player) => 30;
        /// <summary>
        /// 冲刺开始时调用
        /// </summary>
        public virtual void OnDashStart(Player player)
        {
        }
        /// <summary>
        /// 冲刺途中调用
        /// </summary>
        public virtual void UpdateDash(Player player)
        {
        }
        /// <summary>
        /// 冲刺结束后调用
        /// </summary>
        public virtual void OnDashEnd(Player player)
        {
        }
        /// <summary>
        /// 冲刺的速度
        /// </summary>
        public virtual float DashSpeed(Player player) => 10f;
        /// <summary>
        /// 冲刺结束时的速度
        /// </summary>
        public virtual float DashEndSpeedMult(Player player) => 0.33f;
        /// <summary>
        /// 冲刺减速的参数
        /// </summary>
        public virtual float DashAmount(Player player, int CurAni, int MaxAni)
        {
            return (float)CurAni / MaxAni;
        }
        /// <summary>
        /// 覆写他会完全接管冲刺速度的计算
        /// </summary>
        public virtual void ModifyDashSpeed(Player player)
        {

        }
        /// <summary>
        /// 击中后的冷却时间
        /// </summary>
        public virtual int DashHitCoolDown(Player player) => 12;
        /// <summary>
        /// 允许你修改碰撞逻辑
        /// </summary>
        public virtual bool Colliding(Rectangle dashHitbox, Rectangle targetHitbox)
        {
            if (dashHitbox.Intersects(targetHitbox))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 允许你修改冲刺的伤害
        /// </summary>
        public virtual void ModifyDashDamage(Player player, ref DashDamageInfo dashDamageInfo)
        {
        }
        /// <summary>
        /// 击中敌对NPC时调用
        /// </summary>
        public virtual void OnHitNPC(Player player, NPC target, int DamageDone)
        {
        }
        protected sealed override void Register()
        {
            Type = ScarletDashPlayer.DashCollection.Count;
            ScarletDashPlayer.DashCollection.Add(this);
            SSD();
        }
        public virtual void SSD()
        {

        }
    }
}
