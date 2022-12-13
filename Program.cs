using BepInEx;
using RoR2;
using UnityEngine;

namespace EDDI
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Jays.EDDI", "Every Death Drop Item", "1.0.0")]
    public class EveryDeathDropItems : BaseUnityPlugin
    {
        // We have to copy the artifacts code.
        private static PickupDropTable _dropTable;

        private static readonly Xoroshiro128Plus treasureRng = new Xoroshiro128Plus(0UL);

        public void Awake()
        {
            _dropTable = RoR2.LegacyResourcesAPI.Load<PickupDropTable>("DropTables/dtSacrificeArtifact");

            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.Stage.onServerStageBegin += Stage_onServerStageBegin;
        }

        public void OnDisable()
        {
            RoR2.GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }

        private void Stage_onServerStageBegin(Stage obj)
        {
            treasureRng.ResetSeed(RoR2.Run.instance.treasureRng.nextUlong);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.victimMaster == null)
            {
                return;
            }

            if (damageReport.attackerTeamIndex == damageReport.victimTeamIndex && damageReport.victimMaster.minionOwnership.ownerMaster)
            {
                return;
            }

            PickupIndex pickupIndex = _dropTable.GenerateDrop(treasureRng);
            if (pickupIndex != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pickupIndex, damageReport.victimBody.corePosition, UnityEngine.Vector3.up * 20f);
            }
        }
    }
}
