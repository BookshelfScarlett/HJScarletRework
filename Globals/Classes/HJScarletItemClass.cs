using HJScarletRework.Globals.Handlers;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletItemClass : ModItem, ILocalizedModType
    {
        public virtual string AssetPath => string.Empty;
        //力竭了
        public new string LocalizationCategory => $"Items.{AssetPath.LocalizedHelper()}";
        public override string Texture => AssetPath + GetType().Name;
    }
}
