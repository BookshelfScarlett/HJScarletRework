using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalProjs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int GlobalTargetIndex = -1;
        public bool FirstFrame = false;
        public bool FocusStrike = false;
        public bool UseFocusStrikeMechanic = false;
        public bool IsHitOnEnablFocusMechanicProj = false;
        public float[] ExtraAI = new float[10];
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
        }

        private void ModifyPreciousTargets(Player owner, Projectile projectile)
        {
            //大部分“由“玩家直接通过shoot属性发射出去的射弹都会吃到这个加成。
            //衍生射弹除外
            //需注意的是这个写法排除了一些可能存在的手持射弹，如果真的有神人手持射弹也吃了这个加成……嗯我也不知道怎么说了。
            if (projectile.owner != Main.myPlayer)
                return;
            //干掉……嗯？
            if (!owner.HJScarlet().PreciousTargetAcc || owner.HJScarlet().PreciousTargetCrtis - 100f < 100f)
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

            //给当前射弹设置额外eu
            projectile.extraUpdates += 1;

        }
    }
}
