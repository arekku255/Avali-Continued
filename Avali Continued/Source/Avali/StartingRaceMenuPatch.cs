using System.Collections.Generic;
using System.Linq;
using AlienRace;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Avali
{
	// Humanoid Alien Races builds its starting-pawn race menu from the RaceSettings
	// startingColonists entries registered for the player's faction. Races only register
	// for the vanilla player factions, so an Avali start (AvaliPlayerColony) would offer
	// nothing but Avali. This adds humans and every race available to a vanilla colony
	// to that menu, at zero chance: they only appear when the player picks them, never
	// on a random reroll.
	[StaticConstructorOnStartup]
	public static class StartingRaceMenuPatch
	{
		static StartingRaceMenuPatch()
		{
			var target = AccessTools.Method(typeof(AlienRace.HarmonyPatches), "NewGeneratedStartingPawnKinds");
			if (target == null)
			{
				Log.Warning("[Avali] Could not find HAR NewGeneratedStartingPawnKinds; other races won't be selectable on Avali starts.");
				return;
			}
			new Harmony("arekku255.avali.startingracemenu").Patch(target,
				postfix: new HarmonyMethod(typeof(StartingRaceMenuPatch), nameof(Postfix)));
		}

		public static void Postfix(ref IEnumerable<PawnKindEntry> __result)
		{
			FactionDef playerFaction = Faction.OfPlayerSilentFail?.def;
			if (playerFaction == null || playerFaction.defName != "AvaliPlayerColony")
			{
				return;
			}

			var extraKinds = new List<PawnKindDef> { PawnKindDefOf.Colonist };
			extraKinds.AddRange(DefDatabase<RaceSettings>.AllDefsListForReading
				.Where(rs => rs.pawnKindSettings?.startingColonists != null)
				.SelectMany(rs => rs.pawnKindSettings.startingColonists)
				.Where(fpke => fpke.factionDefs != null && fpke.factionDefs.Contains(FactionDefOf.PlayerColony))
				.SelectMany(fpke => fpke.pawnKindEntries)
				.Where(pke => pke.kindDefs != null)
				.SelectMany(pke => pke.kindDefs));

			List<PawnKindEntry> entries = __result.ToList();
			var alreadyListed = new HashSet<PawnKindDef>(entries.SelectMany(pke => pke.kindDefs));
			List<PawnKindDef> toAdd = extraKinds.Where(k => k != null && alreadyListed.Add(k)).ToList();
			if (toAdd.Count > 0)
			{
				entries.Add(new PawnKindEntry { kindDefs = toAdd, chance = 0f });
			}
			__result = entries;
		}
	}
}
