# xliff-tasks

A set of MSBuild tasks and targets to automatically update xliff (.xlf) files for localizable resources, and to build satellite assemblies from those xliff files.

## Build Status

|Windows x64 |
|:------:|
|[![][win-x64-build-badge]][win-x64-build]|

## Installing

If you're using the [Roslyn Repo Toolset][roslyn-repo-toolset] then the `XliffTasks` package is already pulled in, and enabled by default.

Otherwise, you'll need to add the dotnet-core feed on MyGet (`https://dotnet.myget.org/F/dotnet-core/api/v3/index.json`) to your nuget.config file, and then add a `PackageReference` for the XliffTasks package, like so:

```
<PackageReference Include="XliffTasks" Version="0.2.0-beta-000081" PrivateAssets="all" />
```

The `PrivateAssets` metdata is needed to prevent `dotnet pack` or `msbuild /t:pack` from listing `XliffTasks` as one of your package's dependencies.

## Using XliffTasks

Once `XliffTasks` is installed building a project will automatically build satellite assemblies from .xlf files. To _update_ .xlf files to bring them in line with the source .resx/.vsct/.xaml files you need to run the `UpdateXlf` target, like so: 

```
msbuild /t:UpdateXlf
```

This will only update the .xlf files. Alternatively, run a normal build with the `UpdateXlfOnBuild` property set:

```
msbuild /p:UpdateXlfOnBuild=true
```

By default, `XliffTasks` will produce an error during build if it detects that the .xlf files are out of data with the source .resx/.vsct/.xaml files.

Many teams using `XliffTasks` default `UpdateXlfOnBuild` to true for local developer builds, but leave it off for CI builds. This way the .xlf files are automatically updated as the developer works, and the CI build will fail if the developer forgets to include the changes to the .xlf files as part of their PR. This way the .xlf files are always in sync with the source files, and can be handed off to a localization team at any time.

Other workflows are possible by changing the `XliffTasks` properties.

## Properties

`EnableXlfLocalization` - The "master switch" for turning locallization with `XliffTasks` on or off completely. When set to false, .xlf files will not be updated and satellite assemblies will not be generated from the .xlf files, regardless of the other properties. Defaults to true, but it is useful to set it to false for any project that does not need to produce localized resources (unit test projects, packaging projects, etc.).

`UpdateXlfOnBuild` - When set to true, .xlf files will automatically be brought in sync with the source .resx/.vsct/.xaml files. This may involve adding or removing items from the .xlf files, or creating new .xlf files. Defaults to false.

`ErrorOnOutOfDateXlf` - When set to true the build will produce an error if the .xlf files are out-of-date with respect to the source files. Defaults to true.

`XlfLanguages` - The set of locales to which the project is localized. Defaults to the thirteen locales supported by Visual Studio: `cs;de;es;fr;it;ja;ko;pl;pt-BR;ru;tr;zh-Hans;zh-Hant`.

## Contact

For more information, contact @tmeschter or @nguerrera on GitHub, or file an issue.
 
[win-x64-build-badge]: https://devdiv.visualstudio.com/_apis/public/build/definitions/0bdbc590-a062-4c3f-b0f6-9383f67865ee/5803/badge
[win-x64-build]: https://devdiv.visualstudio.com/DevDiv/_build/index?definitionId=5803&_a=completed
[roslyn-repo-toolset]: https://github.com/dotnet/roslyn-tools/blob/master/docs/RepoToolset.md
