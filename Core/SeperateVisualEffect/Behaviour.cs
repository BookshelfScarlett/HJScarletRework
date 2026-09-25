using HJScarletRework.Globals.Database.Enums;
using Terraria.ModLoader;

namespace HJScarletRework.Core.SeperateVisualEffect
{
    public class SeperateVisualBehavior : ModType
    {
        public SeperateVisualInstance Instance = null;
        public int Type { get; set; }
        protected sealed override void Register()
        {
            Type = SeperateVisualManager.VisualBehaviors.Count;
            if (!SeperateVisualManager.VisualBehaviors.Contains(this))
                SeperateVisualManager.VisualBehaviors.Add(this);
        }
        /// <summary>
        /// 决定这个分离视效的处于的渲染层
        /// </summary>
        public virtual ScarletDrawLayer DrawLayer => ScarletDrawLayer.AfterDusts;
        /// <summary>
        /// 决定这个分离视效的混合模式
        /// </summary>
        public virtual BlendState BlendState => BlendState.Additive;
        public virtual void OnSpawn() { }
        /// <summary>
        /// 决定是否使其受到<see cref="SeperateVisualInstance.Velocity"/>速度的影响
        /// <br>类似于射弹的速度</br>
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldUpdatePosition() => true;
        public virtual void Update() { }
        public virtual void OnDestroy() { }
        public virtual void Draw() { }
        public virtual SeperateVisualBehavior CloneForSpawner()
        {
            return MemberwiseClone() as SeperateVisualBehavior;
        }
    }
}
