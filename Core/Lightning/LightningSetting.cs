using Microsoft.Xna.Framework;

namespace HJScarletRework.Core.Lightning
{
    public struct LightningSetting(Vector2 begin, Vector2 end, Color color, float strength, float width, int lifetime, int generationsStep, float branchChance, int maxBranchGenerations,
        float distanceProtect = 100, float strengthDecay = 0.6f, float maxBranchAllowedDistance = 50f)
    {
        // 起点
        public Vector2 Begin = begin;
        // 终点
        public Vector2 End = end;
        // 辉光的颜色
        public Color color = color;
        // 扭曲强度
        public float strength = strength;
        // 宽度
        public float width = width;
        // 闪电宽度
        public int lifetime = lifetime;
        // 生成多少个节点
        public int GenerationsStep = generationsStep;
        // 分支生成概率
        public float BranchChance = branchChance;
        // 分支生成最大步进
        public int MaxBranchGenerations = maxBranchGenerations;
        // 主闪电每次迭代的强度衰减系数，范围为 0~1，越小衰减越快
        public float StrengthDecay = strengthDecay;
        // 分支允许偏离主干的最大距离
        public float MaxBranchAllowedDistance = maxBranchAllowedDistance;
        // 分支生成距离保护，确保两个分支不会距离太近
        public float DistanceProtect = distanceProtect;
    }
}
