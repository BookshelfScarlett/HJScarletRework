using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.General
{
    public class PreciousTargetCross : HJScarletProj
    {
        public override string Texture => GetInstance<PreciousTarget>().Texture;
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.IsHeldProj[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(0);
            //大小需要一定的容错
            Projectile.width = Projectile.height = 100;

        }
        public override void ProjAI()
        {
            if (!Projectile.IsMe())
                return;
            if (Projectile.localAI[0] == 0)
            {
                ScarletSound(HJScarletSounds.Lightning_Quick, Projectile.Center);
                for (int i = 0; i < 56; i++)
                    ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(16), RandVelTwoPi(-2, 9), RandLerpColor(Color.DarkSeaGreen, Color.LightSeaGreen), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .61f, rotSpeed: Main.rand.NextFloat(-.1f, .1f));
                for (int i = 0; i < 6; i++)
                    ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(16), RandVelTwoPi(.0f), RandLerpColor(Color.DarkSeaGreen, Color.LightSeaGreen), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .31f, 2);
                Projectile.localAI[0] = 1;
            }

            Projectile.scale = 1;
            if (Owner.HJScarlet().preciousTargetLevel > 0 && Owner.HeldItem.IsLegal() && Owner.HeldItem.DamageType.CountsAsClass<RangedDamageClass>())
                Projectile.timeLeft = 2;
            Projectile.Center = Vector2.Lerp(Projectile.Center, Main.MouseWorld, .95f);
            if (Owner.HeldItem.IsLegal() && Owner.HeldItem.DamageType.CountsAsClass<RangedDamageClass>())
            {
                foreach (var activeTarget in Main.ActiveNPCs)
                {
                    if (!activeTarget.IsLegal())
                        continue;
                    if (activeTarget.friendly)
                        continue;
                    if (activeTarget.lifeMax <= 5)
                        continue;
                    if (!Projectile.Hitbox.Intersects(activeTarget.Hitbox))
                        continue;
                    activeTarget.HJScarlet().isUnderPreciousTargetCross = 2;
                    TargetParticle(activeTarget);
                }
            }
        }
        public void TargetParticle(NPC tar)
        {
            if (Main.rand.NextBool())
                ECSParticle.GlowSquare(tar.ToRandRec(), -Vector2.UnitY, RandLerpColor(Color.DarkSeaGreen, Color.LightSeaGreen), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .61f, rotSpeed: Main.rand.NextFloat(-.1f, .1f));
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;

            return false;
        }
    }
}
