using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Runtime.InteropServices;
using System.Threading;

using Task = System.Threading.Tasks.Task;

namespace SassCompiler
{
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(PackageGuids.guidSassCompilerPackageString)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Extensions", Vsix.Name, 0, 0, true, 0, ProvidesLocalizedCategoryName = false)]
    [ProvideProfile(typeof(DialogPageProvider.General), "Extensions", Vsix.Name, 0, 0, true)]
    [ProvideCodeGenerator(typeof(SassTranspiler), SassTranspiler.Name, SassTranspiler.Description, true, RegisterCodeBase = true)]
    //[ProvideCodeGeneratorExtension(SassTranspiler.Name, ".scss")]
    //[ProvideCodeGeneratorExtension(SassTranspiler.Name, ".sass")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideUIContextRule(PackageGuids.guidSassUIRuleString,
        name: "Sass files",
        expression: "Sass",
        termNames: new[] { "Sass"},
        termValues: new[] { "HierSingleSelectionName:.s(a|c)ss$" })]
    public sealed class SassCompilerPackage : AsyncPackage
	{
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		    await CustomToolCommand.InitializeAsync(this);
		}
	}
}
