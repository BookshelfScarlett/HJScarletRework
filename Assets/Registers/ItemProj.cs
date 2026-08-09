using HJScarletRework.Items.Weapons.Melee;
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
    internal class HJScarletItemProj : ModSystem
    {
        internal static string ItemPath = "HJScarletRework/Assets/Texture/Items";
        internal static string ProjPath = "HJScarletRework/Assets/Texture/Projs";
        internal static string Path_Pets = $"HJScarletRework/Assets/Texture/Pets";
        internal static string WeaponPath = $"{ItemPath}/Weapons";
        internal static string AccessoriesPath = $"{ItemPath}/Equips";
        internal static string MaterialsPath = $"{ItemPath}/Materials";
        internal static Tex2DWithPath Wreach { get; private set; }
        internal static Tex2DWithPath Item_SpearofDarknessThrown { get; private set; }
        internal static Tex2DWithPath Item_DialecticsThrown { get; private set; }
        internal static Tex2DWithPath DualWraithStaffBlade { get; private set; }
        internal static Tex2DWithPath DualWraithStaff { get; private set; }
        public override void Load()
        {
            Wreach = new Tex2DWithPath($"{ItemPath}/{nameof(Wreach)}");

            Item_SpearofDarknessThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(SpearofDarknessThrown)}");
            Item_DialecticsThrown = new Tex2DWithPath($"{WeaponPath}/{nameof(DialecticsThrown)}");
            DualWraithStaffBlade = new Tex2DWithPath($"{WeaponPath}/{nameof(DualWraithStaffBlade)}");
            DualWraithStaff = new Tex2DWithPath($"{WeaponPath}/{nameof(DualWraithStaff)}");
            }
        public override void Unload()
        {
            Wreach = null;

            Item_SpearofDarknessThrown = null;
            Item_DialecticsThrown = null;
            DualWraithStaffBlade = null;
            DualWraithStaff = null;
        }
    }
}
