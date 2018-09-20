using System.IO;
using XliffTasks.Tasks;
using Xunit;

namespace XliffTasks.Tests
{
    public class UpdateXlfTests
    {
        [Fact]
        public void XamlRule_WithTranslations_FileGetsCreated()
        {

            string tempPath = Path.Combine(Path.GetTempPath(), "UpdateXlfTests");
            try
            {
                var inputPath = Path.Combine(tempPath, "Input");
                if (!Directory.Exists(tempPath))
                {
                    // creating the input path with automatically create the tempPath, and we need it later anyway
                    Directory.CreateDirectory(inputPath);
                }

                string source =
    @"<Rule Name=""MyRule"" DisplayName=""My rule display name"" PageTemplate=""generic"" Description=""My rule description"" xmlns=""http://schemas.microsoft.com/build/2009/properties"">
  <Rule.Categories>
    <Category Name=""MyCategory"" DisplayName=""My category display name"" />
  </Rule.Categories>
  <EnumProperty Name=""MyEnumProperty"" DisplayName=""My enum property display name"" Category=""MyCategory"" Description=""Specifies the source file will be copied to the output directory."">
    <EnumValue Name=""First"" DisplayName=""Do the first thing"" />
    <EnumValue Name=""Second"" DisplayName=""Do the second thing"" />
    <EnumValue Name=""Third"" DisplayName=""Do the third thing"" />
  </EnumProperty>
  <BoolProperty Name=""MyBoolProperty"" Description=""My bool property description."" />
</Rule>";
                var sourceFile = Path.Combine(inputPath, "input.xaml");
                File.WriteAllText(sourceFile, source);

                var xlfPath = Path.Combine(tempPath, "xlf");

                var task = new UpdateXlf();
                task.AllowModification = true;
                task.Languages = new string[] { "fr" };
                task.TranslateDocument(sourceFile, "XamlRule", xlfPath);

                Assert.True(File.Exists(Path.Combine(xlfPath, "xlf.fr.xlf")));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void XamlRule_NoTranslations_NoFileCreated()
        {

            var tempPath = Path.Combine(Path.GetTempPath(), "UpdateXlfTests");
            try
            {
                var inputPath = Path.Combine(tempPath, "Input");
                if (!Directory.Exists(tempPath))
                {
                    // creating the input path with automatically create the tempPath, and we need it later anyway
                    Directory.CreateDirectory(inputPath);
                }

                string source =
 @"<Rule Name=""MyRule"" PageTemplate=""generic"">
  <Rule.Categories>
    <Category Name=""MyCategory"" />
  </Rule.Categories>
  <EnumProperty Name=""MyEnumProperty"" Category=""MyCategory"">
    <EnumValue Name=""First"" />
    <EnumValue Name=""Second"" />
    <EnumValue Name=""Third"" />
  </EnumProperty>
  <BoolProperty Name=""MyBoolProperty"" />
</Rule>";
                var sourceFile = Path.Combine(inputPath, "input.xaml");
                File.WriteAllText(sourceFile, source);

                var xlfPath = Path.Combine(tempPath, "xlf");

                var task = new UpdateXlf();
                task.AllowModification = true;
                task.Languages = new string[] { "fr" };
                task.TranslateDocument(sourceFile, "XamlRule", xlfPath);

                Assert.False(File.Exists(Path.Combine(xlfPath, "xlf.fr.xlf")));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
    }
}
