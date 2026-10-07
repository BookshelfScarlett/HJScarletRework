using ContinentOfJourney;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Armor.ExecutorAlter;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Projs;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Projs
{
    public partial class HJScarletGlobalProjs : GlobalProjectile
    {
        public bool HasCreatedProj = false;
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ScarletProjIDSets.GiantKiller[projectile.type] && ScarletNPCIDSets.Giant[target.type])
            {
                modifiers.FinalDamage *= BalacingHandler.GiantKillerDamageMult;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[projectile.owner];
            switch (projectile.type)
            {
                case ProjectileID.NightsEdge:
                case ProjectileID.TrueNightsEdge:
                    target.AddBuff(BuffType<ForeverNightBuff>(), 60);
                    break;
            }
            //判定是否为悠悠球
            bool isYoyo = projectile.aiStyle == ProjAIStyleID.Yoyo && projectile.DamageType.CountsAsClass<ExecutorDamageClass>();
            if (HasExecutionMechanic && ((!AddExecutionHit && projectile.numHits < 1) || isYoyo))
            {
                HandleCowboy(Owner, target);
                HandlePendant(Owner, target);
                if (!isYoyo)
                    AddExecutionHit = true;
            }
            HandleMaidReaperOnHit(Owner, projectile, target);
            HandleBlackKeyOnHit(Owner);
            if (projectile.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                if (Owner.HJScarlet().KnifeMarkIndex == ProjectileType<TearEyeMark>())
                {
                    Owner.HJScarlet().tearEyeBuff = GetSeconds(1);
                }
                if (Owner.HJScarlet().KnifeMarkIndex == ProjectileType<MoltenKnifeMark>() && projectile.type != ProjectileType<MoltenKnifeBoom>() && projectile.type != ProjectileType<MoltenKnifeProj>() && Owner.HJScarlet().markProjTimer == 0 && HasExecutionMechanic)
                {
                    int dmg = (int)(projectile.originalDamage * MoltenKnife.BoomDamageMult);
                    Owner.HJScarlet().markProjTimer = GetInstance<MoltenKnife>().Item.useTime;
                    Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<MoltenKnifeBoom>(), dmg, 1f, Owner.whoAmI);
                }
                if (Owner.HJScarlet().KnifeMarkIndex == ProjectileType<GrassKnifeMark>() && projectile.type != ProjectileType<GrassKnifePoisonProj>() && projectile.type != ProjectileType<InvisBoom>() && Owner.HJScarlet().markProjTimer == 0)
                {
                    Owner.HJScarlet().markProjTimer = GetInstance<GrassKnife>().Item.useTime;
                    if (target.HasBuff<GrassPoison>())
                    {
                        foreach (var proj in Main.ActiveProjectiles)
                        {
                            if (proj.type != ProjectileType<GrassKnifePoisonProj>())
                                continue;
                            if (proj.owner != Owner.whoAmI)
                                continue;
                            ((GrassKnifePoisonProj)proj.ModProjectile).StackLevel += 1;
                            target.AddBuff(BuffType<GrassPoison>(), GetSeconds(10));
                            proj.timeLeft = GetSeconds(10);
                        }
                    }
                    else
                    {
                        if (!HasCreatedProj)
                        {
                            target.AddBuff(BuffType<GrassPoison>(), GetSeconds(10));
                            int damageValueInstance = 25 * (1 + DownedBossSystem.downedBarrier.ToInt() * 2 + Condition.Hardmode.IsMet().ToInt());
                            Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<GrassKnifePoisonProj>(), damageValueInstance / 5, 0, Owner.whoAmI);
                            HasCreatedProj = true;
                            proj.originalDamage = damageValueInstance;
                            ((GrassKnifePoisonProj)proj.ModProjectile).CurTarget = target;
                        }
                    }
                }
            }
            if (Owner.HJScarlet().KnifeMarkIndex != -1)
                Owner.HJScarlet().KnifeMarkIndex = -1;
        }

        public void HandlePendant(Player owner, NPC target)
        {
            HJScarletPlayer usPlayer = owner.HJScarlet();
            if (owner.HJScarlet().pendantLevel < 1)
                return;
            if (!owner.HeldItem.IsExecutorWeapon())
                return;

            foreach (var keys in usPlayer.ExecutionListStored.Keys.ToList())
            {
                if (HJScarletMethods.CheckExecuteTypes(keys, ExecutorWeaponType.Assistance))
                {
                    owner.AddExecutionTimeDirectly(keys, 1 * usPlayer.pendantLevel);
                }
            }
        }

        public void HandleCowboy(Player Owner, NPC target)
        {
            if (Owner.HJScarlet().cowboyExecutor && Owner.HJScarlet().cowboyRevolverTimer == 0)
            {
                int revolverDamage = (int)Owner.GetTotalDamage<ExecutorDamageClass>().ApplyTo(CowboyHelmet.RevolerDamage);
                Projectile proj2 = Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), target.Center, (-Vector2.UnitY).ToRandVelocity(ToRadians(35f), 9f, 11f), ProjectileType<CowboyRevolverProj>(), revolverDamage, 0f, Owner.whoAmI);
                proj2.timeLeft = 300;
                if (target.CanBeChasedBy())
                    ((CowboyRevolverProj)proj2.ModProjectile).CurTarget = target;
                Owner.HJScarlet().cowboyRevolverTimer = 30;
            }
        }

        public void HandleBlackKeyOnHit(Player owner)
        {
            if (owner.HJScarlet().blackKeyDoT && ExecutionStrike && owner.HJScarlet().blackKeyTimer == 0)
            {
                //对的没错，这个鬼东西的减防数据存在了玩家类里面。
                owner.AddBuff(BuffType<BlackKeyExecutionBuff>(), GetSeconds(5));
                owner.HJScarlet().blackKeyTimer = GetSeconds(10);
                if (owner.HJScarlet().blackKeyDefenseBuff != 0)
                    owner.HJScarlet().blackKeyDefenseTrigger = true;
            }
        }

        public void HandleMaidReaperOnHit(Player owner, Projectile proj, NPC target)
        {
            if (owner.HJScarlet().maidReaperArmor && target.IsLegal())
            {
                owner.HJScarlet().maidReaperIndex = target.whoAmI;
            }
        }

    }
}
