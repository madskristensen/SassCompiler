using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace SassCompiler
{
    internal sealed class CustomToolCommand
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            DTE2 dte = await package.GetServiceAsync<DTE, DTE2>();

            var cmdId = new CommandID(PackageGuids.guidSassCompilerPackageCmdSet, PackageIds.CustomToolCommandId);
            var cmd = new OleMenuCommand((s, e) => Execute(dte), cmdId, false);

            commandService.AddCommand(cmd);
        }

        private static void Execute(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ProjectItem item = dte.SelectedItems.Item(1).ProjectItem;

            item.Properties.Item("CustomTool").Value = SassTranspiler.Name;
        }
    }
}
