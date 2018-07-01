﻿namespace MoreCyclopsUpgrades
{
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// This class handles keeping track of the nuclear batteries.
    /// </summary>
    internal static class NuclearChargingManager
    {
        internal const float BatteryDrainRate = 0.15f;
        internal const float MaxCharge = 6000f; // Less than the normal 20k for balance
        internal const float MinDeficitForCharge = 50f;

        /// <summary>
        /// Replaces a nuclear battery modules with Depleted Reactor Rods when they fully drained.
        /// </summary>
        public static void HandleBatteryDepletion(Equipment modules, string slotName, Battery nuclearBattery)
        {
            if (nuclearBattery.charge <= 0f) // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedModule(), true);
            }
        }
 
        private static InventoryItem SpawnDepletedModule()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod);
            GameObject gameObject = Object.Instantiate(prefab);

            gameObject.GetComponent<PrefabIdentifier>().ClassId = "DepletedCyclopsNuclearModule";
            gameObject.AddComponent<TechTag>().type = DepletedNuclearModule.TechTypeID;

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }

}
