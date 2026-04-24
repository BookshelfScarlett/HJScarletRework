using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class KiraStar: BaseParticle
    {
        public bool UseRot = false;
        public Color InitColor;
        public float SpinSpeed = 0;
        private float BeginScale;
        private int TheBlendStateID;
        private bool FadeIn;
        private float MaxOpacity;
        private bool UseAlt;
        private float GlowCenterMult = -1f;
        public override int UseBlendStateID => TheBlendStateID;
        public KiraStar(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float spinSpeed = 0f, bool fadeIn = false, bool useAlt=false,int? blendstateID = null)
        {
            Position = position;
            Velocity = velocity;
            InitColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = BeginScale = scale;
            SpinSpeed = spinSpeed;
            TheBlendStateID = blendstateID ?? BlendStateID.Additive;
            FadeIn = fadeIn;
            UseAlt = useAlt;
        }
        /// <summary>
        /// glowCenterMult传参<1时不会绘制中心高光
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="color"></param>
        /// <param name="lifetime"></param>
        /// <param name="scale"></param>
        /// <param name="useAlt"></param>
        /// <param name="glowCenterMult"></param>
        /// <param name="opacity"></param>
        /// <param name="blendstateID"></param>
        public KiraStar(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, bool useAlt, float glowCenterMult = 0.85f, float opacity = 1f, int? blendstateID = null)
        {
            Position = position;
            Velocity = velocity;
            InitColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = BeginScale = scale;
            SpinSpeed = 0;
            Rotation = 0;
            TheBlendStateID = blendstateID ?? BlendStateID.Additive;
            GlowCenterMult = glowCenterMult;
            FadeIn = false;
            UseAlt = useAlt;
        }

        public override void OnSpawn()
        {
            MaxOpacity = Opacity;
            if (FadeIn)
                Opacity = 0;
        }

        public override void Update()
        {
            if (FadeIn)
            {
                Opacity = Lerp(Opacity, MaxOpacity, 0.2f);
            }
            else
                Opacity = MaxOpacity;
            DrawColor = InitColor;
            Velocity *= 0.96f;
            Rotation += SpinSpeed;
            if (LifetimeRatio > 0.8f)
                Scale = Lerp(Scale, 0, 0.2f);
            //太小的情况下直接处死粒子就行了
            if (Scale < BeginScale * 0.15f)
                Time = Lifetime;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D star = UseAlt ? HJScarletTexture.Particle_KiraStarGlow.Value : HJScarletTexture.Particle_KiraStar.Value;
            Vector2 drawPos = Position - Main.screenPosition;

            spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity, Rotation, star.Size()/2, Scale, SpriteEffects.None, 0);
            if(GlowCenterMult > 0)
            spriteBatch.Draw(star, drawPos, null, Color.White * Opacity, Rotation, star.Size()/2, Scale * GlowCenterMult, SpriteEffects.None, 0);
        }
    }
}
