using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using Terraria;

namespace HJScarletRework.Projs.NPCs.Enemy
{
    public class SuicideKnifeInvisProj : HJScarletEnemyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int SuicideDamage
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public override void ExSD()
        {
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.Center = Owner.Center;
            base.AI();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SetMaxDamage(SuicideDamage);
            if (Projectile.owner == target.whoAmI && target.IsHolding<SuicideKnife>())
                modifiers.FinalDamage *= 1;
            else
                modifiers.FinalDamage *= 1;
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
