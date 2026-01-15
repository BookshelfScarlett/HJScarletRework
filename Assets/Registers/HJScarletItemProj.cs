using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public class Tex2DWithPath
    {
        public Asset<Texture2D> Texture { get; }
        public string Path { get; }
        public Tex2DWithPath(Asset<Texture2D> texture, string path)
        {
            Path = path;
            Texture = texture;
        }
        public Tex2DWithPath(string path)
        {
            Path = path;
            Texture = Request<Texture2D>($"{Path}");
        }
        public Texture2D Value => Texture.Value;
        public int Height => Texture.Height();
        public int Width => Texture.Width();
        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }
        public Vector2 Origin
        {
            get
            {
                return Size / 2;
            }
        }
    }
    internal class HJScarletBuffIcon : ModSystem
    {
        internal static string BuffPath = "HJScarletRework/Assets/Texture/Buffs";
        internal static Tex2DWithPath Buff_RewardsofWarrior { get; private set; }
        public override void Load()
        {
            Buff_RewardsofWarrior = new Tex2DWithPath($"{BuffPath}/{nameof(Buff_RewardsofWarrior)}");
        }
        public override void Unload()
        {
            Buff_RewardsofWarrior = null;
        }
    }
    internal class HJScarletItemProj : ModSystem
    {
        internal static string ItemPath = "HJScarletRework/Assets/Texture/Items";
        internal static string ProjPath = "HJScarletRework/Assets/Texture/Projs";
        internal static string Path_Pets= $"HJScarletRework/Assets/Texture/Pets";
        internal static string WeaponPath = $"{ItemPath}/Weapons";
        internal static string AccessoriesPath = $"{ItemPath}/Accessories";
        internal static string MaterialsPath = $"{ItemPath}/Materials";
        internal static Tex2DWithPath Wreach { get; private set; }
        internal static Tex2DWithPath Item_SpearofDarknessThrown { get; private set; }
        internal static Tex2DWithPath Item_DialecticsThrown { get; private set; }
        internal static Tex2DWithPath Item_StormSpear { get; private set; }
        internal static Tex2DWithPath Item_Disaster { get; private set; }
        internal static Tex2DWithPath Item_FierySpear { get; private set; }
        
        internal static Tex2DWithPath Item_Judgement { get; private set; }
        internal static Tex2DWithPath Item_AetherfireSmasher { get; private set; }
        internal static Tex2DWithPath Item_DeathTolls { get; private set; }
        internal static Tex2DWithPath Item_BinaryStars { get; private set; }
        internal static Tex2DWithPath Item_Bamboomerang { get; private set; }

        internal static Tex2DWithPath Equip_StardustRune { get; private set; }
        internal static Tex2DWithPath Equip_StardustBless { get; private set; }
        internal static Tex2DWithPath Equip_PreciousAim { get; private set; }
        internal static Tex2DWithPath Equip_PreciousTarget { get; private set; }
        internal static Tex2DWithPath Equip_RewardofKingdom { get; private set; }
        internal static Tex2DWithPath Equip_RewardofWarrior { get; private set; }
        internal static Tex2DWithPath Equip_HeartoftheCrystal { get; private set; }
        internal static Tex2DWithPath Equip_HeartoftheMountain { get; private set; }
        internal static Tex2DWithPath Equip_CrimsonRune { get; private set; }

        internal static Tex2DWithPath Material_DisasterBar { get; private set; }

        internal static Tex2DWithPath Proj_SpearofDarkness { get; private set; }
        internal static Tex2DWithPath Proj_StormSpear { get; private set; }
        internal static Tex2DWithPath Proj_CandLanceFire { get; private set; }
        internal static Tex2DWithPath Proj_Dialectics { get; private set; }
        internal static Tex2DWithPath Proj_Disaster { get; private set; }
        internal static Tex2DWithPath Proj_FierySpear { get; private set; }

        internal static Tex2DWithPath Pet_SquidBuff { get; private set; }
        internal static Tex2DWithPath Pet_SquidItem { get; private set; }
        internal static Tex2DWithPath Pet_SquidProj { get; private set; }
        internal static Tex2DWithPath Pet_WatcherBuff { get; private set; }
        internal static Tex2DWithPath Pet_WatcherItem { get; private set; }
        internal static Tex2DWithPath Pet_WatcherProj { get; private set; }
        internal static Tex2DWithPath Pet_WhaleBuff { get; private set; }
        internal static Tex2DWithPath Pet_WhaleItem { get; private set; }
        internal static Tex2DWithPath Pet_WhaleProj { get; private set; }

        

        public override void Load()
        {
            Wreach = new Tex2DWithPath($"{ItemPath}/{nameof(Wreach)}");

            Item_SpearofDarknessThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_SpearofDarknessThrown)}");
            Item_DialecticsThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_DialecticsThrown)}");
            Item_StormSpear = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_StormSpear)}");
            Item_Disaster = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_Disaster)}");
            Item_Judgement = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_Judgement)}");
            Item_AetherfireSmasher = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_AetherfireSmasher)}");
            Item_DeathTolls = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_DeathTolls)}");
            Item_BinaryStars = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_BinaryStars)}");
            Item_Bamboomerang = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_Bamboomerang)}");
            Item_FierySpear = new Tex2DWithPath($"{WeaponPath}/{nameof(Item_FierySpear)}");

            Equip_StardustBless = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_StardustBless)}");    
            Equip_StardustRune = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_StardustRune)}");    
            Equip_RewardofKingdom = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_RewardofKingdom)}");
            Equip_RewardofWarrior = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_RewardofWarrior)}");
            Equip_PreciousAim = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_PreciousAim)}");
            Equip_PreciousTarget = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_PreciousTarget)}");
            Equip_HeartoftheCrystal = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_HeartoftheCrystal)}");
            Equip_HeartoftheMountain = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_HeartoftheMountain)}");
            Equip_CrimsonRune = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Equip_CrimsonRune)}");

            Material_DisasterBar = new Tex2DWithPath($"{MaterialsPath}/{nameof(Material_DisasterBar)}");

            Proj_SpearofDarkness = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_SpearofDarkness)}");
            Proj_StormSpear = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_StormSpear)}");
            Proj_CandLanceFire = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_CandLanceFire)}");
            Proj_Dialectics = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_Dialectics)}");
            Proj_Disaster = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_Disaster)}");
            Proj_FierySpear = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_FierySpear)}");

            Pet_SquidBuff = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_SquidBuff)}");
            Pet_SquidItem = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_SquidItem)}");
            Pet_SquidProj = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_SquidProj)}");
            Pet_WhaleBuff = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WhaleBuff)}");
            Pet_WhaleItem = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WhaleItem)}");
            Pet_WhaleProj = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WhaleProj)}");
            Pet_WatcherBuff = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WatcherBuff)}");
            Pet_WatcherItem = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WatcherItem)}");
            Pet_WatcherProj = new Tex2DWithPath($"{Path_Pets}/{nameof(Pet_WatcherProj)}");

        }
        public override void Unload()
        {
            Wreach = null;

            Item_SpearofDarknessThrown = null;
            Item_DialecticsThrown = null;
            Item_StormSpear = null;
            Item_Disaster = null;
            Item_Judgement = null;
            Item_AetherfireSmasher = null;
            Item_DeathTolls = null;
            Item_BinaryStars = null;
            Item_FierySpear = null;

            Equip_StardustBless = null;
            Equip_StardustRune = null;
            Equip_RewardofKingdom = null;
            Equip_RewardofWarrior = null;
            Equip_PreciousAim = null;
            Equip_PreciousTarget = null;
            Equip_HeartoftheCrystal = null;
            Equip_HeartoftheMountain = null;
            Equip_CrimsonRune = null;

            Material_DisasterBar = null;

            Proj_SpearofDarkness = null;
            Proj_StormSpear = null;
            Proj_CandLanceFire = null;
            Proj_Dialectics = null;
            Proj_Disaster = null;
            Proj_FierySpear = null;

            Pet_SquidBuff = null;
            Pet_SquidItem = null;
            Pet_SquidProj = null;
            Pet_WatcherBuff = null;
            Pet_WatcherItem = null;
            Pet_WatcherProj = null;
            Pet_WhaleBuff = null;
            Pet_WhaleItem = null;
            Pet_WhaleProj = null;
        }
    }
}
