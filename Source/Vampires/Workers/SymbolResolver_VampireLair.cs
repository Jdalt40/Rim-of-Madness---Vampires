﻿using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace Vampire
{
	public class SymbolResolver_VampireLair : SymbolResolver
	{
		public override void Resolve(ResolveParams rp)
		{
			var map = BaseGen.globalSettings.map;
			Faction faction = rp.faction ?? Find.FactionManager.FirstFactionOfDef(FactionDef.Named("ROMV_Sabbat")); //Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
			int num = 0;
			int? edgeDefenseWidth = rp.edgeDefenseWidth;
			if (edgeDefenseWidth != null)
			{
				num = rp.edgeDefenseWidth.Value;
			}
			else if (rp.rect.Width >= 20 && rp.rect.Height >= 20 && (faction.def.techLevel >= TechLevel.Industrial || Rand.Bool))
			{
				num = ((!Rand.Bool) ? 4 : 2);
			}
			float num2 = (float)rp.rect.Area / 144f * 0.17f;
			BaseGen.globalSettings.minEmptyNodes = ((num2 >= 1f) ? GenMath.RoundRandom(num2) : 0);
			Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, rp.rect.CenterCell), map, null);
			TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
			ResolveParams resolveParams = rp;
			resolveParams.rect = rp.rect;
			resolveParams.faction = faction;
			resolveParams.singlePawnLord = singlePawnLord;
			resolveParams.pawnGroupKindDef = (rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Settlement);
			resolveParams.singlePawnSpawnCellExtraPredicate = (rp.singlePawnSpawnCellExtraPredicate ?? ((IntVec3 x) => map.reachability.CanReachMapEdge(x, traverseParms)));
			if (resolveParams.pawnGroupMakerParams == null)
			{
				float num3 = 1000f; //(!faction.def.techLevel.IsNeolithicOrWorse()) ? SymbolResolver_VampireLair.NonNeolithicPawnsPoints.RandomInRange : SymbolResolver_VampireLair.NeolithicPawnsPoints.RandomInRange;
				float? factionBasePawnGroupPointsFactor = rp.settlementPawnGroupPoints;
				if (factionBasePawnGroupPointsFactor != null)
				{
					num3 *= rp.settlementPawnGroupPoints.Value;
				}
				resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
				resolveParams.pawnGroupMakerParams.tile = map.Tile;
				resolveParams.pawnGroupMakerParams.faction = faction;
				resolveParams.pawnGroupMakerParams.points = num3;
				resolveParams.pawnGroupMakerParams.inhabitants = true;
			}
			BaseGen.symbolStack.Push("pawnGroup", resolveParams);
			BaseGen.symbolStack.Push("outdoorLighting", rp);
			if (faction.def.techLevel >= TechLevel.Industrial)
			{
				int num4 = (!Rand.Chance(0.75f)) ? 0 : GenMath.RoundRandom((float)rp.rect.Area / 400f);
				for (int i = 0; i < num4; i++)
				{
					ResolveParams resolveParams2 = rp;
					resolveParams2.faction = faction;
					BaseGen.symbolStack.Push("firefoamPopper", resolveParams2);
				}
			}
			if (num > 0)
			{
				ResolveParams resolveParams3 = rp;
				resolveParams3.faction = faction;
				resolveParams3.edgeDefenseWidth = new int?(num);
				BaseGen.symbolStack.Push("edgeDefense", resolveParams3);
			}
			ResolveParams resolveParams4 = rp;
			resolveParams4.rect = rp.rect.ContractedBy(num);
			resolveParams4.faction = faction;
			BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams4);
			ResolveParams resolveParams5 = rp;
			resolveParams5.rect = rp.rect.ContractedBy(num);
			resolveParams5.faction = faction;
			BaseGen.symbolStack.Push("basePart_outdoors", resolveParams5);
		}
	}
}
