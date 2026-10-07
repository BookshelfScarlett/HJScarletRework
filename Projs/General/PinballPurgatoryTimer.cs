using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI.Chat;

namespace HJScarletRework.Projs.General
{
    public class PinballPurgatoryTimer : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
        }
        public override void ProjAI()
        {
            Projectile.Center = Owner.MountedCenter - Vector2.UnitY * 30f;
            Projectile.position.Y += Owner.gfxOffY;
            if (Projectile.timeLeft < 10)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 1 - Projectile.timeLeft / 10f);
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, .2f);

            }
            if (Owner.HJScarlet().mayaPumperParty)
            {

                Color c = RandLerpColor(Color.DarkOrange, Color.Orange);
                switch (Owner.HJScarlet().mayaPumperDashType)
                {
                    case 2:
                        c = RandLerpColor(Color.SkyBlue, Color.RoyalBlue);
                        break;
                    case 4:
                        c = RandLerpColor(Color.LightPink, Color.HotPink);
                        break;
                    case 1:
                        c = RandLerpColor(Color.WhiteSmoke, Color.White);
                        break;
                    default:
                        break;
                }
                ECSParticle.SmokeParticle(Owner.Center.ToRandCirclePos(5f), RandVelTwoPi(1f, 2f), c, 40, RandRotTwoPi, .21f, 0.87f * Main.rand.NextFloat(0.8f, 1.1f), false, BlendState.AlphaBlend);
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Owner.IsOwnerSide())
                return false;
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            DrawNumberWithEffect(SB, Projectile.Center, Owner.HJScarlet().mayaPumperDashTime, Projectile.Opacity);
            return false;
        }
        private void DrawNumberWithEffect(SpriteBatch sb, Vector2 basePos, int number, float opa)
        {
            string numStr = number.ToString();
            DynamicSpriteFont font = HJScarletTexture.Font_MGR.Value;
            Vector2 scale = new Vector2(.6f);
            Vector2 size = ChatManager.GetStringSize(font, numStr, scale);
            Vector2 ori = font.MeasureString(numStr);
            Vector2 textPos = basePos - new Vector2(0, 0) - Main.screenPosition;

            Color shadowColor1 = Color.Lerp(Color.Red, Color.Black, 0.95f) * (opa * 0.248f);

            Color shadowColor2 = Color.Black * opa;
            Color mainColor = Color.White * opa;
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = (TwoPi * i / 8f).ToRotationVector2() * 1.2f;

                //第一层阴影
                Vector2 pos1 = textPos + offset + new Vector2(3.5f, 3.5f);
                ChatManager.DrawColorCodedString(sb, font, numStr, pos1, shadowColor1, 0f, ori / 2f, scale);

                //第二层阴影
                Vector2 pos2 = textPos + offset;
                ChatManager.DrawColorCodedString(sb, font, numStr, pos2, shadowColor2, 0f, ori / 2f, scale);
            }
            //中心白色文字
            ChatManager.DrawColorCodedString(sb, font, numStr, textPos, mainColor, 0f, ori / 2f, scale);
        }

    }
}
