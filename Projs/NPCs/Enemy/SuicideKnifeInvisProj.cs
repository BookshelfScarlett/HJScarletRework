using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.NPCs.Enemy
{
    public class SuicideKnifeInvisProj : HJScarletEnemyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
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
            modifiers.SetMaxDamage(150);
            if (Projectile.owner == target.whoAmI && target.IsHolding<SuicideKnife>())
                modifiers.FinalDamage *= 1;
            else
                modifiers.FinalDamage *= 1;
            Owner.AddImmuneTime(ImmunityCooldownID.General, 180);
            //Main.NewText(Owner.immuneTime);
            //Owner.GetImmnue(ImmunityCooldownID.General, 180,true);
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
