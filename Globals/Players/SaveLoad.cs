using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(terraRecipe_CurEat), terraRecipe_CurEat);
            tag.Add(nameof(terraRecipe_haventEat), terraRecipe_haventEat);
            tag.Add(nameof(terraRecipe), terraRecipe);
            tag.Add(nameof(terraRecipe_EatenFoods), terraRecipe_EatenFoods);
            tag.Add(nameof(terraRecipe_LifeMaxMultTime), terraRecipe_LifeMaxMultTime);
        }
        public override void LoadData(TagCompound tag)
        {
            terraRecipe_CurEat = (System.Collections.Generic.List<int>)tag.GetList<int>(nameof(terraRecipe_CurEat));
            terraRecipe_haventEat= (System.Collections.Generic.List<int>)tag.GetList<int>(nameof(terraRecipe_haventEat));
            terraRecipe = tag.GetBool(nameof(terraRecipe));
            terraRecipe_EatenFoods = tag.GetInt(nameof(terraRecipe_EatenFoods));
            terraRecipe_LifeMaxMultTime = tag.GetInt(nameof(terraRecipe_LifeMaxMultTime));
        }

    }
}
