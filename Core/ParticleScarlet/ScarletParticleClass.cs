using HJScarletRework.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.ParticleScarlet
{
    public abstract class ScarletParticle
    {
        #region 基础属性
        public bool Important = false;
        /// <summary>
        /// 使用材质
        /// </summary>
        public virtual string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

        /// <summary>
        /// 该粒子存在了多少帧，一般不需要手动修改这个值
        /// </summary>
        public int Time = 0;

        /// <summary>
        /// 粒子的存在时间上限
        /// </summary>
        public int Lifetime = 0;

        /// <summary>
        /// 位置与向量
        /// </summary>
        public Vector2 Position;
        public Vector2 Velocity;

        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale = 1f;

        /// <summary>
        /// 粒子的透明度
        /// </summary>
        public float Opacity = 1f;

        /// <summary>
        /// 生命周期的进度，介于0到1之间。
        /// 0表示粒子刚生成，1表示粒子消失。
        /// </summary>
        public float LifetimeRatio => Time / (float)Lifetime;

        /// <summary>
        /// 渲染的混合模式，默认为<see cref="BlendState.AlphaBlend"/>.
        /// 暂时用不上
        /// </summary>
        public virtual BlendState TheBlendState => BlendState.AlphaBlend;
        public bool Priority = false;
        #endregion
        public static readonly Dictionary<Type, Stack<ScarletParticle>> Pools = new();
        public static T Get<T>() where T : ScarletParticle, new()
        {
            var type = typeof(T);
            if (Pools.TryGetValue(type, out var stack) && stack.Count > 0)
            {
                T p = (T)stack.Pop();
                p.Reset();
                return p;
            }
            T p2 = new T();
            p2.Reset();
            return p2;
        }
        public void ReturnToPool()
        {
            //Reset();
            var type = GetType();
            if (!Pools.TryGetValue(type, out var stack))
                Pools[type] = stack = new Stack<ScarletParticle>();
            stack.Push(this);
        }
        private void Reset()
        {
            if (PreReset())
                return;
            ResetDefault();
            PostReset();
        }
        public virtual bool PreReset() => false;
        public void ResetDefault()
        {
            Important = false;
            Time = 0;
            Lifetime = 0;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Origin = Vector2.Zero;
            DrawColor = Color.White;
            Rotation = 0;
            Scale = 1;
            Opacity = 1;
            Priority = false;
        }
        public static T Spawn<T>(Action<T> init = null) where T : ScarletParticle, new()
        {
            T p = Get<T>();
            init?.Invoke(p);
            p.Spawn();
            return p;
        }
        public virtual void PostReset() { }
        public virtual void OnSpawn() { }

        /// <summary>
        /// 粒子的更新，默认不做任何操作
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 立刻清除粒子
        /// </summary>
        public void Kill()
        {
            Time = Lifetime;
        }

        public virtual void OnKill() { }

        /// <summary>
        /// 覆写这个就可以自定义绘制
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// 在世界内生成粒子
        /// </summary>
        /// <returns></returns>
        public ScarletParticle Spawn(bool priority = false)
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            //if (TheBlendState != BlendState.AlphaBlend)
            //    return this;
            if (TheBlendState == BlendState.AlphaBlend)
            {
                if (priority)
                {
                    if (!Important && ScarletParticleManager.ParticleAlphaPriority.Count > ParticleUtilities.MaxParticles)
                    {
                        var old = ScarletParticleManager.ParticleAlphaPriority[0];
                        old.ReturnToPool();
                        ScarletParticleManager.ParticleAlphaPriority.RemoveAt(0);
                    }
                    ScarletParticleManager.ParticleAlphaPriority.Add(this);
                }
                else
                {

                    if (!Important && ScarletParticleManager.ParticleAlpha.Count > ParticleUtilities.MaxParticles)
                    {
                        var old = ScarletParticleManager.ParticleAlpha[0];
                        old.ReturnToPool();
                        ScarletParticleManager.ParticleAlpha.RemoveAt(0);
                    }

                    ScarletParticleManager.ParticleAlpha.Add(this);
                }
            }
            if (TheBlendState == BlendState.Additive)
            {
                if (priority)
                {
                    if (!Important && ScarletParticleManager.ParticleAddPriority.Count > ParticleUtilities.MaxParticles)
                    {
                        var old = ScarletParticleManager.ParticleAddPriority[0];
                        old.ReturnToPool();
                        ScarletParticleManager.ParticleAddPriority.RemoveAt(0);
                    }
                    ScarletParticleManager.ParticleAddPriority.Add(this);
                }
                else
                {
                    if (!Important && ScarletParticleManager.ParticleAdditive.Count > ParticleUtilities.MaxParticles)
                    {
                        var old = ScarletParticleManager.ParticleAdditive[0];
                        old.ReturnToPool();

                        ScarletParticleManager.ParticleAdditive.RemoveAt(0);
                    }
                    ScarletParticleManager.ParticleAdditive.Add(this);
                }
                
            }
            if (TheBlendState == BlendState.NonPremultiplied)
                {
                    if (priority)
                    {
                        if (!Important && ScarletParticleManager.ParticleNonPrePriority.Count > ParticleUtilities.MaxParticles)
                        {
                            var old = ScarletParticleManager.ParticleNonPrePriority[0];
                            old.ReturnToPool();

                            ScarletParticleManager.ParticleNonPrePriority.RemoveAt(0);
                        }
                        ScarletParticleManager.ParticleNonPrePriority.Add(this);
                    }
                    else
                    {
                        if (!Important && ScarletParticleManager.ParticleNonPre.Count > ParticleUtilities.MaxParticles)
                        {
                            var old = ScarletParticleManager.ParticleNonPre[0];
                            old.ReturnToPool();

                            ScarletParticleManager.ParticleNonPre.RemoveAt(0);
                        }
                        ScarletParticleManager.ParticleNonPre.Add(this);
                    }

                }

            OnSpawn();
            return this;
        }
        /// <summary>
        ///// 在世界内生成粒子
        ///// </summary>
        ///// <returns></returns>
        //public ScarletParticle SpawnAdditive(bool priority = false)
        //{
        //    if (Main.netMode == NetmodeID.Server)
        //        return this;
        //    //if (TheBlendState != BlendState.Additive)
        //    //    return this;
        //    OnSpawn();
        //    return this;
        //}
        ///// <summary>
        ///// 在世界内生成粒子
        ///// </summary>
        ///// <returns></returns>
        //public ScarletParticle SpawnNonPre(bool priority = false)
        //{
        //    if (Main.netMode == NetmodeID.Server)
        //        return this;
        //    //if (TheBlendState != BlendState.NonPremultiplied)
        //    //    return this;
        //    OnSpawn();
        //    return this;
        //}
    }
}
