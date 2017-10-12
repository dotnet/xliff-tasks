// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Threading;
using XliffTasks.Model;

namespace XliffTasks.Tasks
{
    public sealed class UpdateXlf : XlfTask
    {
        [Required]
        public ITaskItem[] Sources { get; set; }

        [Required]
        public string[] Languages { get; set; }

        [Required]
        public bool AllowModification { get; set; }

        private const string HowToUpdate =
            "Run `msbuild /t:UpdateXlf` to update .xlf files or set UpdateXlfOnBuild=true"
            + " to update them on every build, but note that it is strongly discouraged to set"
            + " UpdateXlfOnBuild=true in official/CI build environments as they should not"
            + " modify source code during the build.";

        protected override void ExecuteCore()
        {
            foreach (var item in Sources)
            {
                OnlyOneProcessToWorkOn(Path.GetFullPath(item.ItemSpec),
                    () => {
                        string sourcePath = item.ItemSpec;
                        string sourceFormat = item.GetMetadataOrThrow(MetadataKey.XlfSourceFormat);
                        TranslatableDocument sourceDocument = LoadSourceDocument(sourcePath, sourceFormat);
                        string sourceDocumentId = GetSourceDocumentId(sourcePath);

                        foreach (var language in Languages)
                        {
                            string xlfPath = GetXlfPath(sourcePath, language);
                            XlfDocument xlfDocument;

                            try
                            {
                                xlfDocument = LoadXlfDocument(xlfPath, language, createIfNonExistent: AllowModification);
                            }
                            catch (FileNotFoundException ex) when (ex.FileName == xlfPath)
                            {
                                Release.Assert(!AllowModification);
                                throw new BuildErrorException($"'{xlfPath}' for '{sourcePath}' does not exist. {HowToUpdate}");
                            }

                            if (!xlfDocument.Update(sourceDocument, sourceDocumentId))
                            {
                                continue; // no changes
                            }

                            if (!AllowModification)
                            {
                                throw new BuildErrorException($"'{xlfPath}' is out-of-date with '{sourcePath}'. {HowToUpdate}");
                            }

                            Directory.CreateDirectory(Path.GetDirectoryName(xlfPath));
                            xlfDocument.Save(xlfPath);
                        }
                    });
            }
        }

        private static void OnlyOneProcessToWorkOn(string filePath, Action action)
        {
            using (Mutex mtx = new Mutex(false, @"Global\" + filePath.GetHashCode().ToString()))
            {
                int blockTime = 1000;
                while (true)
                {
                    if (mtx.WaitOne(blockTime))
                    {
                        action();

                        mtx.ReleaseMutex();
                        break;
                    }
                    else
                    {
                        blockTime = blockTime * 2;
                        if (blockTime > 100000)
                        {
                            throw new Exception("Cannot aquire mutex to run on one xlf file, there might be a deadlock");
                        }
                    }
                }
            }
        }
    }
}