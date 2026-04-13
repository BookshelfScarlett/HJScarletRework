namespace HJScarletRework.Globals.Handlers
{
    public static class AssetHandler
    {
        private static string AssetPath = "HJScarletRework/Assets/Texture/";
        private static string ItemPath = AssetPath + "Items";
        public static readonly string Armors = $"{ItemPath}/{nameof(Armors)}/";
        public static readonly string Equips = $"{ItemPath}/{nameof(Equips)}/";
        public static readonly string Materials = $"{ItemPath}/{nameof(Materials)}/";
        public static readonly string Useables = $"{ItemPath}/{nameof(Useables)}/";
        public static readonly string Weapons =$"{ItemPath}/{nameof(Weapons)}/";
        public static string LocalizedHelper(this string assetPath)
        {
            if (assetPath.Equals(Armors))
                return "Armor";
            if (assetPath.Equals(Equips))
                return "Accessories";
            if(assetPath.Equals(Materials))
                return "Material";
            if(assetPath.Equals(Useables))
                return "Useable";
            if (assetPath.Equals(Weapons))
                return nameof(Weapons);
            return string.Empty;
        }
    }
}
