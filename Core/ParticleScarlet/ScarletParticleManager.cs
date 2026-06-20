using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.ParticleScarlet
{
    public partial class ScarletParticleManager : ModSystem
    {
        // 别在外部可以修改了，至少别人都加了readonly（
        /// <summary>
        /// 给list设定硬性上限，三万
        /// <para>通常情况下应该不会超出这个三万上限</para>
        /// 这样主要是为了保证内存连续
        /// </summary>
        private const int MaxParticleCountsSet = 30002;
        public static readonly List<ScarletParticle> ParticleAlpha = new(MaxParticleCountsSet);
        public static readonly List<ScarletParticle> ParticleNonPre = new(MaxParticleCountsSet);
        public static readonly List<ScarletParticle> ParticleAdditive = new(MaxParticleCountsSet);
        // 先绘制先更新的粒子
        public static readonly List<ScarletParticle> ParticleAlphaPriority = new(MaxParticleCountsSet);
        public static readonly List<ScarletParticle> ParticleNonPrePriority = new(MaxParticleCountsSet);
        public static readonly List<ScarletParticle> ParticleAddPriority = new(MaxParticleCountsSet);

        public int TotalDust;
        #region 加载卸载
        // 扔给统一的管理了
        public override void Load()
        {
            //On_Main.DrawDust += DrawParticles;
        }
        public override void Unload()
        {
            //On_Main.DrawDust -= DrawParticles;
        }
        #endregion
        /// <summary>
        /// 清除世界状态时调用（例如退出世界时）。
        /// </summary>
        public override void ClearWorld()
        {
            ParticleAlpha.Clear();
            ParticleNonPre.Clear();
            ParticleAdditive.Clear();
            ParticleAlphaPriority.Clear();
            ParticleNonPrePriority.Clear();
            ParticleAddPriority.Clear();
        }

        // 粒子更新
        public override void PostUpdateDusts()
        {
            UpdatePriorityParticles();
            UpdateParticles();
            TotalDust = ParticleAlpha.Count + ParticleNonPre.Count + ParticleAdditive.Count + ParticleAlphaPriority.Count + ParticleNonPrePriority.Count + ParticleAddPriority.Count;
        }
        // 绘制粒子
        public static void DrawScarletParticles(On_Main.orig_DrawDust orig, Main self)
        {
            // 调用源
            orig(self);
            //void DrawBatch(BlendState blendState, List<ScarletParticle> priorityList, List<ScarletParticle> normalList)
            //{
            //    if (priorityList.Count + normalList.Count == 0)
            //        return;
            //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //    foreach (var p in priorityList)
            //        p.Draw(Main.spriteBatch);

            //    foreach (var p in normalList)
            //        p.Draw(Main.spriteBatch);
            //    Main.spriteBatch.End();

            //}
            //DrawBatch(BlendState.AlphaBlend, ParticleAlphaPriority, ParticleAlpha);
            //DrawBatch(BlendState.Additive, ParticleAddPriority, ParticleAdditive);
            //DrawBatch(BlendState.NonPremultiplied, ParticleNonPrePriority, ParticleNonPre);
            #region 渲染粒子
            #region 渲染优先粒子
            if (ParticleAlphaPriority.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleAlphaPriority.Count; i++)
                {
                    ParticleAlphaPriority[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ParticleAddPriority.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleAddPriority.Count; i++)
                {
                    ParticleAddPriority[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ParticleNonPrePriority.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleNonPrePriority.Count; i++)
                {
                    ParticleNonPrePriority[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #region 渲染常规粒子
            if (ParticleAlpha.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleAlpha.Count; i++)
                {
                    ParticleAlpha[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ParticleAdditive.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleAdditive.Count; i++)
                {
                    ParticleAdditive[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ParticleNonPre.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ParticleNonPre.Count; i++)
                {
                    ParticleNonPre[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #endregion
        }
    }
}
