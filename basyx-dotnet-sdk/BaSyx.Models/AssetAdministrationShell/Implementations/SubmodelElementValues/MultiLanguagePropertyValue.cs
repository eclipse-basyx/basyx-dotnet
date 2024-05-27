namespace BaSyx.Models.AdminShell
{
	public class MultiLanguagePropertyValue : ValueScope
	{
		public override ModelType ModelType => ModelType.MultiLanguageProperty;

		public LangStringSet Value { get; set; } = new LangStringSet();

		public MultiLanguagePropertyValue() { }
		public MultiLanguagePropertyValue(LangStringSet value)
		{
			Value = value;
		}
	}
}
