using Terraria.Audio;

namespace HJScarletRework.Assets.Registers
{
    public class HJScarletSounds
    {
        public static string SoundsPath => "HJScarletRework/Assets/Sounds/";
        public static SoundStyle DeathsToll_Toss => new($"{SoundsPath}{nameof(DeathsToll_Toss)}");
        public static SoundStyle Evolution_Thrown => new($"{SoundsPath}{nameof(Evolution_Thrown)}");
        public static SoundStyle GrabCharge => new($"{SoundsPath}{nameof(GrabCharge)}");
        public static SoundStyle Dialectics_Throw => new($"{SoundsPath}{nameof(Dialectics_Throw)}");
        public static SoundStyle Dialectics_Hit => new($"{SoundsPath}{nameof(Dialectics_Hit)}");
        public static SoundStyle TheMars_Toss => new($"{SoundsPath}{nameof(TheMars_Toss)}");
        public static SoundStyle TheMars_Hit => new($"{SoundsPath}{nameof(TheMars_Hit)}");
        public static SoundStyle SodomsDisaster_BoomHit => new($"{SoundsPath}{nameof(SodomsDisaster_BoomHit)}");
        public static SoundStyle SodomsDisaster_Hit => new($"{SoundsPath}{nameof(SodomsDisaster_Hit)}");
        public static SoundStyle SodomsDisaster_Toss => new($"{SoundsPath}{nameof(SodomsDisaster_Toss)}");
        public static SoundStyle HymnFireball_Release => new($"{SoundsPath}{nameof(HymnFireball_Release)}");
        public static SoundStyle Light_Fire => new($"{SoundsPath}{nameof(Light_Fire)}", numVariants: 3);
        public static SoundStyle Light_CrackedShield => new($"{SoundsPath}{nameof(Light_CrackedShield)}", numVariants: 4);
        public static SoundStyle Light_FleshHit => new($"{SoundsPath}{nameof(Light_FleshHit)}", numVariants: 4);
        public static SoundStyle Light_ShieldHit => new($"{SoundsPath}{nameof(Light_ShieldHit)}", numVariants: 2);
        public static SoundStyle Dream_Toss => new($"{SoundsPath}{nameof(Dream_Toss)}");

        #region 锤子们
        private static SoundStyle Smash_AirHeavy1 => new($"{SoundsPath}{nameof(Smash_AirHeavy1)}");
        private static SoundStyle Smash_AirHeavy2 => new($"{SoundsPath}{nameof(Smash_AirHeavy2)}");
        public static SoundStyle Smash_GroundHeavy => new($"{SoundsPath}{nameof(Smash_GroundHeavy)}");
        private static SoundStyle Hammer_Shoot1 => new($"{SoundsPath}{nameof(Hammer_Shoot1)}");
        private static SoundStyle Hammer_Shoot2 => new($"{SoundsPath}{nameof(Hammer_Shoot2)}");
        private static SoundStyle Hammer_Shoot3 => new($"{SoundsPath}{nameof(Hammer_Shoot3)}");
        public static SoundStyle Hammer_LightHit => new($"{SoundsPath}{nameof(Hammer_LightHit)}");
        #endregion
        public static SoundStyle Buzz => new($"{SoundsPath}{nameof(Buzz)}");

        private static SoundStyle Atom_Strike1 => new($"{SoundsPath}{nameof(Atom_Strike1)}");
        private static SoundStyle Atom_Strike2 => new($"{SoundsPath}{nameof(Atom_Strike2)}");
        private static SoundStyle Atom_Strike3 => new($"{SoundsPath}{nameof(Atom_Strike3)}");
        public static SoundStyle Pipes => new($"{SoundsPath}{nameof(Pipes)}");
        public static SoundStyle Misc_MagicStaffFire => new($"{SoundsPath}{nameof(Misc_MagicStaffFire)}");
        public static SoundStyle Misc_SwordHit => new($"{SoundsPath}{nameof(Misc_SwordHit)}");
        public static SoundStyle Misc_AngelBlast => new($"{SoundsPath}{nameof(Misc_AngelBlast)}");
        public static SoundStyle Misc_KnifeExpired => new($"{SoundsPath}{nameof(Misc_KnifeExpired)}");
        private static SoundStyle Misc_KnifeToss1 => new($"{SoundsPath}{nameof(Misc_KnifeToss1)}");
        private static SoundStyle Misc_KnifeToss2 => new($"{SoundsPath}{nameof(Misc_KnifeToss2)}");
        private static SoundStyle Misc_KnifeToss3 => new($"{SoundsPath}{nameof(Misc_KnifeToss3)}");
        public static SoundStyle GalvanizedHand_Hit => new($"{SoundsPath}{nameof(GalvanizedHand_Hit)}", numVariants: 2);
        public static SoundStyle GalvanizedHand_Hit2 => new($"{SoundsPath}{nameof(GalvanizedHand_Hit2)}");
        public static SoundStyle GalvanizedHand_Toss => new($"{SoundsPath}{nameof(GalvanizedHand_Toss)}", numVariants: 3);
        public static SoundStyle GalvanizedHand_Charge => new($"{SoundsPath}{nameof(GalvanizedHand_Charge)}", numVariants: 2);
        public static SoundStyle SpearofEscape_Toss => new($"{SoundsPath}{nameof(SpearofEscape_Toss)}", numVariants: 3);
        public static SoundStyle SpearofEscape_Boom => new($"{SoundsPath}{nameof(SpearofEscape_Boom)}");
        public static SoundStyle[] Hammer_Shoot =>
            [
                Hammer_Shoot1,
                Hammer_Shoot2,
                Hammer_Shoot3,
            ];
        public static SoundStyle[] Smash_AirHeavy =>
            [
                Smash_AirHeavy1,
                Smash_AirHeavy2,
            ];
        public static SoundStyle[] Atom_Strike =>
            [
                Atom_Strike1,
                Atom_Strike2,
                Atom_Strike3,
            ];
        public static SoundStyle[] Misc_KnifeToss =>
            [
                Misc_KnifeToss1,
                Misc_KnifeToss2,
                Misc_KnifeToss3,
            ];
    }
}
