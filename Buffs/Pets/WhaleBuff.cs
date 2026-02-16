using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Pets;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs.Pets
{
    public class WhaleBuff : PetsBuff   
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref player.HJScarlet().WhalePet, ProjectileType<WhaleProj>());
        }
    }
    public abstract class PetsBuff : ModBuff
    {
        internal static string BuffPath = "HJScarletRework/Assets/Texture/Pets/Pet_";
        public override string Texture => BuffPath + GetType().Name;
        public virtual bool IsLightPet => false; 
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
            if (IsLightPet)
                Main.lightPet[Type] = true;
        }
    }
}
