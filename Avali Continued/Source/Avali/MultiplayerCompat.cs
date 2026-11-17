using System;
using Multiplayer.API;
using Verse;

namespace Avali
{
	// Multiplayer runs the simulation in lockstep; state changes made from the UI (gizmos) must be
	// sent as synced commands. Without the Multiplayer mod MP.enabled is false and nothing happens.
	[StaticConstructorOnStartup]
	public static class MultiplayerCompat
	{
		static MultiplayerCompat()
		{
			if (!MP.enabled)
			{
				return;
			}
			try
			{
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), nameof(Building_TurretGunAvali.InterfaceChangeTurretShootDelay));
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), nameof(Building_TurretGunAvali.ResetShootDelay));
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), nameof(Building_TurretGunAvali.ExtractShell));
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), nameof(Building_TurretGunAvali.ToggleHoldFire));
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), "ResetForcedTarget");
				MP.RegisterSyncMethod(typeof(Building_TurretGunAvali), nameof(Building_TurretGunAvali.OrderAttack));
				MP.RegisterSyncMethod(typeof(CompRangedWeaponAvali), nameof(CompRangedWeaponAvali.EraseOwnerPawnInfo));
			}
			catch (Exception e)
			{
				Log.Error("[Avali] Multiplayer sync registration failed: " + e);
			}
		}
	}
}
