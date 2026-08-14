using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    /// <summary>
    /// 猩红重镰击杀NPC的专属特效
    /// </summary>
    public class ScytheBlood : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Alpha;
        public float BeginScale;
        public SpriteEffects se = SpriteEffects.None;
        public bool UseBoomLight = false;
        public bool UseFadeIn = true;
        public int BlendstateID;
        public int Frame = 0;
        public ScytheBlood(Vector2 position, float scale)
        {
            Position = position;
            DrawColor = Color.White;
            Lifetime = 30;
            Opacity = 1;
            Scale = 0;
            BeginScale = scale;
        }
        public override void OnSpawn()
        {
            if (Main.rand.NextBool())
                se = SpriteEffects.FlipHorizontally;
            Frame = Main.rand.Next(0, 1);
            ScarletSound(HJScarletSounds.Tlipoca_NpcKillSound, Position, 1f, 0, pitch: 0, pitchVariance: .1f);
        }
        public override void Update()
        {
            Scale = Lerp(0f, BeginScale * 1.1f, EaseOutCubic(LifetimeRatio));
            if (LifetimeRatio > .65f)
            {
                Opacity = Lerp(Opacity, 0f, .2f);
            }
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {

            Texture2D texture = HJScarletTexture.Particle_ScytheBlood.Value;
            Rectangle frame = texture.Frame(1, 3, 0, Frame);
            Vector2 orig = frame.Size() / 2f;
            Color c = Color.Lerp(DrawColor, Color.Transparent, 1 - Opacity);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, c, Rotation, orig, Scale, se, 0f);
            frame = texture.Frame(1, 3, 0, 1);
            orig = frame.Size() / 2f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, c, Rotation, orig, Scale*1.14f, se, 0f);
        }
    }
}
