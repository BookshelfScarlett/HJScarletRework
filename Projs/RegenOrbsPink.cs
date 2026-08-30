using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.GJKCollision;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.GameContent.Generation;

namespace HJScarletRework.Projs
{
    public class RegenOrbsPink : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        private enum Styles
        {
            Slowdown,
            Return
        }
        private Styles AttackType
        {
            get => (Styles)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(40, 2);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(1);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Main.MouseWorld;
            Projectile.timeLeft = 2;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //创建一个局部顶点
            Vector2[] localVertices = [new Vector2(30), new Vector2(30), new Vector2(0, -30)];
            //转化为需要的实际坐标值
            Vector2[] world = new Vector2[localVertices.Length];
            Matrix transform = Matrix.CreateRotationZ(Projectile.rotation) * Matrix.CreateScale(Projectile.scale);
            for (int i = 0; i < localVertices.Length; i++)
                world[i] = Vector2.Transform(localVertices[i], transform) + Projectile.Center;
            //最后，构建两个形状：
            var shapeA = new ConvexPolygon { Vertices = world };
            //构建目标形状：
            Vector2[] targetVert = [targetHitbox.TopLeft(), targetHitbox.TopRight(), targetHitbox.BottomRight(), targetHitbox.BottomLeft()];
            var shapeB = new ConvexPolygon { Vertices = targetVert };
            if (ConvexPolygon.GJKIntersect(shapeA, shapeB))
            {
                return true;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SB.FastDrawCube(Projectile.Center);
            return false;
        }
    }
}
