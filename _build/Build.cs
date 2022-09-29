using BuildHelpers;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.NSwag;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.VSTest;
using Nuke.Common.Tools.Xunit;
using Nuke.Common.Utilities.Collections;
using Octokit;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[GitHubActions(
    "Release",
    GitHubActionsImage.UbuntuLatest,
    ImportSecrets = new[] { nameof(GitHubToken) },
    OnPushBranches = new[] { "master", "main", "release/*" },
    InvokedTargets = new[] { nameof(Release) }
)]
[GitHubActions(
    "PR_Validation",
    GitHubActionsImage.UbuntuLatest,
    ImportSecrets = new[] { nameof(GitHubToken) },
    OnPullRequestBranches = new[] { "master", "main", "develop", "development", "release/*" },
    InvokedTargets = new[] { nameof(Package) }
)]
[GitHubActions(
    "Build",
    GitHubActionsImage.UbuntuLatest,
    ImportSecrets = new[] { nameof(GitHubToken) },
    OnPushBranches = new[] { "master", "develop", "release/*" },
    InvokedTargets = new[] { nameof(DeployGeneratedFiles) }
    )]
[UnsetVisualStudioEnvironmentVariables]
internal class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Package);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Github token to authenticate in CI")]
    readonly string GitHubToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net6.0", UpdateAssemblyInfo = false, NoFetch = true)] readonly GitVersion GitVersion;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath InstallDirectory => RootDirectory.Parent.Parent / "Install" / "Module";
    AbsolutePath WebProjectDirectory => RootDirectory / "module.web";
    AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";
    AbsolutePath UnitTestsResultsDirectory => TestResultsDirectory / "UnitTests";
    AbsolutePath GithubDirectory => RootDirectory / ".github";
    AbsolutePath BadgesDirectory => GithubDirectory / "badges";
    AbsolutePath UnitTestBadgesDirectory => BadgesDirectory / "UnitTests";
    AbsolutePath ClientServicesDirectory => WebProjectDirectory / "src" / "services";
    AbsolutePath DocsDirectory => RootDirectory / "docs";

    private const string devViewsPath = "http://localhost:3333/build/";
    private const string prodViewsPath = "/DesktopModules/Dnn.SecurityCenter/resources/scripts/dnn-securitycenter/";

    string releaseNotes = "";
    GitHubClient gitHubClient;
    Release release;

    Target UpdateTokens => _ => _
        .OnlyWhenDynamic(() => GitRepository != null)
        .Executes(() =>
        {
            if (GitRepository != null)
            {
                Serilog.Log.Information($"We are on branch {GitRepository.Branch}");
                var repositoryFiles = GlobFiles(RootDirectory, "README.md", "build/**/git.html", "**/articles/git.md");
                repositoryFiles.ForEach(f =>
                {
                    var file = ReadAllText(f, Encoding.UTF8);
                    file = file.Replace("{owner}", GitRepository.GetGitHubOwner());
                    file = file.Replace("{repository}", GitRepository.GetGitHubName());
                    WriteAllText(f, file, Encoding.UTF8);
                });
            }
        });

    Target LogInfo => _ => _
        .Before(Release)
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            Serilog.Log.Information($"Branch name is {GitRepository.Branch}");
            Serilog.Log.Information(SerializationTasks.JsonSerialize(GitVersion));
        });

    Target Clean => _ => _
        .Before(Restore)
        .Before(Package)
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(TestResultsDirectory);
            EnsureCleanDirectory(UnitTestsResultsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.GetProject("Module")));

            DotNetRestore(s => s
                .SetProjectFile(Solution.GetProject("UnitTests")));
        });

    Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            MSBuild(_ => _
                .SetConfiguration(Configuration.Debug)
                .SetProjectFile(Solution.GetProject("UnitTests"))
                .SetTargets("Build")
                .ResetVerbosity());

            DotNetTest(_ => _
                .SetConfiguration(Configuration.Debug)
                .ResetVerbosity()
                .SetResultsDirectory(UnitTestsResultsDirectory)
                .EnableCollectCoverage()
                .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                .SetLoggers("trx;LogFileName=UnitTests.trx")
                .SetCoverletOutput(UnitTestsResultsDirectory / "coverage.xml")
                .SetExcludeByFile("**/App_LocalResources/**/*")
                .SetProjectFile(RootDirectory / "UnitTests" / "UnitTests.csproj")
                .SetNoBuild(true));

            ReportGenerator(_ => _
                .SetReports(UnitTestsResultsDirectory / "*.xml")
                .SetReportTypes(ReportTypes.Badges, ReportTypes.HtmlInline, ReportTypes.HtmlChart)
                .SetTargetDirectory(UnitTestsResultsDirectory)
                .SetHistoryDirectory(RootDirectory / "UnitTests" / "history")
                .SetProcessArgumentConfigurator(a => a
                    .Add("-title:UnitTests"))
                .SetFramework("net5.0"));

            Helpers.CleanCodeCoverageHistoryFiles(RootDirectory / "UnitTests" / "history");

            var testBadges = GlobFiles(UnitTestsResultsDirectory, "badge_branchcoverage.svg", "badge_linecoverage.svg");
            testBadges.ForEach(f => CopyFileToDirectory(f, UnitTestBadgesDirectory, FileExistsPolicy.Overwrite, true));

            if (IsWin && (InvokedTargets.Contains(UnitTests) || InvokedTargets.Contains(Test)))
            {
                Process.Start(@"cmd.exe ", @"/c " + (UnitTestsResultsDirectory / "index.html"));
            }
        });

    Target Test => _ => _
        .DependsOn(UnitTests)
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(SetManifestVersions)
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            var moduleAssemblyName = Solution.GetProject("Module").GetProperty("AssemblyName");
            Helpers.GenerateLocalizationFiles(moduleAssemblyName);
            var assemblyVersion = "0.1.0";
            var fileVersion = "0.1.0";
            if (GitVersion != null && GitRepository != null && GitRepository.IsOnMainOrMasterBranch())
            {
                assemblyVersion = GitVersion.AssemblySemVer;
                fileVersion = GitVersion.InformationalVersion;
            }

            MSBuildTasks.MSBuild(s => s
                .SetProjectFile(Solution.GetProject("Module"))
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(assemblyVersion)
                .SetFileVersion(fileVersion));
        });

    Target SetManifestVersions => _ => _
        .Executes(() =>
        {
            var manifests = GlobFiles(RootDirectory, "**/*.dnn");
            foreach (var manifest in manifests)
            {
                var doc = new XmlDocument();
                doc.Load(manifest);
                var packages = doc.SelectNodes("dotnetnuke/packages/package");
                foreach (XmlNode package in packages)
                {
                    var version = package.Attributes["version"];
                    if (version != null)
                    {
                        Serilog.Log.Information($"Found package {package.Attributes["name"].Value} with version {version.Value}");
                        version.Value =
                            GitVersion != null
                            ? $"{GitVersion.Major.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Minor.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Patch.ToString("00", CultureInfo.InvariantCulture)}"
                            : "00.01.00";
                        Serilog.Log.Information($"Updated package {package.Attributes["name"].Value} to version {version.Value}");

                        var components = package.SelectNodes("components/component");
                        foreach (XmlNode component in components)
                        {
                            if (component.Attributes["type"].Value == "Cleanup")
                            {
                                var cleanupVersion = component.Attributes["version"];
                                cleanupVersion.Value =
                                    GitVersion != null
                                    ? $"{GitVersion.Major.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Minor.ToString("00", CultureInfo.InvariantCulture)}.{GitVersion.Patch.ToString("00", CultureInfo.InvariantCulture)}"
                                    : "00.01.00";
                            }
                        }
                    }
                }
                doc.Save(manifest);
                Serilog.Log.Information($"Saved {manifest}");
            }
        });

    Target DeployBinaries => _ => _
        .OnlyWhenDynamic(() => RootDirectory.Parent.ToString().EndsWith("DesktopModules", StringComparison.OrdinalIgnoreCase))
        .DependsOn(Compile)
        .Executes(() =>
        {
            var manifest = GlobFiles(RootDirectory, "*.dnn").FirstOrDefault();
            var assemblyFiles = Helpers.GetAssembliesFromManifest(manifest);
            var files = GlobFiles(RootDirectory, "bin/Debug/*.dll", "bin/Debug/*.pdb", "bin/Debug/*.xml");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (assemblyFiles.Contains(fileInfo.Name))
                {
                    Helpers.CopyFileToDirectoryIfChanged(file, RootDirectory.Parent.Parent / "bin");
                }
            }
        });

    Target SetRelativeScripts => _ => _
        .DependsOn(DeployFrontEnd)
        .Executes(() =>
        {
            var views = GlobFiles(RootDirectory, "resources/views/**/*.html");
            foreach (var view in views)
            {
                var content = ReadAllText(view);
                content = content.Replace(devViewsPath, prodViewsPath, StringComparison.OrdinalIgnoreCase);
                WriteAllText(view, content, System.Text.Encoding.UTF8);
                Serilog.Log.Information("Set scripts path to {0} in {1}", prodViewsPath, view);
            }
        });

    Target SetLiveServer => _ => _
        .DependsOn(DeployFrontEnd)
        .Executes(() =>
        {
            var views = GlobFiles(RootDirectory, "resources/views/**/*.html");
            foreach (var view in views)
            {
                var content = ReadAllText(view);
                content = content.Replace(prodViewsPath, devViewsPath, StringComparison.OrdinalIgnoreCase);
                WriteAllText(view, content, System.Text.Encoding.UTF8);
                Serilog.Log.Information("Set scripts path to {0} in {1}", devViewsPath, view);
            }
        });

    Target DeployFrontEnd => _ => _
        .DependsOn(BuildFrontEnd)
        .Executes(() =>
        {
            var scriptsDestination = RootDirectory / "resources" / "scripts" / "dnn-securitycenter";
            EnsureCleanDirectory(scriptsDestination);
            CopyDirectoryRecursively(RootDirectory / "module.web" / "dist" / "dnn-securitycenter", scriptsDestination, DirectoryExistsPolicy.Merge);
        });

    Target InstallNpmPackages => _ => _
        .Executes(() =>
        {
            NpmLogger = (type, output) =>
            {
                if (type == OutputType.Std)
                {
                    Serilog.Log.Information(output);
                }
                if (type == OutputType.Err)
                {
                    if (output.StartsWith("npm WARN", StringComparison.OrdinalIgnoreCase))
                    {
                        Serilog.Log.Warning(output);
                    }
                    else
                    {
                        Serilog.Log.Error(output);
                    }
                }
            };
            NpmInstall(s =>
                s.SetProcessWorkingDirectory(WebProjectDirectory));
        });

    Target BuildFrontEnd => _ => _
        .DependsOn(InstallNpmPackages)
        .DependsOn(SetManifestVersions)
        .DependsOn(SetPackagesVersions)
        .DependsOn(Swagger)
        .Executes(() =>
        {
            NpmRun(s => s
                .SetProcessWorkingDirectory(WebProjectDirectory)
                .AddArguments("build")
            );
        });

    Target SetPackagesVersions => _ => _
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            if (GitVersion != null)
            {
                Npm($"version --no-git-tag-version --allow-same-version {GitVersion.MajorMinorPatch}", WebProjectDirectory);
            }
        });

    Target SetupGitHubClient => _ => _
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .OnlyWhenDynamic(() => GitRepository != null)
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            Serilog.Log.Information($"We are on branch {GitRepository.Branch}");
            if (GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
            {
                gitHubClient = new GitHubClient(new ProductHeaderValue("Nuke"));
                var tokenAuth = new Credentials(GitHubToken);
                gitHubClient.Credentials = tokenAuth;
            }
        });

    Target GenerateReleaseNotes => _ => _
        .OnlyWhenDynamic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .DependsOn(SetupGitHubClient)
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            // Get the milestone
            var milestone = gitHubClient.Issue.Milestone.GetAllForRepository(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName()).Result
                .Where(m => m.Title == GitVersion.MajorMinorPatch).FirstOrDefault();
            Serilog.Log.Information(SerializationTasks.JsonSerialize(milestone));
            if (milestone == null)
            {
                Serilog.Log.Warning("Milestone not found for this version");
                releaseNotes = "No release notes for this version.";
                return;
            }

            // Get the PRs
            var prRequest = new PullRequestRequest()
            {
                State = ItemStateFilter.All
            };
            var pullRequests = gitHubClient.Repository.PullRequest.GetAllForRepository(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(), prRequest).Result
                .Where(p =>
                    p.Milestone?.Title == milestone.Title &&
                    p.Merged == true &&
                    p.Milestone?.Title == GitVersion.MajorMinorPatch);
            Serilog.Log.Information(SerializationTasks.JsonSerialize(pullRequests));

            // Build release notes
            var releaseNotesBuilder = new StringBuilder();
            releaseNotesBuilder
                .AppendLine($"# {GitRepository.GetGitHubName()} {milestone.Title}")
                .AppendLine()
                .AppendLine($"A total of {pullRequests.Count()} pull requests where merged in this release.")
                .AppendLine();

            foreach (var group in pullRequests.GroupBy(p => p.Labels[0]?.Name, (label, prs) => new { label, prs }))
            {
                Serilog.Log.Information(SerializationTasks.JsonSerialize(group));
                releaseNotesBuilder.AppendLine($"## {group.label}");
                foreach (var pr in group.prs)
                {
                    Serilog.Log.Information(SerializationTasks.JsonSerialize(pr));
                    releaseNotesBuilder.AppendLine($"- #{pr.Number} {pr.Title}. Thanks @{pr.User.Login}");
                }
            }

            // Checksums
            releaseNotesBuilder
                .AppendLine()
                .Append(File.ReadAllText(ArtifactsDirectory / "checksums.md"));

            releaseNotes = releaseNotesBuilder.ToString();
            Serilog.Log.Information(releaseNotes);
        });

    Target TagRelease => _ => _
        .OnlyWhenDynamic(() => GitRepository != null && (GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch()))
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .DependsOn(SetupGitHubClient)
        .DependsOn(UpdateTokens)
        .Before(Compile)
        .Executes(() =>
        {
            Git($"remote set-url origin https://{GitRepository.GetGitHubOwner()}:{GitHubToken}@github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}.git");
            var version = GitRepository.IsOnMainOrMasterBranch() ? GitVersion.MajorMinorPatch : GitVersion.SemVer;
            GitLogger = (type, output) => Serilog.Log.Information(output);
            Git($"tag v{version}");
            Git($"push --tags");
        });

    Target Release => _ => _
        .OnlyWhenDynamic(() => GitRepository != null && (GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch()))
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .DependsOn(UpdateTokens)
        .DependsOn(SetupGitHubClient)
        .DependsOn(GenerateReleaseNotes)
        .DependsOn(TagRelease)
        .DependsOn(Package)
        .Executes(() =>
        {
            var newRelease = new NewRelease(GitRepository.IsOnMainOrMasterBranch() ? $"v{GitVersion.MajorMinorPatch}" : $"v{GitVersion.SemVer}")
            {
                Body = releaseNotes,
                Draft = true,
                Name = GitRepository.IsOnMainOrMasterBranch() ? $"v{GitVersion.MajorMinorPatch}" : $"v{GitVersion.SemVer}",
                TargetCommitish = GitVersion.Sha,
                Prerelease = GitRepository.IsOnReleaseBranch(),
            };
            release = gitHubClient.Repository.Release.Create(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(),
                newRelease).Result;
            Serilog.Log.Information($"{release.Name} released !");

            var artifactFile = GlobFiles(RootDirectory, "artifacts/**/*.zip").FirstOrDefault();
            var artifact = File.OpenRead(artifactFile);
            var artifactInfo = new FileInfo(artifactFile);
            var assetUpload = new ReleaseAssetUpload()
            {
                FileName = artifactInfo.Name,
                ContentType = "application/zip",
                RawData = artifact
            };
            var asset = gitHubClient.Repository.Release.UploadAsset(release, assetUpload).Result;
            Serilog.Log.Information($"Asset {asset.Name} published at {asset.BrowserDownloadUrl}");
        });

    /// <summary>
    /// Lauch in deploy mode, updates the module on the current local site.
    /// </summary>
    Target Deploy => _ => _
        .DependsOn(DeployBinaries)
        .DependsOn(SetRelativeScripts)
        .Executes(() =>
        {
        });

    /// <summary>
    /// Watch frontend for changes
    /// </summary>
    Target Watch => _ => _
    .DependsOn(SetLiveServer)
    .Executes(() =>
    {
        NpmRun(s => s
            .SetProcessWorkingDirectory(WebProjectDirectory)
            .AddArguments("start")
            );
    });


    Target GenerateAppConfig => _ => _
    .OnlyWhenDynamic(() => RootDirectory.Parent.ToString().EndsWith("DesktopModules", StringComparison.OrdinalIgnoreCase))
    .Executes(() =>
    {
        var webConfigPath = RootDirectory.Parent.Parent / "web.config";
        var webConfigDoc = new XmlDocument();
        webConfigDoc.Load(webConfigPath);
        var connectionString = webConfigDoc.SelectSingleNode("/configuration/connectionStrings/add[@name='SiteSqlServer']");

        var appConfigPath = RootDirectory / "_build" / "App.config";
        var appConfig = new XmlDocument();
        var configurationNode = appConfig.AppendChild(appConfig.CreateElement("configuration"));
        var connectionStringsNode = configurationNode.AppendChild(appConfig.CreateElement("connectionStrings"));
        var importedNode = connectionStringsNode.OwnerDocument.ImportNode(connectionString, true);
        connectionStringsNode.AppendChild(importedNode);
        appConfig.Save(appConfigPath);

        Serilog.Log.Information("Generated {0} from {1}", appConfigPath, webConfigPath);
        Serilog.Log.Information("This file is local as it could contain credentials, it should not be committed to the repository.");
    });

    /// <summary>
    /// Package the module
    /// </summary>
    Target Package => _ => _
        .DependsOn(Clean)
        .DependsOn(SetManifestVersions)
        .DependsOn(Compile)
        .DependsOn(SetRelativeScripts)
        .DependsOn(GenerateAppConfig)
        .DependsOn(Test)
        .DependsOn(UpdateTokens)
        .Executes(() =>
        {
            var stagingDirectory = ArtifactsDirectory / "staging";
            EnsureCleanDirectory(stagingDirectory);

            // Resources
            Compress(RootDirectory / "resources", stagingDirectory / "resources.zip", f => (f.Name != "resources.zip.manifest"));

            // Symbols
            var moduleAssemblyName = Solution.GetProject("Module").GetProperty("AssemblyName");
            var symbolFiles = GlobFiles(RootDirectory, $"bin/Release/**/{moduleAssemblyName}.pdb");
            Helpers.AddFilesToZip(stagingDirectory / "symbols.zip", symbolFiles.ToArray());

            // Install files
            var installFiles = GlobFiles(RootDirectory, "LICENSE", "manifest.dnn", "ReleaseNotes.html");
            installFiles.ForEach(i => CopyFileToDirectory(i, stagingDirectory));

            // Libraries
            var manifest = GlobFiles(RootDirectory, "*.dnn").FirstOrDefault();
            var assemblies = GlobFiles(RootDirectory / "bin" / Configuration, "*.dll");
            var manifestAssemblies = Helpers.GetAssembliesFromManifest(manifest);
            assemblies.ForEach(assembly =>
            {
                var assemblyFile = new FileInfo(assembly);
                var assemblyIncludedInManifest = manifestAssemblies.Any(a => a == assemblyFile.Name);

                if (assemblyIncludedInManifest)
                {
                    CopyFileToDirectory(assembly, stagingDirectory / "bin", FileExistsPolicy.Overwrite);
                }
            });

            // Install package
            string fileName = new DirectoryInfo(RootDirectory).Name + "_";
            fileName += GitRepository != null && GitRepository.IsOnMainOrMasterBranch()
                ? GitVersion != null ? GitVersion.MajorMinorPatch : "0.1.0"
                : GitVersion != null ? GitVersion.SemVer : "0.1.0";
            fileName += "_install.zip";
            ZipFile.CreateFromDirectory(stagingDirectory, ArtifactsDirectory / fileName);
            DeleteDirectory(stagingDirectory);

            var artifact = ArtifactsDirectory / fileName;
            string hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(artifact))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }

            var hashMd = new StringBuilder();
            hashMd.AppendLine($"## MD5 Checksums");
            hashMd.AppendLine($"| File       | Checksum |");
            hashMd.AppendLine($"|------------|----------|");
            hashMd.AppendLine($"| {fileName} | {hash}   |");
            hashMd.AppendLine();
            File.WriteAllText(ArtifactsDirectory / "checksums.md", hashMd.ToString());

            // Open folder
            if (IsWin)
            {
                CopyFileToDirectory(ArtifactsDirectory / fileName, InstallDirectory, FileExistsPolicy.Overwrite);

                // Uncomment next line if you would like a package task to auto-open the package in explorer.
                // Process.Start("explorer.exe", ArtifactsDirectory);
            }

            Serilog.Log.Information("Packaging succeeded!");
        });

    Target Swagger => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var swaggerFile = DocsDirectory / "rest" / "rest.json";

            NSwagTasks.NSwagWebApiToOpenApi(c => c
                .AddAssembly(RootDirectory / "bin" / Configuration / "Dnn.Modules.SecurityCenter.dll")
                .SetInfoTitle("DNN Community DNN Security Center")
                .SetInfoVersion(GitVersion != null ? GitVersion.AssemblySemVer : "0.1.0")
                .SetProcessArgumentConfigurator(a => a.Add("/DefaultUrlTemplate:{{controller}}/{{action}}"))
                .SetOutput(swaggerFile));

            NSwagTasks.NSwagOpenApiToTypeScriptClient(c => c
                .SetInput(swaggerFile)
                .SetOutput(ClientServicesDirectory / "services.ts")
                .SetProcessArgumentConfigurator(c => c
                    .Add("/Template:Fetch")
                    .Add("/GenerateClientClasses:True")
                    .Add("/GenerateOptionalParameters")
                    .Add("/ClientBaseClass:ClientBase")
                    .Add("/ConfigurationClass:ConfigureRequest")
                    .Add("/UseTransformOptionsMethod:True")
                    .Add("/MarkOptionalProperties:True")
                    .Add($"/ExtensionCode:{ClientServicesDirectory / "client-base.ts"}")
                    .Add("/UseGetBaseUrlMethod:True")
                    .Add("/ProtectedMethods=ClientBase.getBaseUrl,ClientBase.transformOptions")
                    .Add("/UseAbortSignal:True")));
        });

    Target DeployGeneratedFiles => _ => _
        .OnlyWhenDynamic(() => GitRepository.IsOnDevelopBranch())
        .DependsOn(Test)
        .Executes(() =>
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("Nuke"));
            var authToken = new Credentials(GitHubToken);
            gitHubClient.Credentials = authToken;

            var repo = gitHubClient.Repository.Get(GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName()).Result;
            if (!repo.Fork)
            {
                Git($"config --global user.name '{GitRepository.GetGitHubOwner()}'");
                Git($"config --global user.email '{Helpers.GetManifestOwnerEmail(GlobFiles(RootDirectory / "*.dnn").FirstOrDefault())}'");
                Git($"remote set-url origin https://{GitRepository.GetGitHubOwner()}:{GitHubToken}@github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}.git");
                Git("status");
                Git("add docs -f");
                Git("add IntegrationTests/history -f");
                Git("add UnitTests/history -f");
                Git("add .github/badges -f");
                Git("status");
                Git("commit --allow-empty -m \"Commit latest generated files\""); // We allow an empty commit in case the last change did not affect the site.
                Git("status");
                Git("fetch origin");
                Git($"pull origin {GitRepository.Branch}");
                Git($"push --set-upstream origin {GitRepository.Branch}");
            }
        });
}
