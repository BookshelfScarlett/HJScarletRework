using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI.Chat;

namespace HJScarletRework.Projs.General
{
    public class GeneralStringProj: HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public string TextValue = string.Empty;
        public override void ExSD()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
        }
        public override void ProjAI()
        {
            Projectile.Center = Owner.MountedCenter - Vector2.UnitY * 45f;
            Projectile.position.Y += Owner.gfxOffY;
            if (Projectile.timeLeft < 10)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 1 - Projectile.timeLeft / 10f);
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, .2f);
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
            string numStr = TextValue;
            DynamicSpriteFont font = HJScarletTexture.Font_MGR.Value;
            Vector2 scale = new Vector2(.36f);
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
