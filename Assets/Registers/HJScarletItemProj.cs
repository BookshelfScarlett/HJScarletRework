using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Build.ObjectModelRemoting;
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
    public static class InstallMethods
    {
        public static string GetItemTexture(this string name, AssetCategory type) => $"HJScarletRework/Assets/Texture/{type}s/{type}_{name}";
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
        internal static string AccessoriesPath = $"{ItemPath}/Equips";
        internal static string MaterialsPath = $"{ItemPath}/Materials";
        internal static Tex2DWithPath Wreach { get; private set; }
        internal static Tex2DWithPath Item_SpearofDarknessThrown { get; private set; }
        internal static Tex2DWithPath Item_DialecticsThrown { get; private set; }
        

        internal static Tex2DWithPath Proj_SpearofDarkness { get; private set; }
        internal static Tex2DWithPath Proj_CandLanceFire { get; private set; }
        internal static Tex2DWithPath Proj_Dialectics { get; private set; }

        public override void Load()
        {
            Wreach = new Tex2DWithPath($"{ItemPath}/{nameof(Wreach)}");

            Item_SpearofDarknessThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(SpearofDarknessThrown)}");
            Item_DialecticsThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(DialecticsThrown)}");

            Proj_SpearofDarkness = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_SpearofDarkness)}");
            Proj_CandLanceFire = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_CandLanceFire)}");
            Proj_Dialectics = new Tex2DWithPath($"{ProjPath}/{nameof(Proj_Dialectics)}");


        }
        public override void Unload()
        {
            Wreach = null;

            Item_SpearofDarknessThrown = null;
            Item_DialecticsThrown = null;

            Proj_SpearofDarkness = null;
            Proj_CandLanceFire = null;
            Proj_Dialectics = null;
        }
    }
}
