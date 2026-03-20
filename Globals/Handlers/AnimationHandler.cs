namespace HJScarletRework.Globals.Handlers
{
    public struct AnimationStruct(int slot)
    {
        public bool[] IsDone = new bool[slot];
        public int[] Progress = new int[slot];
        public int[] MaxProgress = new int[slot];
        public float[] Buffer = new float[slot];
    }
    public static class AniState
    {
        public const int Begin = 0;
        public const int Mid = 1;
        public const int End = 2;
    }
    public enum AniID
    {
        Begin,
        Mid,
        End
    }
    public static class AniMethods
    {
        public static void UpdateAniState(this AnimationStruct animationStruct, AniID slotID)
        {
            animationStruct.Progress[(int)slotID]++;
        }
        public static bool UpdateAniState(this AnimationStruct animationStruct, int slotID, float bufferLength = 0)
        {
            animationStruct.Progress[slotID]++;
            if (animationStruct.Progress[slotID] >= animationStruct.MaxProgress[slotID])
            {
                if (bufferLength > 0)
                {
                    animationStruct.Buffer[slotID]++;
                    if (animationStruct.Buffer[slotID] >= bufferLength)
                        animationStruct.IsDone[slotID] = true;
                }
                else
                    animationStruct.IsDone[slotID] = true;
            }
            return false;
        }
        public static float GetAniProgress(this AnimationStruct animationStruct, AniID slotID)
        {
            int id = (int)slotID;
            float progress = animationStruct.Progress[id] / (float)animationStruct.MaxProgress[id];
            return Clamp(progress, 0f, 1f);
        }
        public static float UpdateAngle(this AnimationStruct animationHelper, float BeginAngle, float EndAngle, int Filp, float Progress, float PreFilpAdd = 0)
        {
            float startAngleOffset = ToRadians(BeginAngle);
            float endAngleOffset = ToRadians(EndAngle);
            float baseRotation = Lerp(startAngleOffset, endAngleOffset, Progress) + PreFilpAdd;
            if (Filp == -1)
                baseRotation = baseRotation * Filp;
            return baseRotation;
        }
    }
}
