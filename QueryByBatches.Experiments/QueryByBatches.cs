using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace QueryByBatches.Experiments
{
	public class QueryByBatches
	{
		private readonly ServiceClient _client;
		//private static readonly double factor = (Math.Sqrt(5) + 1) / 2;
		private static readonly double factor = 2;

		public QueryByBatches(ServiceClient client)
		{
			_client = client;
		}

		/// <summary>
		/// consider splitting the implementation into 2 private methods - one for initial query.Count == 0 
		/// another - != 0
		/// </summary>
		public IEnumerable<int> RetrieveByBatches(Query query, int batchSize)
		{
			var expectedTotal = query.Count;
			query.Count = batchSize;
			var alreadyGot = 0;

			int previousSuccessSize = 0, previousFailSize = 0;

			while (expectedTotal == 0 || alreadyGot < expectedTotal)
			{
				var batch = RetieveBatch(query, ref previousSuccessSize, ref previousFailSize);

				for (int i = 0; i < batch.Count && (expectedTotal == 0 || alreadyGot + i < expectedTotal); i++)
				{
					yield return batch[i];
				}

				alreadyGot += batch.Count;
				query.FirstIndex += batch.Count;

				if (batch.Count == 0)
					yield break;
			}
		}

		/// <summary> binary search of optimal batch size </summary>
		//private IList<int> RetieveBatch(Query query, ref int step, ref int previousFailSize)
		//{
		//	if (step == 0)
		//		step = query.Count;
		//	while (true)
		//	{
		//		try
		//		{
		//			var batch = _client.RetrieveMultiple(query).ToList();

		//			step = step < 0 ? -step / 2 : step / 2;
		//			query.Count += step;
		//			return batch;
		//		}
		//		catch (WebException webException) when (webException.Message.Contains("timeout"))
		//		{
		//			Console.WriteLine(webException);

		//			if (query.Count < 2)
		//				throw;

		//			step = step > 0 ? -step / 2 : step / 2;
		//			query.Count += step;
		//		}
		//	}
		//}
		private IList<int> RetieveBatch(Query query, ref int previousSuccessSize, ref int previousFailSize)
		{
			while (true)
			{
				try
				{
					var batch = _client.RetrieveMultiple(query).ToList();

					previousSuccessSize = query.Count;
					query.Count = IncreaseBatchSize(previousFailSize, query.Count);
					return batch;
				}
				catch (WebException webException) when (webException.Message.Contains("timeout"))
				{
					Console.WriteLine(webException);

					previousFailSize = query.Count;
					if (query.Count < 2)
						throw;

					if (previousSuccessSize >= query.Count)
						query.Count = DecreaseBatchSize(0, query.Count);
					else
						query.Count = DecreaseBatchSize(previousSuccessSize, query.Count);
				}
			}
		}

		int IncreaseBatchSize(int upperBound, int currentBatchSize)
		{
			if (upperBound == 0)
				return (int) (currentBatchSize * factor);
			return (int) ((currentBatchSize * (factor - 1) + upperBound) / factor);
		}

		int DecreaseBatchSize(int lowerBound, int currentBatchSize)
		{
			return (int) ((lowerBound * (factor - 1) + currentBatchSize) / factor);
		}
	}
}
