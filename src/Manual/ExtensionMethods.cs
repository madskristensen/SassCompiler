using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SassCompiler.Manual
{
    public static class ExtensionMethods
    {
        public static bool CheckOutItemSafely(this SourceControl scc, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (scc.IsItemUnderSCC(filePath) && !scc.IsItemCheckedOut(filePath))
            {
                return scc.CheckOutItem(filePath);
            }

            return false;
        }

        public static bool TryAddNestedFile(this ProjectItem parent, string nestedFilePath, out ProjectItem nestedItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            nestedItem = null;

            try
            {
                if (parent.ContainingProject != null && parent.DTE.Solution.FindProjectItem(nestedFilePath) == null)
                {
                    nestedItem = parent.ProjectItems.AddFromFile(nestedFilePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex);
            }

            return false;
        }
    }
}
