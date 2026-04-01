using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.Huds
{
    public abstract class ExecutionHud : ModType
    {
        /// <summary>
        /// 允许发起处决攻击的武器ID
        /// 这个会用于绘制武器
        /// </summary>
        public virtual int ExecutionItemType => -1;
        /// <summary>
        /// 武器对应的总处决次数
        /// </summary>
        public int TotalExecutionTime = -1;
        /// <summary>
        /// 缓存的上一把武器名，这个会用于一些缓动的切换
        /// </summary>
        public int CachedLastWeapon = -1;
        /// <summary>
        /// 当前处决进度
        /// </summary>
        public int CurExecutionTime = -1;
        public virtual void OnMouseHovering()
        {
            if (MouseHover)
                Scale = Lerp(Scale, 1.1f, 0.2f);
            else
                Scale = Lerp(Scale, 1f, 0.2f);
        }
        public virtual bool Predraw() { return true; }
        public virtual void Update() { }
        public virtual void FadeIn()
        {
            if(CanFaded)
            {
                Opacity = Lerp(Opacity, 0f, 0.2f);
                if (Opacity < 0.02f)
                    Opacity = 0;
            }
            else
            {
                Opacity = Lerp(Opacity, 1f, 0.2f);
                if (Opacity > 0.98f)
                    Opacity = 1f;
            }
        }
        public virtual void PostDraw() { }
        /// <summary>
        /// 在正式发起处决的时候触发的效果
        /// 主要用于特效相关
        /// </summary>
        public virtual void OnExecutionActive(Player player) { }
        protected sealed override void Register()
        {
        }
        public Vector2 Position;
        public float Opacity;
        public bool CanFaded;
        public bool MouseHover;
        public float Scale;
        public Rectangle Hitbox;
        public virtual void SetDefaults()
        {
            Position = new Vector2(500, 500);
            Opacity = 1f;
            CanFaded = false;
            Hitbox = Utils.CenteredRectangle(Position, new(200));
            Scale =1f;
        }
    }
}
