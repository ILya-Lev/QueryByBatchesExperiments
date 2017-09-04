namespace QueryByBatches.Experiments
{
	public class Query
	{
		private int _count;
		public int TimesCountChanged { get; set; } = 0;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				TimesCountChanged++;
			}
		}

		public int FirstIndex { get; set; }
	}
}
