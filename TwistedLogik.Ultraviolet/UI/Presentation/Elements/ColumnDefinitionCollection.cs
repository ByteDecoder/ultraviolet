﻿using System;
using System.Collections.Generic;
using TwistedLogik.Nucleus;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Elements
{
    /// <summary>
    /// Represents a collection of column definitions belonging to an instance of the <see cref="Grid"/> container class.
    /// </summary>
    public sealed partial class ColumnDefinitionCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinitionCollection"/> class.
        /// </summary>
        /// <param name="grid">The <see cref="Grid"/> that owns the collection.</param>
        internal ColumnDefinitionCollection(Grid grid)
        {
            Contract.Require(grid, "grid");

            this.grid = grid;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            if (storage.Count > 0)
            {
                foreach (var item in storage)
                {
                    item.Grid = null;
                }
                storage.Clear();

                OnModified();
            }
        }

        /// <summary>
        /// Adds a column to the collection.
        /// </summary>
        /// <param name="definition">The column definition to add to the collection.</param>
        public void Add(ColumnDefinition definition)
        {
            Contract.Require(definition, "definition");

            if (definition.Grid != null)
                definition.Grid.ColumnDefinitions.Remove(definition);

            definition.Grid = grid;
            storage.Add(definition);

            OnModified();
        }

        /// <summary>
        /// Removes a column from the collection.
        /// </summary>
        /// <param name="definition">The column to remove from the collection.</param>
        /// <returns><c>true</c> if the specified column was removed from the collection; otherwise, <c>false</c>.</returns>
        public Boolean Remove(ColumnDefinition definition)
        {
            Contract.Require(definition, "definition");

            if (storage.Remove(definition))
            {
                definition.Grid = null;

                OnModified();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified column.
        /// </summary>
        /// <param name="definition">The column to evaluate.</param>
        /// <returns><c>true</c> if the collection contains the specified column; otherwise, <c>false</c>.</returns>
        public Boolean Contains(ColumnDefinition definition)
        {
            Contract.Require(definition, "definition");

            return storage.Contains(definition);
        }

        /// <summary>
        /// Gets the column definition at the specified index within the collection.
        /// </summary>
        /// <param name="ix">The index of the column definition to retrieve.</param>
        /// <returns>The column definition at the specified index within the collection.</returns>
        public ColumnDefinition this[Int32 ix]
        {
            get { return storage[ix]; }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public Int32 Count
        {
            get { return storage.Count; }
        }

        /// <summary>
        /// Called when the collection is modified.
        /// </summary>
        private void OnModified()
        {
            grid.OnColumnsModified();
        }

        // State values.
        private readonly Grid grid;
        private readonly List<ColumnDefinition> storage = 
            new List<ColumnDefinition>();
    }
}
