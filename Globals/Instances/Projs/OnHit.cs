using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.ExecutorAlter;
using HJScarletRework.Projs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Projs
{
    public partial class  HJScarletGlobalProjs : GlobalProjectile
    {
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
            if (HasExecutionMechanic && !AddExecutionHit && projectile.numHits < 1)
            {
                HandleCowboy(Owner, target);
                AddExecutionHit = true;
            }
            HandleMaidReaperOnHit(Owner, projectile, target);
            HandleBlackKeyOnHit(Owner);
            ModifyDefenderProj(Owner, projectile, target);
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
