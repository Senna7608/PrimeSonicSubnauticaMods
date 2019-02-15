﻿namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class MovedRecipe : EmPropertyCollection, IMovedRecipe
    {
        internal const string TutorialText = "MovedRecipe: Move an existing recipe in the crafting tree from one location to another. Great for reorganizing.";

        protected readonly EmProperty<string> emTechType;
        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly EmYesNo hidden;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>()
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<string>("OldPath"),
            new EmProperty<string>("NewPath"),
            new EmYesNo("Hidden"){ Optional = true }
        };

        public MovedRecipe() : this(MovedRecipeProperties)
        {
        }

        protected MovedRecipe(ICollection<EmProperty> definitions) : base("MovedRecipe", definitions)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            oldPath = (EmProperty<string>)Properties["OldPath"];
            newPath = (EmProperty<string>)Properties["NewPath"];
            hidden = (EmYesNo)Properties["Hidden"];
        }

        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public string OldPath
        {
            get => oldPath.Value;
            set => oldPath.Value = value;
        }

        public string NewPath
        {
            get => newPath.Value;
            set => newPath.Value = value;
        }

        public bool Hidden
        {
            get => hidden.Value;
            set => hidden.Value = value;
        }

        internal override EmProperty Copy() => new MovedRecipe(this.CopyDefinitions);
    }
}
