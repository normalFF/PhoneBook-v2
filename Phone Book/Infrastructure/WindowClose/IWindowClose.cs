using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phone_Book.Infrastructure.WindowClose
{
	internal interface IWindowClose
	{
		Action Close { get; set; }

		bool CanClose();
	}
}
