﻿using AbilityUser;
using RimWorld;
using Verse;

namespace Vampire
{
    public class Projectile_BloodReturner : Projectile_AbilityBase
    {
        public override void Impact_Override(Thing hitThing)
        {
            base.Impact_Override(hitThing);
            if (hitThing is Pawn p && p.BloodNeed() is Need_Blood bn)
            {
                MoteMaker.ThrowText(p.DrawPos, p.Map, "+2");
                bn.AdjustBlood(2);
            }
        }
        
    }
}
