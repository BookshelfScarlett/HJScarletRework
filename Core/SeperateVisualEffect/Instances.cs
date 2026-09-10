using System.Collections.Generic;

namespace HJScarletRework.Core.SeperateVisualEffect
{
    public class SeperateVisualInstance
    {
        public SeperateVisualBehavior Behavior { get; set; }
        public int WhoAmI;
        public bool Active;
        /// <summary>
        /// 当前分离视效的持续时间<br></br>
        /// 与生存时间区分开来
        /// </summary>
        public int Time;
        /// <summary>
        /// 当前分离视效的最大存续时间
        /// </summary>
        public int LifeTime;
        /// <summary>
        /// 持续时间与生存时间的归一化比率
        /// </summary>
        public float LifeTimeRatios => Time / (float)LifeTime;
        /// <summary>
        /// 当前视效的“额外更新”
        /// </summary>
        public int ExtraUpdates;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale;
        public Vector2 ScaleVector2;
        public float Opacity;
        /// <summary>
        /// 只有6个栏位
        /// </summary>
        public float[] aifloats = new float[6];
        public int[] aiints = new int[6];
        public bool[] aibool = new bool[6];
        public List<Vector2> OldPos = new List<Vector2>();
        public List<float> OldRot = new List<float>();
        public void Reset()
        {
            Behavior = null;
            WhoAmI = 0;
            Active = false;
            Time = 0;
            LifeTime = 0;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Origin = Vector2.Zero;
            DrawColor = Color.White;
            Rotation = 0;
            Scale = 1;
            ScaleVector2 = Vector2.One;
            Opacity = 1;
            for (int i = 0; i < aifloats.Length; i++)
                aifloats[i] = 0;
            for (int i = 0; i < aiints.Length; i++)
                aiints[i] = 0;
            for (int i = 0; i < aibool.Length; i++)
                aibool[i] = false;
            OldPos.Clear();
            OldRot.Clear();
        }
    }
}
