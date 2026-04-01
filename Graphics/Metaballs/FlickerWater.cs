using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.MetaballSystem;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Graphics.Metaballs
{
    public class FlickerWater: BaseMetaball
    {
        public class FlickerWaterParticle(Vector2 center, Vector2 vel, Vector2 scale, float rotation, int lifeTime, Texture2D shapeTex)
        {
            public Vector2 Scale = scale;
            public Vector2 Velocity = vel;
            public Vector2 Center = center;
            public Texture2D ShapeTex = shapeTex;
            public int LifeTime = lifeTime;
            public float Rotation = rotation;
            public int Timer = 0;
            public void Update()
            {
                Scale *= 0.96f;
                Center += Velocity;
                Velocity *= 0.9f;
                Timer++;
            }
        }
        public override Color EdgeColor => Color.Lerp(Color.DeepSkyBlue, Color.White, 0.64f);
        public static List<FlickerWaterParticle> ParticleList = [];
        public override Texture2D BackgroundTexture => HJScarletTexture.Metaball_FlickerWater.Value;
        public static void SpawnParticle(Vector2 pos, Vector2 vel, Vector2 scale, float rotation, int lifeTime, Texture2D shapeTex) => ParticleList.Add(new(pos, vel, scale, rotation, lifeTime, shapeTex));
        public override bool Active()
        {
            return ParticleList.Count != 0;
        }
        public override void Update()
        {
            for (int i = 0; i < ParticleList.Count; i++)
                ParticleList[i].Update();
            ParticleList.RemoveAll(particleList =>
            {
                return particleList.Timer / (float)particleList.LifeTime >= 1f;
            });
        }
        public override bool PreDrawRT2D()
        {
            return base.PreDrawRT2D();
        }
        public override void PrepareRenderTarget()
        {
            if (ParticleList.Count == 0)
                return;
            for (int i = 0; i < ParticleList.Count; i++)
            {

                Main.spriteBatch.Draw(ParticleList[i].ShapeTex, ParticleList[i].Center - Main.screenPosition, null, Color.White, ParticleList[i].Rotation, ParticleList[i].ShapeTex.ToOrigin(), ParticleList[i].Scale, 0, 0);
            }
        }
    }
}
