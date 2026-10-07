using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs
{
    public class InvisBoom : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        private int ApplyBuffID = -1;
        private int ApplyBuffFrame = 60;
        private int LifeTime = 40;
        private int LocalNPCHitCooldown = 40;
        private int Penetrate = -1;
        private NPC MountedTarget = null;
        private DamageClass DamageType = DamageClass.Generic;
        public void SetUpBoom(int buffID, int buffFrame)
        {
            ApplyBuffID = buffID;
            ApplyBuffFrame = buffFrame;
        }
        public void SetUpBoom(int buffID, int buffFrame, int lifeTime, int npcHitCooldown, int penetrate, DamageClass damageType, NPC mountedTarget = null)
        {
            ApplyBuffID = buffID;
            ApplyBuffFrame = buffFrame;
            LifeTime = lifeTime;
            LocalNPCHitCooldown = npcHitCooldown;
            Penetrate = penetrate;
            DamageType = damageType;
            MountedTarget = mountedTarget;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = LifeTime;
            Projectile.localNPCHitCooldown = LocalNPCHitCooldown;
            Projectile.penetrate = Penetrate;
            Projectile.DamageType = DamageType;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            if (MountedTarget.IsLegal())
                Projectile.Center = MountedTarget.Center;
            base.ProjAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ApplyBuffID != -1 && ApplyBuffFrame > 0)
            {
                target.AddBuff(ApplyBuffID, ApplyBuffFrame);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        //public override string Texture => HJScarletTexture.InvisAsset.Path;
        //public override void ExSD()
        //{
        //    Projectile.width = Projectile.height = 60;
        //    Projectile.tileCollide = false;
        //    Projectile.ownerHitCheck = true;
        //    Projectile.ignoreWater = true;
        //    Projectile.penetrate = -1;
        //    Projectile.SetupImmnuity(60);
        //    Projectile.timeLeft = 60;
        //}
        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    return base.Colliding(projHitbox, targetHitbox);
        //}
        public override bool? CanHitNPC(NPC target)
        {
            NPC tar = Projectile.HJScarlet().CurStoredTarget;
            if (tar.IsLegal() && tar.Equals(target))
                return false;
            return null;
        }
        //public override void AI()
        //{
        //    base.AI();
        //}
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
