using System.ComponentModel;

namespace SassCompiler
{
    internal class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        [Category("General")]
        [DisplayName("Transpiler method")]
        [Description("Specify which type of compilation you want.")]
        [DefaultValue(TranspilerMethod.SingleFileGenerator)]
        [TypeConverter(typeof(EnumConverter))]
        public TranspilerMethod Method { get; set; } = TranspilerMethod.SingleFileGenerator;
    }

    public enum TranspilerMethod
    {
        SingleFileGenerator,
        ManualFileNesting
    }
}
