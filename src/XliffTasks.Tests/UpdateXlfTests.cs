// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using XliffTasks.Tasks;
using Xunit;
using Microsoft.Build.Utilities;

namespace XliffTasks.Tests
{
    public class UpdateXlfTests
    {
        [Fact]
        public void TwoNodesHasTheSameIdAttributeWhenExecuteNoErrorThrows()
        {
            var updateXlf = new UpdateXlf();
            var a = new TaskItem();
            a.ItemSpec = "testAssetVsct.vsct";
            a.SetMetadata(MetadataKey.XlfSourceFormat, "vsct");

            updateXlf.Sources = new[] { a };
            updateXlf.Languages = new[] { "cs" };
            updateXlf.AllowModification = true;

            var ex = Record.Exception(() => updateXlf.Execute());
            Assert.Null(ex);
        }
    }
}