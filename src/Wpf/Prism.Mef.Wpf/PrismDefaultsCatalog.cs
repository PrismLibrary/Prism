

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Prism.Mef
{
    ///<summary>
    /// A very simple custom <see href="ComposablePartCatalog" /> that takes an enumeration
    /// of parts and returns them when requested.
    ///</summary>
    internal class PrismDefaultsCatalog : ComposablePartCatalog
    {
        private readonly IEnumerable<ComposablePartDefinition> parts;

        ///<summary>
        /// Creates a PrismDefaultsCatalog that will return the provided parts when requested.
        ///</summary>
        ///<param name="parts">Parts to add to the catalog</param>
        ///<exception cref="ArgumentNullException">Thrown if the parts parameter is null.</exception>
        public PrismDefaultsCatalog(IEnumerable<ComposablePartDefinition> parts)
        {
            if (parts == null) throw new ArgumentNullException(nameof(parts));
            this.parts = parts;
        }

        /// <summary>
        /// Gets the parts contained in the catalog.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return parts.AsQueryable(); }
        }
    }
}