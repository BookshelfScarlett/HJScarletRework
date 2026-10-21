using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class ScarletPetProjClass : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projs.Friendly.Pets";
        public static string TexturePath = "HJScarletRework/Assets/Texture/Pets/Pet_";
        public override string Texture => TexturePath + GetType().Name;
        public Player Owner => Main.player[Projectile.owner];
        public virtual int TotalFrames => 0;
        public virtual int ExtraUpdates => 0;
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
        }
        public override void SetDefaults()
        {
            HJScarletMethods.PetDefaultsCommon(Projectile, 20, 20);
            ExSD();
        }
        public virtual void ExSD()
        {
        }
        public override void AI()
        {
            SimplePetFunction();
            PetAI();
        }

        public virtual void PetAI()
        {
        }

        /// <summary>
        /// 用于处理宠物的一些正确与否的存活态，如：主人存续等
        /// </summary>
        public virtual void SimplePetFunction()
        {

        }

        /// <summary>
        /// 序列帧处理，这里推荐直接填入至SimplePetFunction钩子内。
        /// </summary>
        /// <param name="Speed"></param>
        public void SimplePetAnimation(float Speed)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > Speed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

            }
            if (Projectile.frame >= TotalFrames)
                Projectile.frame = 0;
        }
    }
}
