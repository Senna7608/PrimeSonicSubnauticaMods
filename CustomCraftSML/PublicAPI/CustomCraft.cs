﻿namespace CustomCraft2SML.PublicAPI
{
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {
        public const string RootModName = "CustomCraft2SML";

        public static void AddRecipe(TechType craftedItem, TechData recipe, CraftingPath craftingPath)
        {
            AddRecipe(craftedItem, recipe, craftingPath.Scheme, craftingPath.Path);
        }

        public static void AddRecipe(TechType craftedItem, TechData recipe, CraftTree.Type craftTree, string path)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            //CraftTreeHandler.customNodes.Add(new CustomCraftNode(craftedItem, craftTree, path));            
            CraftDataHandler.SetTechData(craftedItem, recipe);

            string[] steps = path.Split(CraftingNode.Splitter);

            if (steps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(craftTree, craftedItem);
            else
                CraftTreeHandler.AddCraftingNode(craftTree, craftedItem, steps);

            KnownTechHandler.UnlockOnStart(craftedItem);
        }

        public static void ModifyRecipe(TechType craftedItem, TechData recipe)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            CraftDataHandler.SetTechData(craftedItem, recipe);
        }

        public static void CustomizeItemSize(TechType inventoryItem, int width, int height)
        {
            Assert.AreNotEqual(inventoryItem.ToString(), ((int)inventoryItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(width > 0 && height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(width < 6 && height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataHandler.SetItemSize(inventoryItem, width, height);
            
        }
    }
}
