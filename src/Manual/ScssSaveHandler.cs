using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NUglify;
using SharpScss;

namespace SassCompiler.Manual
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("SCSS")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class ScssSaveHandler : IWpfTextViewCreationListener
    {
        private ITextDocument _doc;

        [Import]
        private ITextDocumentFactoryService DocService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (DocService.TryGetTextDocument(textView.TextBuffer, out _doc))
            {
                _doc.FileActionOccurred += OnSave;
                textView.Closed += TextViewClosed;
            }
        }


        private void OnSave(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                ITextSnapshotLine line = _doc.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(0);

                if (line.GetText().Contains("transpile"))
                {
                    ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                    {
                        await TranspileAsync(e.FilePath);

                    }).FileAndForget(nameof(ScssSaveHandler));
                }
            }
        }

        private async System.Threading.Tasks.Task TranspileAsync(string filePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            DTE2 dte = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<DTE, DTE2>();

            var options = new ScssOptions();
            options.IncludePaths.Add(Path.GetDirectoryName(filePath));
            ScssResult result = Scss.ConvertToCss(_doc.TextBuffer.CurrentSnapshot.GetText(), options);

            var cssPath = Path.ChangeExtension(filePath, ".css");
            dte.SourceControl.CheckOutItemSafely(cssPath);

            using (var writer = new StreamWriter(cssPath))
            {
                await writer.WriteAsync(result.Css);
            }

            ProjectItem scssItem = dte.Solution.FindProjectItem(filePath);
            ProjectItem cssItem = dte.Solution.FindProjectItem(cssPath);

            if (cssItem != null || scssItem.TryAddNestedFile(cssPath, out cssItem))
            {
                UglifyResult minified = Uglify.Css(result.Css);
                var minPath = Path.ChangeExtension(cssPath, ".min.css");

                dte.SourceControl.CheckOutItemSafely(minPath);

                using (var writer = new StreamWriter(minPath))
                {
                    await writer.WriteAsync(minified.Code);
                }

                cssItem.TryAddNestedFile(minPath, out _);
            }
        }

        private void TextViewClosed(object sender, System.EventArgs e)
        {
            var view = (IWpfTextView)sender;
            view.Closed -= TextViewClosed;

            _doc.FileActionOccurred -= OnSave;
        }
    }
}