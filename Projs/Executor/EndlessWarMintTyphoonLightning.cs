using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Lightning;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarMintTyphoonLightning : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public Vector2 BeginPos = Vector2.Zero;
        public Vector2 EndPos = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(10);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, EndPos, 48f, ref _);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnFirstFrame()
        {
            Vector2 NowBeginPos = new Vector2(Main.rand.NextFloat(-200, 200), -700);
            Vector2 beginPos = Projectile.Center + NowBeginPos;
            Vector2 fireVel = beginPos.GetNormalVector2(Projectile.Center);
            Vector2 endPos = beginPos + fireVel * 1800;

            Color color = RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue);
            LightningSetting setting = new LightningSetting(beginPos, endPos, color, 160, 575, 20, 4, 0f, 1, 100, 0.7f, 30);
            //LightningSetting setting = new LightningSetting(beginPos, endPos, Color.RoyalBlue,
            //        strength: 30,
            //        width: 50,
            //        lifetime: 60,
            //        generationsStep: 7,
            //        branchChance: 0.4f,
            //        maxBranchGenerations: 3,
            //        distanceProtect: 100,
            //        strengthDecay: 0.6f,
            //        maxBranchAllowedDistance: 1000);
            LightningBuilder.SpawnLightning(setting);

            BeginPos = beginPos;
            EndPos = endPos;
        }
    }
}
