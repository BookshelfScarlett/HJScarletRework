using HJScarletRework.Core.Primitives.Trail;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Core.Lightning
{
    public struct LightningNode(Vector2 pos, float width, List<Vector2> children = null)
    {
        public Vector2 Position = pos;
        public float Width = width;
        public List<Vector2> Children = children;
    }
    public class HJLightning
    {
        // 缓存计算好的轨迹几何数据
        public List<List<TrailDrawDate>> CachedTrails = new();
        public float RandomFlowOffset;
        public float xScale;
        public bool Active;
        public int WhoAmI;
        public int lifeTime;
        public int MaxlifeTime;
        public List<LightningNode> Nodes = new();
        public float Width;
        public Color Color;
        public float Opacity;
        public void Reset()
        {
            CachedTrails.Clear(); // 清理缓存
            Nodes.Clear();
            RandomFlowOffset = Main.rand.NextFloat(0f, 1000f);
            Active = false;
            lifeTime = 0;
            MaxlifeTime = 0;
            Width = 0f;
            Color = Color.White;
            Opacity = 1f;
        }
        public void Update()
        {
            float progress = (float)lifeTime / MaxlifeTime;
            // 基础淡出曲线
            float baseAlpha = Lerp(1f, 0f, EaseOutCubic(progress));

            Opacity = baseAlpha;

            lifeTime++;
            if (lifeTime > MaxlifeTime)
                Active = false;
        }
        public void GenerateCache()
        {
            CachedTrails.Clear();
            if (Nodes == null || Nodes.Count == 0)
                return;

            // 1. 缓存主干
            List<TrailDrawDate> trunk = new List<TrailDrawDate>(Nodes.Count);
            for (int i = 0; i < Nodes.Count; i++)
            {
                float rot = 0;
                if (i < Nodes.Count - 1)
                    rot = (Nodes[i + 1].Position - Nodes[i].Position).ToRotation();
                else if (i > 0)
                    rot = (Nodes[i].Position - Nodes[i - 1].Position).ToRotation();

                trunk.Add(new TrailDrawDate(Nodes[i].Position, Color.White, new Vector2(0, Nodes[i].Width), rot));
            }
            CachedTrails.Add(trunk);
            for (int i = 0; i < Nodes.Count; i++)
            {
                LightningNode node = Nodes[i];
                if (node.Children != null && node.Children.Count > 1)
                {
                    List<TrailDrawDate> branch = new List<TrailDrawDate>(node.Children.Count);
                    float beginWidth = node.Width;
                    for (int j = 0; j < node.Children.Count; j++)
                    {
                        float branchRot = 0;
                        if (j < node.Children.Count - 1)
                            branchRot = (node.Children[j + 1] - node.Children[j]).ToRotation();
                        else if (j > 0)
                            branchRot = (node.Children[j] - node.Children[j - 1]).ToRotation();

                        float progressAlongPath = (float)j / node.Children.Count;
                        float currentWidth = beginWidth * MathHelper.Lerp(1f, 0.2f, progressAlongPath);

                        branch.Add(new TrailDrawDate(node.Children[j], Color.White, new Vector2(0, currentWidth), branchRot));
                    }
                    CachedTrails.Add(branch);
                }
            }
        }
    }
}
