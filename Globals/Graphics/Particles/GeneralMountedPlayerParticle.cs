using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class GeneralMountedPlayerParticle : BaseParticle
    {
        public int SetBlendStateID = BlendStateID.Additive;
        public int MountedPlayer = -1;
        public float TargetScale = 0;
        public float TargetOpac = 0;
        public Texture2D UseTex = HJScarletTexture.Particle_RingShiny.Value;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override int UseBlendStateID => SetBlendStateID;
        public GeneralMountedPlayerParticle(Texture2D useTex, Color color, float rot, float scale, float opac, int lifeTime, int playerIndex, int blendStateID)
        {
            UseTex = useTex;
            DrawColor = color;
            Scale = 0;
            Opacity = 0;
            TargetScale = scale;
            TargetOpac = opac;
            Rotation = rot;
            MountedPlayer = playerIndex;
            SetBlendStateID = blendStateID;
            Lifetime = lifeTime;
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public override void Update()
        {

            if (MountedPlayer != -1)
            {
                Player p = Main.player[MountedPlayer];
                if (p.dead)
                {
                    Kill();
                    return;
                }

                if (LifetimeRatio > .9f)
                {
                    //1->0
                    float rat = (1 - LifetimeRatio) / .1f;
                    Opacity = Lerp(0, Opacity, rat);
                }
                else
                {
                    Opacity = Lerp(Opacity, TargetOpac + .1f, .25f);
                    Scale = Lerp(Scale, TargetScale + .01f, .2f);

                }
            }
            else
            {
                Kill();
            }
            base.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.player[MountedPlayer];
            if (!p.dead)
            {
                Position = p.MountedCenter;
                Position.Y += p.gfxOffY;
            }

            Texture2D tex = UseTex;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, tex.Size() / 2f, Scale, 0, 0);
            base.Draw(spriteBatch);
        }
    }
}
