namespace HJScarletRework.Projs.Ranged
{
    public static class AetherfireSmasherName
    {
        public enum NameType
        {
            TrueScarlet,
            Shizuku,
            SerratAntler,
            WutivOrChaLost,

            Emma,
            SherryOrAnnOrKino,
            Hanna,
            None
        }
        public static NameType SelectedName(this string name)
        {
            name = name.ToLower();
            if (name.Contains("shizuku") || name.Contains("溯月"))
                return NameType.Shizuku;
            if (name.Contains("truescarlet") || name.Contains("fakeaqua") || name.Contains("scarletshelf"))
                return NameType.TrueScarlet;
            if (name.Contains("锯角"))
                return NameType.SerratAntler;
            if (name.Contains("雾梯") || name.Contains("wutiv") || name.Contains("chalost") || name.Contains("查诗络"))
                return NameType.WutivOrChaLost;
            if (name.Contains("sakumara") || name.Contains("emma") || name.Contains("樱羽") || name.Contains("yinjiu") || name.Contains("银九"))
                return NameType.Emma;
            if (name.Contains("sherry") || name.Contains("雪莉") || name.Contains("ann") || name.Contains("安安") || name.Contains("kino") || name.Contains("kinoko"))
                return NameType.SherryOrAnnOrKino; 
            if (name.Contains("hanna") || name.Contains("汉娜"))
                return NameType.Hanna;
            return NameType.None;
        }
    }
}
