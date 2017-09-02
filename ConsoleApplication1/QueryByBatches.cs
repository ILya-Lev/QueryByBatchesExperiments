using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace QueryByBatchesExperiments
{
	public class QueryByBatches
	{
		private readonly ServiceClient _client;

		public QueryByBatches(ServiceClient client)
		{
			_client = client;
		}

		/// <summary>
		/// consider splitting the implementation into 2 private methods - one for initial query.Count == 0 
		/// another - != 0
		/// </summary>
		/// <param name="query"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
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
		private IList<int> RetieveBatch(Query query, ref int previousSuccessSize, ref int previousFailSize)
		{
			while (true)
			{
				try
				{
					var batch = _client.RetrieveMultiple(query).ToList();

					previousSuccessSize = query.Count;
					if (previousFailSize == 0)
						query.Count *= 2;
					else
						query.Count = (previousFailSize + query.Count) / 2;
					return batch;
				}
				catch (WebException webException) when (webException.Message.Contains("timeout"))
				{
					Console.WriteLine(webException);

					previousFailSize = query.Count;
					if (query.Count < 2)
						throw;

					if (previousSuccessSize == 0)
						query.Count /= 2;
					else
						query.Count = (query.Count + previousSuccessSize) / 2;
				}
			}
		}
	}
}
