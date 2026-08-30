using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;

namespace HJScarletRework.Core.GJKCollision
{
    public interface IConvexShape
    {
        Vector2 Support(Vector2 direction);
        Vector2 Center { get; }
    }
    /// <summary>
    /// 一个凸多边形的实现
    /// <br>如果是对于圆/椭圆，实现起来更为容易，但对应情况下可以直接使用tmod本来就提供的aabb方法处理碰撞</br>
    /// <br>这里只考虑凸多边形的情况</br>
    /// </summary>
    public class ConvexPolygon : IConvexShape
    {
        /// <summary>
        /// 用于表示世界坐标下的顶点
        /// </summary>
        public Vector2[] Vertices;
        public Vector2 Center
        {
            get
            {
                if (Vertices == null || Vertices.Length == 0)
                    return Vector2.Zero;
                Vector2 sum = Vector2.Zero;
                foreach (Vector2 v in Vertices)
                    sum += v;
                return sum / Vertices.Length;
            }
        }
        public Vector2 Support(Vector2 direction)
        {
            float maxDot = float.NegativeInfinity;
            Vector2 best = Vertices[0];
            foreach (var v in  Vertices)
            {
                float dot = Vector2.Dot(v, direction);
                if(dot >maxDot)
                {
                    maxDot = dot;
                    best = v;
                }
            }
            return best;
        }
        /// <summary>
        /// 给定A与B形状，计算闵可夫斯基差
        /// </summary>
        public static Vector2 SupportMinkowski(IConvexShape A, IConvexShape B, Vector2 direction)
        {
            // A 在 direciton上的最远点 - B 在-direciton上的最远点 
            return A.Support(direction) - B.Support(direction);
        }
        /// <summary>
        /// GJK碰撞的核心实现
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        /// <returns></returns>
        public static bool GJKIntersect(IConvexShape shapeA, IConvexShape shapeB)
        {
            Vector2 direciton = GetInitialDirection(shapeA, shapeB);
            Vector2 support = SupportMinkowski(shapeA,shapeB,direciton);
            List<Vector2> simplex = new List<Vector2> { support };
            //方向取反
            direciton = -support;
            //最大迭代次数，避免死循环
            int maxIteration = 50;
            for (int i = 0; i < maxIteration; i++)
            {
                support = SupportMinkowski(shapeA, shapeB, direciton);
                if (Vector2.Dot(support, direciton) < 0)
                {
                    return false;
                }
                simplex.Add(support);
                //更新单行的方向和状态
                if (DoSimplex(ref simplex, ref direciton))
                    return true;
            }
            //迭代用尽的情况下默认相交，或者返回false判定不相交。看实际情况
            return true;
        }
        private static bool DoSimplex(ref List<Vector2> simplex, ref Vector2 direction)
        {
            return simplex.Count switch
            {
                2 => ProcessLine(ref simplex, ref direction),
                3 => ProcessTriangle(ref simplex, ref direction),
                _ => false,
            };
        }
        private static bool ProcessLine(ref List<Vector2> simplex, ref Vector2 direction)
        {
            Vector2 A = simplex[1]; // 最新点
            Vector2 B = simplex[0];
            Vector2 AB = B - A;
            Vector2 AO = -A; // 原点方向

            if (Vector2.Dot(AB, AO) > 0)
            {
                // 原点在 AB 同一侧，保留 AB，方向为 AB 的法线指向原点
                direction = TripleProduct(AB, AO, AB);
                // 移除 B 点
                simplex.RemoveAt(0);
            }
            else
            {
                // 原点在线段另一侧，只保留 A 点
                simplex = new List<Vector2> { A };
                direction = AO;
            }
            return false;
        }

        private static bool ProcessTriangle(ref List<Vector2> simplex, ref Vector2 direction)
        {
            Vector2 A = simplex[2]; // 最新点
            Vector2 B = simplex[1];
            Vector2 C = simplex[0];

            Vector2 AB = B - A;
            Vector2 AC = C - A;
            Vector2 AO = -A;

            // 计算法线（指向外侧）
            Vector2 ABperp = TripleProduct(AC, AB, AB);
            if (Vector2.Dot(ABperp, AO) > 0)
            {
                // 原点在 AB 外侧，移除 C
                simplex.RemoveAt(0);
                direction = ABperp;
                return false;
            }

            Vector2 ACperp = TripleProduct(AB, AC, AC);
            if (Vector2.Dot(ACperp, AO) > 0)
            {
                // 原点在 AC 外侧，移除 B
                simplex.RemoveAt(1);
                direction = ACperp;
                return false;
            }

            // 原点在三角形内部，相交！
            return true;
        }

        // 三重叉积用于计算垂直向量
        private static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c)
        {
            float dot = Vector2.Dot(a, c);
            return b * dot - a * Vector2.Dot(b, c);
        }
        /// <summary>
        /// 获取初始方向
        /// <br>常用的初始方向为，形状A指向形状B的中心点。</br>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static Vector2 GetInitialDirection(IConvexShape A, IConvexShape B)
        {
            Vector2 dir = B.Center - A.Center;
            return dir.ToSafeNormalize();
        }
    }
}
