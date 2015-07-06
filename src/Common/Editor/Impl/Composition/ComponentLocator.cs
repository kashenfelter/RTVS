﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Languages.Editor.Shell;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.Languages.Editor.Composition
{
    public static class ComponentLocator<TComponent> where TComponent : class
    {
        public static TComponent Import()
        {
            SingleImporter importer = new SingleImporter();
            EditorShell.CompositionService.SatisfyImportsOnce(importer);

            return importer.Import;
        }

        public static IEnumerable<Lazy<TComponent>> ImportMany()
        {
            return ImportMany(EditorShell.CompositionService);
        }

        public static IEnumerable<Lazy<TComponent>> ImportMany(ICompositionService compositionService)
        {
            ManyImporter importer = new ManyImporter();
            compositionService.SatisfyImportsOnce(importer);

            return importer.Imports;
        }

        private class SingleImporter
        {
            [Import]
            public TComponent Import { get; set; }
        }

        private class ManyImporter
        {
            [ImportMany]
            public IEnumerable<Lazy<TComponent>> Imports { get; set; }
        }
    }

    public static class ComponentLocatorWithMetadata<TComponent, TMetadata>
        where TComponent : class
        where TMetadata : class
    {
        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany()
        {
            return ImportMany(EditorShell.CompositionService);
        }

        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany(ICompositionService compositionService)
        {
            ManyImporter importer = new ManyImporter();
            compositionService.SatisfyImportsOnce(importer);

            return importer.Imports;
        }

        private class ManyImporter
        {
            [ImportMany]
            public IEnumerable<Lazy<TComponent, TMetadata>> Imports { get; set; }
        }
    }

    /// <summary>
    /// Allows using [Order] and [Name] attributes on exports and have them imported in the correct order.
    /// </summary>
    public static class ComponentLocatorWithOrdering<TComponent, TMetadata>
        where TComponent : class
        where TMetadata : IOrderable
    {
        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany()
        {
            return ImportMany(EditorShell.CompositionService);
        }

        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany(ICompositionService compositionService)
        {
            ManyImporter importer = new ManyImporter();
            compositionService.SatisfyImportsOnce(importer);

            return Orderer.Order(importer.Imports);
        }

        /// <summary>
        /// Reverses the order of imported items
        /// </summary>
        public static IEnumerable<Lazy<TComponent, TMetadata>> ReverseImportMany(ICompositionService compositionService)
        {
            List<Lazy<TComponent, TMetadata>> reversedList = new List<Lazy<TComponent, TMetadata>>();

            foreach (Lazy<TComponent, TMetadata> item in ImportMany(compositionService))
            {
                reversedList.Insert(0, item);
            }

            return reversedList;
        }

        /// <summary>
        /// Reverses the order of imported items
        /// </summary>
        public static IEnumerable<Lazy<TComponent, TMetadata>> ReverseImportMany()
        {
            return ReverseImportMany(EditorShell.CompositionService);
        }

        private class ManyImporter
        {
            [ImportMany]
            public IEnumerable<Lazy<TComponent, TMetadata>> Imports { get; set; }
        }
    }

    /// <summary>
    /// Assumes IOrderable for the metadata interface
    /// </summary>
    public static class ComponentLocatorWithOrdering<TComponent> where TComponent : class
    {
        public static IEnumerable<Lazy<TComponent, IOrderable>> ImportMany(ICompositionService compositionService)
        {
            return ComponentLocatorWithOrdering<TComponent, IOrderable>.ImportMany(compositionService);
        }

        public static IEnumerable<Lazy<TComponent, IOrderable>> ImportMany()
        {
            return ComponentLocatorWithOrdering<TComponent, IOrderable>.ImportMany();
        }

        public static IEnumerable<Lazy<TComponent, IOrderable>> ReverseImportMany(ICompositionService compositionService)
        {
            return ComponentLocatorWithOrdering<TComponent, IOrderable>.ReverseImportMany(compositionService);
        }

        public static IEnumerable<Lazy<TComponent, IOrderable>> ReverseImportMany()
        {
            return ComponentLocatorWithOrdering<TComponent, IOrderable>.ReverseImportMany();
        }

        /// <summary>
        /// Returns the topmost component within the ordered set
        /// </summary>
        public static TComponent GetFirst()
        {
            var enumerator = ComponentLocatorWithOrdering<TComponent, IOrderable>.ImportMany().GetEnumerator();

            if (enumerator.MoveNext())
                return enumerator.Current.Value;

            return default(TComponent);
        }
    }

    /// <summary>
    /// Locates components by content type
    /// </summary>
    public static class ComponentLocatorForContentType<TComponent, TMetadata>
        where TComponent : class
        where TMetadata : class, IComponentContentTypes
    {
        /// <summary>
        /// Locates all components exported with a given content type or with any of the content type base types
        /// </summary>
        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany(string contentTypeName)
        {
            var contentTypeRegistry = EditorShell.ExportProvider.GetExport<IContentTypeRegistryService>().Value;
            var contentType = contentTypeRegistry.GetContentType(contentTypeName);

            return ImportMany(EditorShell.CompositionService, contentType);
        }

        /// <summary>
        /// Locates all components exported with a given content type or with any of the content type base types
        /// </summary>
        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany(IContentType contentType)
        {
            return ImportMany(EditorShell.CompositionService, contentType);
        }

        /// <summary>
        /// Locates all components exported with a given content type exactly
        /// </summary>
        public static IEnumerable<Lazy<TComponent, TMetadata>> ImportManyExact(IContentType contentType)
        {
            return ImportManyExact(EditorShell.CompositionService, contentType);
        }

        /// <summary>
        /// Locates first available component that matches content type name or any of its base types
        /// </summary>
        public static Lazy<TComponent, TMetadata> ImportOne(string contentTypeName)
        {
            var contentTypeRegistry = EditorShell.ExportProvider.GetExport<IContentTypeRegistryService>().Value;
            var contentType = contentTypeRegistry.GetContentType(contentTypeName);

            return ImportOne(contentType);
        }

        /// <summary>
        /// Locates first available component that matches content type name or any of its base types
        /// </summary>
        public static Lazy<TComponent, TMetadata> ImportOne(IContentType contentType)
        {
            var providers = ImportMany(contentType);

#if DEBUG
            int count = 0;
            foreach (var provider in providers)
                count++;

            Debug.Assert(count < 2, String.Format(CultureInfo.CurrentCulture,
                "ComponentLocatorForContentType.ImportOne: More than one export of type {0} found for content type {1}",
                typeof(TComponent),
                contentType.TypeName));
#endif
            foreach (var provider in providers)
            {
                return provider;
            }

            return null;
        }

        /// <summary>
        /// Locates first available component that matches content type name exactly
        /// </summary>
        public static Lazy<TComponent, TMetadata> ImportOneExact(IContentType contentType)
        {
            var providers = ImportManyExact(contentType);

#if DEBUG
            int count = 0;
            foreach (var provider in providers)
                count++;

            Debug.Assert(count < 2, String.Format(CultureInfo.CurrentCulture,
                "ComponentLocatorForContentType.ImportOneExact: More than one export of type {0} found for content type {1}",
                typeof(TComponent),
                contentType.TypeName));
#endif
            foreach (var provider in providers)
            {
                return provider;
            }

            return null;
        }

        internal static IEnumerable<Lazy<TComponent, TMetadata>> ImportMany(ICompositionService compositionService, IContentType contentType)
        {
            IEnumerable<Lazy<TComponent, TMetadata>> components =
                ComponentLocatorWithMetadata<TComponent, TMetadata>.ImportMany(compositionService);

            return FilterByContentType(contentType, components);
        }

        internal static IEnumerable<Lazy<TComponent, TMetadata>> ImportManyExact(ICompositionService compositionService, IContentType contentType)
        {
            IEnumerable<Lazy<TComponent, TMetadata>> components =
                ComponentLocatorWithMetadata<TComponent, TMetadata>.ImportMany(compositionService);

            return FilterByContentTypeExact(contentType, components);
        }

        //// The resultant enumerable has the more specific content type matches before less specific ones
        internal static IEnumerable<Lazy<TComponent, TMetadata>> FilterByContentType(IContentType contentType, IEnumerable<Lazy<TComponent, TMetadata>> components)
        {
            List<IContentType> allContentTypes;
            List<string> allContentTypeNames;

            GetAllContentTypes(contentType, out allContentTypes, out allContentTypeNames);

            foreach (var curContentType in allContentTypeNames)
            {
                foreach (Lazy<TComponent, TMetadata> pair in components)
                {
                    if (pair.Metadata.ContentTypes != null)
                    {
                        foreach (string componentContentType in pair.Metadata.ContentTypes)
                        {
                            if (componentContentType.Equals(curContentType, StringComparison.OrdinalIgnoreCase))
                            {
                                yield return pair;
                            }
                        }
                    }
                }
            }
        }

        //// The resultant enumerable has the more specific content type matches before less specific ones
        internal static IEnumerable<Lazy<TComponent, TMetadata>> FilterByContentTypeExact(IContentType contentType, IEnumerable<Lazy<TComponent, TMetadata>> components)
        {
            foreach (Lazy<TComponent, TMetadata> pair in components)
            {
                if (pair.Metadata.ContentTypes != null)
                {
                    foreach (string componentContentType in pair.Metadata.ContentTypes)
                    {
                        if (componentContentType.Equals(contentType.TypeName, StringComparison.OrdinalIgnoreCase))
                        {
                            yield return pair;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given content type provides collection that includes this type and all its base types
        /// </summary>
        private static void GetAllContentTypes(IContentType contentType, out List<IContentType> allContentTypes, out List<string> allContentTypeNames)
        {
            allContentTypes = new List<IContentType>();
            allContentTypeNames = new List<string>();

            allContentTypes.Add(contentType);

            // Add all base types and all their base types
            for (int i = 0; i < allContentTypes.Count; i++)
            {
                IContentType curContentType = allContentTypes[i];

                allContentTypeNames.Add(curContentType.TypeName);
                foreach (IContentType baseContentType in curContentType.BaseTypes)
                {
                    if (!allContentTypeNames.Contains(baseContentType.TypeName))
                        allContentTypes.Add(baseContentType);
                }
            }
        }
    }

    /// <summary>
    /// Imports components that supply Order, Name, and ContentType as metadata.
    /// Maintains specified order within the content type.
    /// </summary>
    public static class ComponentLocatorForOrderedContentType<TComponent> where TComponent : class
    {
        public static IEnumerable<Lazy<TComponent>> ImportMany(IContentType contentType)
        {
            return ImportMany(EditorShell.CompositionService, contentType);
        }

        public static IEnumerable<Lazy<TComponent>> ImportMany(ICompositionService compositionService, IContentType contentType)
        {
            var components = ComponentLocatorForContentType<TComponent, IOrderedComponentContentTypes>.ImportMany(compositionService, contentType);

            return Orderer.Order(components);
        }

        /// <summary>
        /// Locates first component withing ordered components of a particular content type.
        /// </summary>
        public static TComponent FindFirstOrderedComponent(IContentType contentType)
        {
            IEnumerable<Lazy<TComponent>> components = ImportMany(contentType);

            foreach (Lazy<TComponent> pair in components)
            {
                return pair.Value;
            }

            return default(TComponent);
        }

        /// <summary>
        /// Locates first component withing ordered components of a particular content type.
        /// </summary>
        public static TComponent FindFirstOrderedComponent(string contentTypeName)
        {
            var contentTypeRegistryService = EditorShell.ExportProvider.GetExport<IContentTypeRegistryService>().Value;
            var contentType = contentTypeRegistryService.GetContentType(contentTypeName);

            return FindFirstOrderedComponent(contentType);
        }
    }
}
