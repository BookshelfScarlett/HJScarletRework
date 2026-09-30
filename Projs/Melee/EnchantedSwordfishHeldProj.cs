using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class EnchantedSwordfishHeldProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 100f;

        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.IsHeldProj[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.SetupImmnuity(10);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(2), RandVelTwoPi(18f), RandLerpColor(Color.Red, Color.Blue), 40, 1, 0.64f, .2f);
            ECSParticle.ShinyCrossStarSmall(target.Center, RandVelTwoPi(8f), RandLerpColor(Color.Red,Color.Blue), 40, 1, 0.6f, Main.rand.NextFloat(-.05f, .05f));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            int offset = 65;
            rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(tex.Width, tex.Height) - new Vector2(offset) : new Vector2(0, tex.Height) - new Vector2(-offset, offset);
            Texture2D glowTex = HJScarletTexture.Particle_SharpTear;

            int duration = Owner.itemAnimationMax; // Define the duration the projectile will exist in frames
            float halfDuration = duration * 0.5f;
            float progress;
            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 1.5f * progress, Color.White.ToAddColor(), drawRotation, rotationPoint, Projectile.scale, flipSprite);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;
            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            Owner.ChangeDir(Projectile.spriteDirection);


            // Avoid spawning dusts on dedicated servers
            if (!Main.dedServ)
            {
                // These dusts are added later, for the 'ExampleMod' effect
                if (Main.rand.NextBool(3))
                {
                    ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(16), Projectile.velocity, RandLerpColor(Color.Blue, Color.Red), 45, 1, RandRotTwoPi, 0.2f, 1);
                }

                if (Main.rand.NextBool(4))
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), Projectile.velocity, RandLerpColor(Color.Red, Color.Blue), 45, 1, 0.4f, .2f);
                }
            }
            return false; // Don't execute vanilla AI.
        }

    }
}
