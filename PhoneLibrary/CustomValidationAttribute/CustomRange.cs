using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

#pragma warning disable CS8765

namespace PhoneLibrary.CustomValidationAttribute
{
	internal class CustomRange : RangeAttribute
	{
		private Regex _regex;

		public CustomRange(int minimum, int maximum) : base(minimum, maximum)
		{
			_regex = new Regex(@"[0-9]");
		}

		public override bool IsValid(object value)
		{
			if (value == null) return false;
			if (value is string number)
			{
				int collectionNumber = _regex.Matches(number).Count;

				if (collectionNumber >= (int)Minimum && collectionNumber <= (int)Maximum) return true;
				return false;
			}
			return false;
		}
	}
}
