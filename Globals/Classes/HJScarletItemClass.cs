using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletItemClass : ModItem, ILocalizedModType
    {
        public virtual string AssetPath => string.Empty;
        //力竭了
        public new string LocalizationCategory => $"Items.{AssetPath.LocalizedHelper()}";
        public override string Texture => AssetPath + GetType().Name;
        public override void SetDefaults()
        {
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public virtual void ExSD() { }
    }
}
