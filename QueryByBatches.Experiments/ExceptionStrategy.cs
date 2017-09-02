namespace QueryByBatches.Experiments
{
	public interface IExceptionStrategy
	{
		bool ShouldThrowException(Query query);
	}

	public class FixedThresholdStrategy : IExceptionStrategy
	{
		public bool ShouldThrowException(Query query)
		{
			return query.Count > 100;
		}
	}
}
