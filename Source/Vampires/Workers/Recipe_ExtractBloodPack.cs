﻿using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;
using System.Linq;

namespace Vampire
{
    public class Recipe_ExtractBloodPack : Recipe_Surgery
    {
        private const float ViolationGoodwillImpact = 10f;

        [DebuggerHidden]
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            if (pawn?.BloodNeed() is Need_Blood n && !n.IsAnimal && n.CurBloodPoints > 0) //Animals do not have blood vials.
            {
                if (pawn.RaceProps.body.corePart is BodyPartRecord r)
                    yield return r;
            }
        }

        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            if (BloodItemUtility.ExtractionWillKill(pawn, true))
            {
                Messages.Message("ROMV_DeadlyOperation".Translate(pawn.Label), MessageTypeDefOf.NegativeEvent);
            }
            return pawn.Faction != billDoerFaction; //|| BloodItemUtility.ExtractionWillKill(pawn, true); //&& HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
        }

        public static bool IsClean(Pawn pawn, BodyPartRecord part)
        {
            return !pawn.Dead && !(from x in pawn.health.hediffSet.hediffs
                                   where x.Part == part
                                   select x).Any();
        }
        

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            bool flag = IsClean(pawn, part);
            bool flag2 = IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                //if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part))
                //{
                //    return;
                //}
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                BloodItemUtility.SpawnBloodFromExtraction(pawn, true);
            }
            pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 1, -1f, null, part));
            int badGoodwillAmt = -3;
            if (flag)
            {
                if (pawn.Dead)
                {
                    badGoodwillAmt = -20;

                    VampireThoughtUtility.GiveThoughtsForPawnDiedOfBloodLoss(pawn, billDoer);
                }
                else
                {
                    VampireThoughtUtility.GiveThoughtsForPawnBloodHarvested(pawn); //ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
                }
            }
            if (flag2)
            {
                pawn.Faction.TryAffectGoodwillWith(billDoer.Faction, badGoodwillAmt);
            }
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
        {
            return recipe.LabelCap + " (" + BloodTypeUtility.BloodType(pawn).GetLabel() + ")" + ((BloodItemUtility.ExtractionWillKill(pawn, true)) ? " (" + "ROMV_DeadlyOperationShort".Translate() + ")" : "");
        }
    }
}
