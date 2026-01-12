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
        internal static string WeaponPath = $"{ItemPath}/Weapons";
        internal static string AccessoriesPath = $"{ItemPath}/Accessories";
        internal static Tex2DWithPath Wreach { get; private set; }
        internal static Tex2DWithPath Item_SpearofDarknessThrown { get; private set; }
        internal static Tex2DWithPath Item_DialecticsThrown { get; private set; }
        internal static Tex2DWithPath Item_StormSpear { get; private set; }
        internal static Tex2DWithPath Item_Disaster { get; private set; }
        internal static Tex2DWithPath Item_Judgement { get; private set; }
        internal static Tex2DWithPath Item_AetherfireSmasher { get; private set; }
        internal static Tex2DWithPath Item_DeathTolls { get; private set; }
        internal static Tex2DWithPath Item_BinaryStars { get; private set; }
        internal static Tex2DWithPath Item_Bamboomerang { get; private set; }

        internal static Tex2DWithPath Item_StardustRune { get; private set; }
        internal static Tex2DWithPath Item_StardustBless { get; private set; }
        internal static Tex2DWithPath Item_PreciousAim { get; private set; }
        internal static Tex2DWithPath Item_PreciousTarget { get; private set; }
        internal static Tex2DWithPath Item_RewardofKingdom { get; private set; }
        internal static Tex2DWithPath Item_RewardofWarrior { get; private set; }
        internal static Tex2DWithPath Item_HeartoftheCrystal { get; private set; }
        internal static Tex2DWithPath Item_HeartoftheMountain { get; private set; }
        internal static Tex2DWithPath Item_CrimsonRune { get; private set; }

        internal static Tex2DWithPath Proj_SpearofDarkness { get; private set; }
        internal static Tex2DWithPath Proj_StormSpear { get; private set; }
        internal static Tex2DWithPath Proj_CandLanceFire { get; private set; }
        internal static Tex2DWithPath Proj_Dialectics { get; private set; }
        internal static Tex2DWithPath Proj_Disaster { get; private set; }
        

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

            Item_StardustBless = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_StardustBless)}");    
            Item_StardustRune = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_StardustRune)}");    
            Item_RewardofKingdom = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_RewardofKingdom)}");
            Item_RewardofWarrior = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_RewardofWarrior)}");
            Item_PreciousAim = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_PreciousAim)}");
            Item_PreciousTarget = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_PreciousTarget)}");
            Item_HeartoftheCrystal = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_HeartoftheCrystal)}");
            Item_HeartoftheMountain = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_HeartoftheMountain)}");
            Item_CrimsonRune = new Tex2DWithPath($"{AccessoriesPath}/{nameof(Item_CrimsonRune)}");

            Proj_SpearofDarkness = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_SpearofDarkness)}");
            Proj_StormSpear = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_StormSpear)}");
            Proj_CandLanceFire = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_CandLanceFire)}");
            Proj_Dialectics = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_Dialectics)}");
            Proj_Disaster = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_Disaster)}");
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

            Item_StardustBless = null;
            Item_StardustRune = null;
            Item_RewardofKingdom = null;
            Item_RewardofWarrior = null;
            Item_PreciousAim = null;
            Item_PreciousTarget = null;
            Item_HeartoftheCrystal = null;
            Item_HeartoftheMountain = null;
            Item_CrimsonRune = null;

            Proj_SpearofDarkness = null;
            Proj_StormSpear = null;
            Proj_CandLanceFire = null;
            Proj_Dialectics = null;
            Proj_Disaster = null;
        }
    }
}
