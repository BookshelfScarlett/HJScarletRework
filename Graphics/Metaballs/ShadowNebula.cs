using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.MetaballSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Graphics.Metaballs
{
    public class ShadowNebula : BaseMetaBall
    {
        public class ShadowNebulaParticle(Vector2 center, Vector2 vel, float scale)
        {
            public float Scale = scale;
            public Vector2 Velocity = vel;
            public Vector2 Center = center;
            public void Update()
            {
                Center += Velocity;
                Velocity *= 0.9f;
                Scale *= 0.96f;
            }
        }
        public override Color EdgeColor => base.EdgeColor;
        public static List<ShadowNebulaParticle> ParticleList = [];
        public override Texture2D BgTexture => HJScarletTexture.Metaball_ShadowNebula.Value;
        public static void SpawnParticle(Vector2 pos, Vector2 vel, float scale) => ParticleList.Add(new(pos, vel, scale));
        public override bool Active()
        {
            return ParticleList.Count != 0;
        }
        public override void Update()
        {
            for(int i = 0; i < ParticleList.Count; i++)
                ParticleList[i].Update();
            ParticleList.RemoveAll(particleList =>
            {
                return particleList.Scale < 0.01f;
            });
        }
        public override void PrepareRenderTarget()
        {
            if (ParticleList.Count == 0)
                return;
            for(int i = 0;i < ParticleList.Count;i++)
            {
                Main.spriteBatch.Draw(HJScarletTexture.Texture_WhiteCircle.Value, ParticleList[i].Center - Main.screenPosition, null, Color.White, 0, HJScarletTexture.Texture_WhiteCircle.Origin, ParticleList[i].Scale, 0, 0);
            }
        }
    } 
}
