using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.ReVisual.Class
{
    public abstract class ReVisualProjClass : GlobalProjectile
    {
        public sealed override bool InstancePerEntity => true;
        public virtual int ApplyProj => -1;
        /// <summary>
        /// 是否允许原本正常绘制。默认否
        /// </summary>
        public virtual bool ApplyOriginalDraw => false;
        public List<Vector2> PosList = [];
        public List<float> RotList = [];
        public virtual int TotalListCount => 7;
        public ReVisualPlayer VisualOwner => Main.LocalPlayer.GetModPlayer<ReVisualPlayer>();

        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public sealed override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ApplyProj;
        }
        public sealed override void AI(Projectile projectile)
        {
            IsMyPlayer(projectile, out ReVisualPlayer vp);
            if (ShouldApplyRevisual(projectile, vp))
            {
                RevisualUpdate(projectile);
                base.AI(projectile);
                return;
            }
            else
                base.AI(projectile);
        }
        /// <summary>
        /// 复写这个钩子，以绘制需要的东西，或者你想做点什么，也可以。
        /// 尽量不要动原本的代码
        /// 如果ShouldApplyRevisual返回为真，不会执行。
        /// </summary>
        /// <param name="proj"></param>
        public virtual void RevisualUpdate(Projectile proj)
        {
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            IsMyPlayer(projectile, out ReVisualPlayer vp);
            if (ShouldApplyRevisual(projectile, vp))
            {
                PreDrawRevisual(projectile, ref lightColor);
                return ApplyOriginalDraw;
            }
            else
                return base.PreDraw(projectile, ref lightColor);
        }
        public virtual void PreDrawRevisual(Projectile proj, ref Color lightColor) {; }

        /// <summary>
        /// 复写这个钩子，以确定你应当在什么时候禁用特效重置
        /// 如果返回真，则允许修改特效，并执行RevisualUpdate钩子，即不执行后续的特效修改
        /// 默认返回否
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public virtual bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp) => false;
        public bool IsMyPlayer(Projectile proj, out ReVisualPlayer reVisual)
        {
            reVisual = Main.LocalPlayer.GetModPlayer<ReVisualPlayer>();
            return proj.owner == Main.myPlayer;
        }
        public void AddListOnNeed(Projectile proj)
        {
            if(!proj.HJScarlet().FirstFrame)
            {
                PosList.Clear();
                RotList.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                }
            }
            PosList.Add(proj.Center);
            RotList.Add(proj.rotation);
            if (RotList.Count > TotalListCount)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount)
                PosList.RemoveAt(0);

        }
    }
}
