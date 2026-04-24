using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalProjs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int GlobalTargetIndex = -1;
        public bool FirstFrame = false;
        public bool IsHitOnEnablFocusMechanicProj = false;
        /// <summary>
        /// 射弹是否正在启用专注攻击的字段
        /// </summary>
        public bool ExecutionStrike = false;
        /// <summary>
        /// 当前射弹是否允许使用专注机制，标记用
        /// </summary>
        public bool HasExecutionMechanic = false;
        /// <summary>
        /// 启用了专注机制的射弹是否命中了一次NPC
        /// </summary>
        public bool AddFocusHit = false;
        public bool DefenderBuff = false;
        public float[] ExtraAI = new float[10];
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[projectile.owner];
            if (HasExecutionMechanic && !AddFocusHit && projectile.numHits < 1)
            {
                AddFocusHit = true;
            }
            ModifyDefenderProj(Owner, projectile, target);
            if (Owner.HJScarlet().blackKeyDoT && ExecutionStrike && Owner.HJScarlet().blackKeyTimer == 0)
            {
                //对的没错，这个鬼东西的减防数据存在了玩家类里面。
                Owner.AddBuff(BuffType<BlackKeyExecutionBuff>(), GetSeconds(5));
                target.HJScarlet().blackKeyDefensesReduces = Owner.HJScarlet().blackKeyReduceDefense;
                target.AddBuff(BuffType<BlackKeyExecutionBuff>(), GetSeconds(5));
                Owner.HJScarlet().blackKeyTimer = GetSeconds(8);
            }
        }
        public override void AI(Projectile projectile)
        {
            Player Owner = Main.player[projectile.owner];
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (Owner.HJScarlet().monkExecutor && projectile.type == ProjectileID.MonkStaffT3)
                {

                    projectile.frameCounter++;
                    if (projectile.frameCounter % 10 == 0)
                    {
                        //创建一个链表，搜索附近可能的单位
                        float searchDist = 1100f;
                        List<NPC> availableTarget = [];
                        foreach (NPC needTar in Main.ActiveNPCs)
                        {
                            if (availableTarget.Count > 3)
                                break;
                            bool legalTarget = needTar.CanBeChasedBy();
                            float distPerTar = Vector2.Distance(needTar.Center, projectile.Center);
                            if (legalTarget && distPerTar < searchDist)
                            {
                                //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                                availableTarget.Add(needTar);
                            }
                        }
                        //确保链表正确
                        if (availableTarget.Count == 0)
                        {
                            return;
                        }
                        SoundEngine.PlaySound(SoundID.Item43 with { Pitch = 0.4f }, Owner.Center);
                        for (int i = 0; i < availableTarget.Count; i++)
                        {
                            NPC target = availableTarget[i];
                            Vector2 pos = Owner.Center - Vector2.UnitY * Main.rand.NextFloat(800f, 900f) + Vector2.UnitX * Main.rand.NextFloat(0f, 20f) * Main.rand.NextBool().ToDirectionInt();
                            Vector2 vel = (target.Center - pos).ToSafeNormalize() * Main.rand.NextFloat(4f, 9f);

                            Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), pos, vel, ProjectileType<SkyDragonFuryLightning>(), projectile.damage, 3f, Owner.whoAmI);
                            ((SkyDragonFuryLightning)proj.ModProjectile).CurTarget = target;
                            pos = projectile.Center + (target.Center - Owner.Center).ToSafeNormalize() * 50f * projectile.scale;
                            new CrossGlow(pos, Color.DeepSkyBlue, 30, 1, 0.12f).Spawn();
                            new CrossGlow(pos, Color.White, 30, 1, 0.08f).Spawn();
                            for (int j = 0; j < 8; j++)
                            {
                                Vector2 vel2 = (target.Center - Owner.Center).ToRandVelocity(ToRadians(30f), 1.2f, 8.8f);
                                new SmokeParticle(pos.ToRandCirclePos(10f), RandVelTwoPi(-3.8f, 2.6f) + vel2 - vel2 * Main.rand.NextFloat(0.1f, 1.2f), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 40, RandRotTwoPi, 1f, 0.35f).SpawnToPriorityNonPreMult();
                            }
                            for (int j = 0; j < 4; j++)
                            {
                                Vector2 vel2 = (target.Center - Owner.Center).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                                Vector2 pos2 = pos.ToRandCirclePos(12f) + vel2 * 0.32f;
                                new StarShape(pos2, vel2, RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 0.8f, 40).Spawn();
                            }
                            for (int j = 0; j < 6; j++)
                            {
                                Vector2 pos2 = pos.ToRandCirclePos(6f);
                                new ShinyCrossStar(pos2, RandVelTwoPi(1.2f, 4f), RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0, 1, 0.75f, false).Spawn();
                            }
                            for (int j = 0; j < 6; j++)
                            {
                                Vector2 pos2 = pos.ToRandCirclePos(6f);
                                Vector2 vel2 = (target.Center - Owner.Center).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                                new ShinyCrossStar(pos2, vel2, RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0, 1, 0.75f, false).Spawn();
                            }
                        }
                    }
                }

                if (Owner.HJScarlet().monkExecutor && projectile.type == ProjectileID.MonkStaffT1)
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter % 3 == 0)
                    {
                        Vector2 pos = projectile.Center.ToRandCirclePos(1f) + projectile.rotation.ToRotationVector2() * 40f * projectile.scale;
                        if (Collision.SolidCollision(pos, 120, 120))
                            pos = projectile.Center;
                        Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), pos, projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(4f, 9f), ProjectileType<SleepyBubbles>(), projectile.damage, 3f, Owner.whoAmI);
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { MaxInstances = 0 }, projectile.Center.ToRandCirclePos(1f));
                    }
                }

            }
            if (!FirstFrame)
            {
                FirstFrameEffect(projectile);
                FirstFrame = true;
            }

        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player owner = Main.player[projectile.owner];
            if(owner.whoAmI == Main.myPlayer)
            {
            }
        }

        private void FirstFrameEffect(Projectile projectile)
        {
            Player Owner = Main.player[projectile.owner];
            if (Owner.whoAmI != Main.myPlayer)
                return;
            ModifyPreciousTargets(Owner, projectile);
            ModifyDefenderEmblemBuff(Owner, projectile);
            if (Owner.HJScarlet().monkExecutor && projectile.type == ProjectileID.MonkStaffT1)
            {
                projectile.scale *= 1.2f;
                projectile.localNPCHitCooldown = 5;
                projectile.usesLocalNPCImmunity = true;

            }
            if (Owner.HJScarlet().monkExecutor && (projectile.type == ProjectileID.MonkStaffT3))
            {
                projectile.scale *= 2f;
                projectile.localNPCHitCooldown = 2;
                projectile.usesLocalNPCImmunity = true;
            }
            if (Owner.HJScarlet().monkExecutor && (projectile.type == ProjectileID.MonkStaffT3_Alt))
            {
                projectile.localNPCHitCooldown = 2;
                projectile.usesLocalNPCImmunity = true;
            }
        }

        private void ModifyDefenderEmblemBuff(Player owner, Projectile projectile)
        {
            bool legal = owner.HJScarlet().defenderEmblem && owner.HJScarlet().defenderEmblemCD == 0;
            if (!legal)
                return;
            bool anohterBool = projectile.IsLegalFriendlyProj(ExecutorDamageClass.Instance) && projectile.HJScarlet().ExecutionStrike;
            DefenderBuff = legal && anohterBool;
        }

        public void ModifyDefenderProj(Player owner, Projectile projectile, NPC target)
        {
            if(DefenderBuff && target.IsLegal())
            {
                owner.GetImmnue(ImmunityCooldownID.General, 60, true);
                SoundEngine.PlaySound(HJScarletSounds.GrabCharge with { MaxInstances= 0},owner.Center);
                owner.HJScarlet().defenderEmblemCD = 90;
                for (int i = 0; i < 30; i++)
                    new TurbulenceShinyOrb(owner.Center.ToRandCirclePos(15f), 2.4f, Color.White, 120, 0.885f, RandRotTwoPi).Spawn();
                DefenderBuff = false;
            }

        }

        private void ModifyPreciousTargets(Player owner, Projectile projectile)
        {
            //大部分“由“玩家直接通过shoot属性发射出去的射弹都会吃到这个加成。
            //衍生射弹除外
            //需注意的是这个写法排除了一些可能存在的手持射弹，如果真的有神人手持射弹也吃了这个加成……嗯我也不知道怎么说了。
            if (owner.whoAmI != projectile.owner)
                return;
            //干掉……嗯？
            bool activeing = owner.HJScarlet().PreciousTargetAcc && (owner.HJScarlet().PreciousTargetCrtis - 100f) < 100f;
            if (!activeing)
                return;
            //干掉任何不是由shoot属性直接发射出去的，与玩家手持的shoot一致的射弹
            //我知道有神人shoot什么属性都不会写，但是他都这样了哥们。
            if (owner.HeldItem.shoot != projectile.type)
                return;
            //干掉手持射弹
            //TODO
            if (projectile.whoAmI == owner.heldProj)
                return;
            //干掉没有伤害的射弹
            if (projectile.damage < 5)
                return;
            if (projectile.MaxUpdates > 1)
                return;
            //给当前射弹设置额外eu
            projectile.extraUpdates += 1;

        }
    }
}
