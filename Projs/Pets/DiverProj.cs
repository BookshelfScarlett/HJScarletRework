using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Classes;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Pets
{
    public class DiverProj : ScarletPetProjClass
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 18)
                .WithOffset(-10, -20f)
                .WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }
        public override void ExSD()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            AIType = ProjectileID.ZephyrFish;
        }
        public override bool PreAI()
        {
            return true;
        }
        public override void AI()
        {
            if (!Owner.dead && Owner.HasBuff(BuffType<DiverBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}
