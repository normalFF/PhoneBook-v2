using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8765, CS8625

namespace PhoneLibrary.CustomValidationAttribute
{
	internal class CustomDateRange : RangeAttribute
	{
		public CustomDateRange(string minValue) : base(typeof(DataType), minValue, null)
		{
		}

		public override bool IsValid(object value)
		{
			if (value == null) return true;
			if (value is DateTime date)
			{
				if (date >= Convert.ToDateTime((string)Minimum) && date <= DateTime.Today) return true;
			}
			return false;
		}
	}
}
