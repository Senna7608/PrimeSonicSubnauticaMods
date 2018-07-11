﻿namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using System.Text;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization;
    using Common.EasyMarkup;

    public class QPatch
    {
        private static readonly string FolderRoot = $"./QMods/{CustomCraft.RootModName}/";
        private static readonly string CustomSizesFile = FolderRoot + "CustomSizes.txt";
        private static readonly string ModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private static readonly string AddedRecipiesFile = FolderRoot + "AddedRecipes.txt";
        private static readonly string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";

        private static CustomSizeList customSizeList;
        private static ModifiedRecipeList modifiedRecipeList;
        private static AddedRecipeList addedRecipeList;

        public static void Patch()
        {
            Logger.Log("Loading files begin");

            PatchCustomSizes();

            PatchModifiedRecipes();

            PatchAddedRecipes();

            HandleReadMeFile();

            Logger.Log("Loading files complete");
        }

        private static void HandleReadMeFile()
        {
            if (!File.Exists(HowToFile))
            {
                var builder = new StringBuilder();
                builder.AppendLine($"# How to use {CustomCraft.RootModName} v1.0 #");
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine();
                builder.AppendLine($"# {CustomCraft.RootModName} uses simple text files to send simple requests to SMLHelper, no additional DLLs needed #");
                builder.AppendLine("# This can be great for those who have a few simple and specific ideas in mind but aren't able to create a whole mod themselves #");
                builder.AppendLine("# Special thanks to Nexus modder Iw23J, creator of the original Custom Craft mod, who showed us how much we can empower players to take modding into their own hands #");
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine("# Currently, this mod includes the following features: #");
                builder.AppendLine("# 1 - Customize the space occupied by an inventory item #");
                builder.AppendLine("# 2 - Modify an existing crafting recipe: #");
                builder.AppendLine("#   a - You can change a recipe's required ingredients in any way #");
                builder.AppendLine("#   b - You can alter how many copies of the item are created when you craft the recipe #");
                builder.AppendLine("#   c - You can also modify the recipe's linked items, those being items also created along side the main one #");
                builder.AppendLine("# 3 - Adding new recipes and placing them into any of the existing fabricators #");
                builder.AppendLine($"# Remember that only the standard in-game items can be used with {CustomCraft.RootModName} #");
                builder.AppendLine("# Modded items can't be used at this time #");
                builder.AppendLine("# Additional features may be added in the future so keep an eye on the Nexus mod page #");
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine();
                builder.AppendLine($"# Writing the text files that {CustomCraft.RootModName} uses can be simple if you're paying attention #");
                builder.AppendLine("# You can even copy the text from the sample files directly and use them right away without any changes #");
                builder.AppendLine("# When you want to add a new item reference, remember that the 'ItemID' is the internal spawn ID of that item #");
                builder.AppendLine("# You can visit the Subnautica Wikia for the full list of item IDs at http://subnautica.wikia.com/wiki/Obtainable_Items #");
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine();
                builder.AppendLine("# You'll notice that a lot of text is written between these hash signs (or pound sign or hashtag if you prefer) #");
                builder.AppendLine("# Any text in between these symbols is safely ignored by the mod's text reader #");
                builder.AppendLine("# Use these if you want to leave notes or comments for yourself in the txt files #");
                builder.AppendLine("# Whitespace is also ignored, so feel free to make your files as flat or as indented as you prefer #");
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine();
                builder.AppendLine("# Once you've created your txt files, go ahead and launch the game #");
                builder.AppendLine("# Assuming you've got everything configured correctly, your customizations will appear on your next game #");
                builder.AppendLine();
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine("# Extra tip: When adding recipes, you must use the correct path to the tab you wish to add it to #");
                builder.AppendLine("# Provided here is a list of all the valid paths to all the standard tabs for all available fabricators #");
                builder.AppendLine();
                builder.AppendLine(PathHelper.GeneratePaths());
                builder.AppendLine();
                builder.AppendLine("# -------------------------------------------- #");
                builder.AppendLine("# Enjoy and happy modding #");

                File.WriteAllText(HowToFile, builder.ToString());
                Logger.Log($"{HowToFile} file not found. File created.");
            }
        }

        private static void PatchAddedRecipes()
        {
            addedRecipeList = new AddedRecipeList();
            if (File.Exists(AddedRecipiesFile))
            {
                string serializedData = File.ReadAllText(AddedRecipiesFile);
                if (!string.IsNullOrEmpty(serializedData) && addedRecipeList.Deserialize(serializedData))
                {
                    foreach (IAddedRecipe item in addedRecipeList)
                    {
                        try
                        {
                            CustomCraft.AddRecipe(item.ItemID, item.SmlHelperRecipe(), new CraftingPath(item.Path));
                        }
                        catch
                        {
                            Logger.Log($"Error on AddRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }

                    }

                    Logger.Log($"AddedRecipies loaded.");
                }
                else
                {
                    Logger.Log($"No AddedRecipes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(AddedRecipiesFile, $"# Added Recipes #{Environment.NewLine}" +
                    $"# Check the AddedRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #{Environment.NewLine}");
                Logger.Log($"{AddedRecipiesFile} file not found. Empty file created.");
            }
        }

        private static void PatchModifiedRecipes()
        {
            modifiedRecipeList = new ModifiedRecipeList();
            if (File.Exists(ModifiedRecipesFile))
            {
                string serializedData = File.ReadAllText(ModifiedRecipesFile);
                if (!string.IsNullOrEmpty(serializedData) && modifiedRecipeList.Deserialize(serializedData))
                {
                    foreach (IModifiedRecipe item in modifiedRecipeList)
                    {
                        try
                        {
                            CustomCraft.ModifyRecipe(item.ItemID, item.SmlHelperRecipe());
                        }
                        catch
                        {
                            Logger.Log($"Error on ModifyRecipe{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{item}");
                        }
                    }

                    Logger.Log($"ModifiedRecipes loaded.");
                }
                else
                {
                    Logger.Log($"No ModifiedRecipes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(ModifiedRecipesFile, $"# Modified Recipes #{Environment.NewLine}" +
                    $"# Check the ModifiedRecipes_Samples.txt file in the SampleFiles folder for details on how to alter existing crafting recipes #{Environment.NewLine}");
                Logger.Log($"{ModifiedRecipesFile} file not found. Empty file created.");
            }
        }

        private static void PatchCustomSizes()
        {
            customSizeList = new CustomSizeList();
            if (File.Exists(CustomSizesFile))
            {
                string serializedData = File.ReadAllText(CustomSizesFile);
                if (!string.IsNullOrEmpty(serializedData) && customSizeList.Deserialize(serializedData))
                {
                    foreach (ICustomSize customSize in customSizeList)
                    {
                        try
                        { 
                        CustomCraft.CustomizeItemSize(customSize.ItemID, customSize.Width, customSize.Height);
                        }
                        catch
                        {
                            Logger.Log($"Error on CustomizeItemSize{Environment.NewLine}" +
                                        $"Entry with error:{Environment.NewLine}" +
                                        $"{customSize}");
                        }
                    }

                    Logger.Log($"CustomSizes loaded.");
                }
                else
                {
                    Logger.Log($"No CustomSizes were loaded. File was empty or malformed.");
                }
            }
            else
            {
                File.WriteAllText(CustomSizesFile,
                    $"# Custom Sizes go in this file #{Environment.NewLine}" +
                    $"# Check the CustomSizes_Samples.txt file in the SampleFiles folder for details on how to set your own custom sizes #{Environment.NewLine}");
                Logger.Log($"{CustomSizesFile} file not found. Empty file created.");
            }
        }
    }



}
