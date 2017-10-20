// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace XliffTasks.Model
{
    /// <summary>
    /// A <see cref="TranslatableDocument"/> for files in .vsct format.
    /// See https://msdn.microsoft.com/en-us/library/bb164699.aspx
    /// </summary>
    internal sealed class VsctDocument : TranslatableXmlDocument
    {
        protected override IEnumerable<TranslatableNode> GetTranslatableNodes()
        {
            foreach (var strings in Document.Descendants(Document.Root.Name.Namespace + "Strings"))
            {
                string vsctGuid = strings.Parent.Attribute("guid")?.Value ?? string.Empty;
                string id = strings.Parent.Attribute("id").Value;

                foreach (var child in strings.Elements())
                {
                    XName name = child.Name;

                    if (name.LocalName == "CanonicalName")
                    {
                        // CanonicalName is never localized.
                        // LocCanonicalName can be used to specify a localized alternative.
                        // See https://msdn.microsoft.com/en-us/library/bb491712.aspx
                        continue;
                    }

                    var uniqueId = $"{vsctGuid}{id}|{name.LocalName}";

                    yield return new TranslatableXmlElement(
                        id: uniqueId,
                        source: child.Value,
                        note: null,
                        element: child);
                }
            }
        }

        public override void RewriteRelativePathsToAbsolute(string sourceFullPath)
        {
            foreach (var imageTag in Document.Descendants(Document.Root.Name.Namespace + "Bitmap"))
            {
                string resourceRelativePath = imageTag.Attribute("href").Value.Replace('\\', Path.DirectorySeparatorChar);

                var absolutePath = Path.Combine(Path.GetDirectoryName(sourceFullPath), resourceRelativePath);

                imageTag.Attribute("href").Value = absolutePath;
            }
        }
    }
}
