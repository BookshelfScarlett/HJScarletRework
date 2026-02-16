using HJScarletRework.Buffs;
using HJScarletRework.Globals.Methods;
using Microsoft.Build.ObjectModelRemoting;
using rail;
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
        public float GeneralCrtiDamageAdd = 0f;
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
        }
        public void ModifyCritDamage(NPC target, ref NPC.HitModifiers modifiers)
        {
            float totalCritsBonus = 0f;
            if (CreationHatSet && modifiers.DamageType == DamageClass.Magic)
            {
                //将所有伤害直接设置为暴击类型，这里先过暴击情况
                modifiers.SetCrit();
                //而后开始依据当前的暴击率设置需要的暴击伤害
                //首先将溢出的暴击概率等价转化
                float baseCritsbuff = Math.Max(0f, GetWantedCrits<MagicDamageClass>());
                //转化成功后，将值/2f，取暴击率的1/2（即20%-> 10%)
                baseCritsbuff /= 2f;
                //最后。直接将暴击伤害设置
                totalCritsBonus += baseCritsbuff;
            }
            //精确打击取溢出暴击率的100伤害。
            if (PreciousTargetCrtis > 0 && PreciousTargetAcc)
            {
                float critBuff = Player.HeldItem.crit;
                totalCritsBonus += critBuff;
            }
            totalCritsBonus += GeneralCrtiDamageAdd;
            modifiers.CritDamage += totalCritsBonus;

        }
        public float GetWantedCrits<Type>() where Type : DamageClass
        {
            return (Player.GetTotalCritChance<Type>() + 4f - 100f);
        }

        public void GlobalOnHitNPCWithSomething(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (PreciousTargetAcc && hit.Crit)
                PreciousTargetCrtis += 5;
            //帝国的荣耀相关buff都写在里面了
            if (!target.friendly && target.lifeMax >= 5 && target.CanBeChasedBy() && Player_RewardofWarrior)
            {
                RewardofWarriorCounter = 180;
                //给予5帧短暂的cd
                if (RewardofWarriorHitCD == 0)
                {
                    RewardofWarriorHitCD = 5;
                    RewardLevel += 1;
                }
                
                if (RewardLevel > 30)
                {
                    if (!Player.HasBuff<RewardsofWarriorBuff>())
                    {
                        Player.AddBuff(BuffType<RewardsofWarriorBuff>(), 300);
                        RewardLevel = 0;
                    }
                    else
                    {
                        RewardLevel = 30;
                    }
                }
                //如果佩戴了上位饰品，在下方进行防御力递增
                if (Player_RewardofKingdom)
                {
                    //这里的Counter必须得启用，每次过来都会刷新
                    //顶多就是续一个180秒，持续攻击这一块
                    if (KingdomDefenseTime < 30)
                        KingdomDefenseTime += 1;
                }
            }
        }
    }
}
