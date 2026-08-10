using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FleshtumorHungerStick : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => $"Terraria/Images/NPC_{NPCID.TheHungry}";
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
            Main.projFrames[Type] = 6;
        }
        public override Vector2 TileHitbox => new Vector2(12);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.SetupImmnuity(-1);
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {

            Projectile.AddFrames(4, 6);
            float maxDistance = 18f; // This also sets the maximun speed the projectile can reach while following the cursor.
            Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
            float distanceToCursor = vectorToCursor.Length();

            // Here we can see that the speed of the projectile depends on the distance to the cursor.
            if (distanceToCursor > maxDistance)
            {
                distanceToCursor = maxDistance / distanceToCursor;
                vectorToCursor *= distanceToCursor;
            }
            Projectile.velocity = vectorToCursor;
            //Projectile.MinionAntiClump(.5f);
            Projectile.timeLeft = 2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chains = TextureAssets.Chain12.Value;
            Vector2 pCenter = Owner.MountedCenter;
            Vector2 projCenter = Projectile.Center;
            Vector2 directionToPlayer = pCenter - projCenter;
            float chainRot = directionToPlayer.ToRotation() - PiOver2;
            float distanceToPlayer = directionToPlayer.Length();
            while (distanceToPlayer > 40f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer;
                directionToPlayer *= chains.Height;
                projCenter += directionToPlayer;
                directionToPlayer = pCenter - projCenter;
                distanceToPlayer = directionToPlayer.Length();
                Color c = Color.White;
                SB.Draw(chains, projCenter - Main.screenPosition, chains.Bounds, c, chainRot, chains.Size() / 2f, 1, 0, 0);
            }
            Texture2D head = TextureAssets.Npc[NPCID.TheHungryII].Value;
            Rectangle rec = head.Frame(1, 6, 0, 2);
            Vector2 ori = rec.Size() / 2f;
            SB.Draw(head, Projectile.Center - Main.screenPosition, rec, Color.White, chainRot + PiOver2, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
