using System.Text;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NUglify;
using SharpScss;

namespace SassCompiler
{
    public class SassTranspiler : BaseCodeGeneratorWithSite
    {
        public const string Name = nameof(SassTranspiler);
        public const string Description = "Transpiles Sass/Scss files to CSS";

        public override string GetDefaultExtension() => ".css";

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            var options = new ScssOptions();

            ScssResult result = Scss.ConvertFileToCss(inputFileName, options);
            UglifyResult minified = Uglify.Css(result.Css);

            return Encoding.UTF8.GetBytes(minified.Code);
        }
    }
}
