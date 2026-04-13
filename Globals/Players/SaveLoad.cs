using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void SaveData(TagCompound tag)
        {
            HammerTagSave(tag);
            tag.Add(nameof(terraRecipe_CurEat), terraRecipe_CurEat);
            tag.Add(nameof(terraRecipe_haventEat), terraRecipe_haventEat);
            tag.Add(nameof(terraRecipe), terraRecipe);
            tag.Add(nameof(terraRecipe_EatenFoods), terraRecipe_EatenFoods);
            tag.Add(nameof(terraRecipe_LifeMaxMultTime), terraRecipe_LifeMaxMultTime);
        }
        public override void LoadData(TagCompound tag)
        {
            HammerTagLoad(tag);
        }
        private void HammerTagSave(TagCompound tag)
        {
            tag.Add(nameof(NoGuideForBinaryStars), NoGuideForBinaryStars);
            tag.Add(nameof(CanDisableGuideForGrandHammer), CanDisableGuideForGrandHammer);
            tag.Add(nameof(CanGiveFreeBinaryStars), CanGiveFreeBinaryStars);
        }

        private void HammerTagLoad(TagCompound tag)
        {
            NoGuideForBinaryStars = tag.GetBool(nameof(NoGuideForBinaryStars));
            CanDisableGuideForGrandHammer = tag.GetBool(nameof(CanDisableGuideForGrandHammer));
            CanGiveFreeBinaryStars = tag.GetBool(nameof(CanGiveFreeBinaryStars));
            terraRecipe_CurEat = (System.Collections.Generic.List<int>)tag.GetList<int>(nameof(terraRecipe_CurEat));
            terraRecipe_haventEat= (System.Collections.Generic.List<int>)tag.GetList<int>(nameof(terraRecipe_haventEat));
            terraRecipe = tag.GetBool(nameof(terraRecipe));
            terraRecipe_EatenFoods = tag.GetInt(nameof(terraRecipe_EatenFoods));
            terraRecipe_LifeMaxMultTime = tag.GetInt(nameof(terraRecipe_LifeMaxMultTime));
        }

    }
}
