﻿namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class NuclearCharger : CyclopsModule
    {
        internal const float BatteryCapacity = 6000f; // Less than the normal 20k for balance

        internal NuclearModuleConfig Config { get; } = new NuclearModuleConfig();

        internal NuclearFabricator NukFabricator { get; } = new NuclearFabricator();

        internal NuclearCharger()
            : base("CyclopsNuclearModule",
                  "Cyclops Nuclear Reactor Module",
                  "Recharge your Cyclops using this portable nuclear reactor. Intelligently provides power only when you need it.",
                  CraftTree.Type.Workbench, // TODO Custom fabricator for all that is Cyclops and nuclear
                  new[] { "CyclopsMenu" },
                  TechType.BaseNuclearReactor)
        {
        }

        public override ModuleTypes ModuleID => ModuleTypes.Nuclear;

        protected override void Patch()
        {
            // Patch through the base as normal then patch the nuclear fabricator
            base.Patch();

            NukFabricator.Patch(CyclopsModule.ModulesEnabled);

            if (!CyclopsModule.ModulesEnabled) // Even if the options have this be disabled,
                return; // we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

            OptionsPanelHandler.RegisterModOptions(Config);
            Config.Initialize();
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[5]
                             {
                                 new Ingredient(TechType.ReactorRod, 1), // This is to validate that the player has access to nuclear power already
                                 new Ingredient(TechType.Benzene, 1), // And this is the validate that they've gone a little further down
                                 new Ingredient(TechType.Lead, 2), // Extra insulation
                                 new Ingredient(TechType.AdvancedWiringKit, 1), // All the smarts
                                 new Ingredient(TechType.PlasteelIngot, 1) // Housing
                             })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            NuclearChargerID = techTypeID;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
            GameObject obj = GameObject.Instantiate(prefab);

            // The battery component makes it easy to track the charge and saving the data is automatic.
            var pCell = obj.AddComponent<Battery>();
            pCell.name = "NuclearBattery";
            pCell._capacity = BatteryCapacity;

            return obj;
        }
    }
}
