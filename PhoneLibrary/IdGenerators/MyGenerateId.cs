namespace PhoneLibrary.IdGenerators
{
	internal class MyGenerateId : IGenerateId
	{
		private IPhoneBook book;
		private int idDiapason;

		public MyGenerateId(IPhoneBook book)
		{
			this.book = book;
			idDiapason = book.GetAbonents().Count() == 0 ? 100 : (int)book.GetAbonents().Select(i => i.Id).Max();
		}

		public int GetId()
		{
			for (int i = 0; i <= idDiapason; i++)
			{
				if (!book.GetAbonents().Select(i => i.Id).Contains(i)) return i;
			}

			while (true)
			{
				idDiapason++;
				if (!book.GetAbonents().Select(i => i.Id).Contains(idDiapason)) return idDiapason;
			}
		}
	}
}
