using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReDesertScourgeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<DesertScourge>();
        public override int TotalListCount => 8;
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualDesertScourge;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            if(!proj.HJScarlet().FirstFrame)
            {
                PosList.Clear();
                RotList.Clear();
                for(int i =0;i<TotalListCount;i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                }
            }
            PosList.Add(proj.Center + proj.rotation.ToRotationVector2() * 30f);
            RotList.Add(proj.rotation);
            if (RotList.Count > TotalListCount)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount)
                PosList.RemoveAt(0);
            //new SmokeParticle(proj.Center.ToRandCirclePosEdge(5f) - proj.SafeDir() * 40f, RandVelTwoPi(3f), RandLerpColor(Color.DarkBlue, Color.Black), 40, RandRotTwoPi, 0.75f, Main.rand.NextFloat(0.4f, 0.6f) * 0.5f).SpawnToNonPreMult();
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && RotList.Count < 1)
                return;
        }

    }
}
