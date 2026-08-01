using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    /// <summary>
    /// 对的没错，这个是专门给如是我闻做的“粒子”
    /// </summary>
    public class RuShiWoWenFlower:BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Alpha;
        public int OwnerIndex = -1;
        public float LerpTime = 0;
        public RuShiWoWenFlower(int owner, Vector2 targetPos)
        {
            Position = targetPos;
            OwnerIndex = owner;
            Important = true;
        }
        public override void Update()
        {
            Lifetime = 100;
            Projectile owner = Main.projectile[OwnerIndex];
            Velocity *= 0;
            if (owner is not null && owner.active)
            {
                LerpTime = Lerp(LerpTime, 1.01f, .1f);
                Time = 10;
            }
            else
            {
                LerpTime = Lerp(LerpTime, 0, .1f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Projectile owner = Main.projectile[OwnerIndex];
            Vector2 newVec = new Vector2(owner.ai[1], owner.ai[2]);
            Position = Vector2.Lerp(Position, newVec, 1f);
            Texture2D tex = HJScarletTexture.Texture_RuShiWoWenFlower.Value;
            Vector2 pos = Position - Main.screenPosition;
            for(int i =0;i<8;i++)
            spriteBatch.Draw(tex, pos + (TwoPi / 8f * i).ToRotationVector2() * 1.25f * LerpTime, null, Color.White.ToAddColor(), 0, tex.Size() / 2, 0.8f* LerpTime, 0, 0);
            spriteBatch.Draw(tex, pos, null, Color.White*LerpTime, 0, tex.Size() / 2, 0.8f, 0, 0);
        }
    }
}
