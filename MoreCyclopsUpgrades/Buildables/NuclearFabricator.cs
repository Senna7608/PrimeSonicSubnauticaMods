﻿namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class NuclearFabricator : ModPrefab
    {
        public CraftTree.Type TreeTypeID { get; private set; }        

        // This name will be used as both the new TechType of the buildable fabricator and the CraftTree Type for the custom crafting tree.
        public const string NameID = "NuclearFabricator";
        public const string FriendlyName = "Nuclear Fabricator";
        public const string HandOverText = "UseNukFabricator";

        internal NuclearFabricator() : base(NameID, $"{NameID}PreFab")
        {
        }

        // This patch is handled by the Nuclear Charger
        internal void Patch(bool nukFabricatorEnabled)
        {
            // Create new Craft Tree Type
            CreateCustomTree(out CraftTree.Type craftType);
            TreeTypeID = craftType;

            // Create a new TechType for new fabricator
            this.TechType = TechTypeHandler.AddTechType(NameID, FriendlyName,
                "A specialized fabricator for safe handling of radioactive energy sources.", false);

            if (!nukFabricatorEnabled) // Even if the options have this be disabled,
                return; // we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

            // Create a Recipie for the new TechType
            var customFabRecipe = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.Titanium, 2),
                                 new Ingredient(TechType.ComputerChip, 1),
                                 new Ingredient(TechType.Magnetite, 1),
                                 new Ingredient(TechType.Lead, 2),
                             })
            };

            // Add the new TechType to the buildables
            CraftDataHandler.AddBuildable(this.TechType);

            // Add the new TechType to the group of Interior Module buildables
            CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, this.TechType);

            // Set the buildable prefab
            PrefabHandler.RegisterPrefab(this);

            LanguageHandler.SetLanguageLine(HandOverText, "Use Nuclear Fabricator");

            // Set the custom sprite for the Habitat Builder Tool menu
            SpriteHandler.RegisterSprite(this.TechType, @"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorI.png");

            // Associate the recipie to the new TechType
            CraftDataHandler.SetTechData(this.TechType, customFabRecipe);

            KnownTechHandler.SetAnalysisTechEntry(TechType.BaseNuclearReactor, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
        }

        private void CreateCustomTree(out CraftTree.Type craftType)
        {
            ModCraftTreeRoot rootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(NameID, out craftType);

            rootNode.AddCraftingNode(CyclopsModule.NuclearChargerID);
            rootNode.AddCraftingNode(CyclopsModule.RefillNuclearModuleID);
            rootNode.AddCraftingNode(TechType.ReactorRod);
        }

        public override GameObject GetGameObject()
        {
            // Instantiate CyclopsFabricator object
            GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/Fabricator"));

            // Update prefab name
            prefab.name = NameID;

            // Add prefab ID
            var prefabId = prefab.AddComponent<PrefabIdentifier>();
            prefabId.ClassId = NameID;
            prefabId.name = FriendlyName;

            // Add tech tag
            var techTag = prefab.AddComponent<TechTag>();
            techTag.type = this.TechType;

            // Update sky applier
            var skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            // Associate custom craft tree to the fabricator
            var fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = TreeTypeID;
            fabricator.handOverText = HandOverText;

            // Associate power relay
            var ghost = fabricator.GetComponent<GhostCrafter>();
            var powerRelay = new PowerRelay();

            fabricator.SetPrivateField("powerRelay", powerRelay, BindingFlags.FlattenHierarchy);

            // Add constructable
            var constructible = prefab.AddComponent<Constructable>();
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnWall = true;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = this.TechType; // This was necessary to correctly associate the recipe at building time

            // Set the custom texture
            var customTexture = ImageUtils.LoadTextureFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorT.png");
            var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = customTexture;

            return prefab;
        }
    }
}
