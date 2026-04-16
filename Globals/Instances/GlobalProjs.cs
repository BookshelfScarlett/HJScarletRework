using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
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
            if (!FirstFrame)
            {
                FirstFrameEffect(projectile);
                FirstFrame = true;
            }
        }

        private void FirstFrameEffect(Projectile projectile)
        {
            Player Owner = Main.player[projectile.owner];
            ModifyPreciousTargets(Owner, projectile);
            ModifyDefenderEmblemBuff(Owner, projectile);
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
