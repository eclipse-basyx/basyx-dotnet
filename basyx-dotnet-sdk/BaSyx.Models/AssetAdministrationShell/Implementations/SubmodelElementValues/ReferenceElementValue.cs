namespace BaSyx.Models.AdminShell
{
	public class ReferenceElementValue : ValueScope
	{
		public override ModelType ModelType => ModelType.ReferenceElement;

		public IReference Value { get; set; }

		public ReferenceElementValue() { }
		public ReferenceElementValue(IReference value)
		{
			Value = value;
		}
	}
}
