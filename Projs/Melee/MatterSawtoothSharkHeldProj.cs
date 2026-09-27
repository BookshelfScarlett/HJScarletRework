using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class MatterSawtoothSharkHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<MatterSawtoothShark>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.IsHeldProj[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(0);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }
        public override void ProjAI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 60;

            // Animation code could go here if the projectile was animated. 

            // Plays a sound every 20 ticks. In aiStyle 20, soundDelay is set to 30 ticks.
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.soundDelay = 20;
            }
            if (!Owner.channel)
                Projectile.Kill();
            else if (Projectile.IsMe())
                Projectile.velocity = Owner.Center.ToMouseVector2();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

            if (Projectile.velocity.X > 0f)
            {
                player.ChangeDir(1);
            }
            else if (Projectile.velocity.X < 0f)
            {
                player.ChangeDir(-1);
            }

            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction); // Change the player's direction based on the projectile's own
            player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
            player.SetDummyItemTime(2); // Make sure the player's item time does not change while the projectile is out
            Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            // Gives the drill a slight jiggle
            Projectile.velocity.X *= 1f + Main.rand.Next(-3, 4) * 0.01f;

            // Spawning dust
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity * Main.rand.Next(6, 10) * 0.15f, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, 0f, 0f, 80, Color.White, 1f);
                dust.position.X -= 4f;
                dust.noGravity = true;
                dust.velocity.X *= 0.5f;
                dust.velocity.Y = -Main.rand.Next(3, 8) * 0.1f;
            }
            base.ProjAI();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 rotPoint = Projectile.spriteDirection < 0 ? new Vector2(tex.Width, tex.Height / 2f) : new Vector2(0, tex.Height / 2f);
            float rot = Projectile.spriteDirection < 0 ? Projectile.rotation - Pi : Projectile.rotation;
            SpriteEffects se = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.FastDraw(tex, pos.ToRandCirclePos(1.5f), Color.White, rot, rotPoint, Projectile.scale, se);
            return false;
        }
    }
}
