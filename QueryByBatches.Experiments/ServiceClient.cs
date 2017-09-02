using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace QueryByBatches.Experiments
{
	public class ServiceClient
	{
		private readonly Func<IExceptionStrategy> _exceptionStrategyFactory;
		private IExceptionStrategy _exceptionStrategy;
		private readonly IEnumerable<int> _sequence;

		private IExceptionStrategy ExceptionStrategy => _exceptionStrategy
													 ?? (_exceptionStrategy = _exceptionStrategyFactory());
		public ServiceClient(Func<IExceptionStrategy> exceptionStrategyFactory, int defaultAmount = 10)
		{
			_exceptionStrategyFactory = exceptionStrategyFactory;
			_sequence = Enumerable.Range(1, defaultAmount);
		}

		public virtual IEnumerable<int> RetrieveMultiple(Query query)
		{
			if (ExceptionStrategy.ShouldThrowException(query))
				throw new WebException($"We've run into a timeout!");

			return _sequence.Skip(query.FirstIndex).Take(query.Count);
		}
	}
}
