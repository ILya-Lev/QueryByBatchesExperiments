using FluentAssertions;
using Moq;
using QueryByBatches.Experiments;
using System.Linq;
using Xunit;

namespace QueryByBatches.Tests
{
	public class QueryByBatchesTests
	{
		private IExceptionStrategy _exceptionStrategy;
		private Experiments.QueryByBatches _queryByBatches;
		private const int ItemsInStorage = 100;

		public QueryByBatchesTests()
		{
			var client = new ServiceClient(() => _exceptionStrategy, ItemsInStorage);
			_queryByBatches = new Experiments.QueryByBatches(client);
		}

		[Fact]
		public void RetrieveByBatches_NoExceptionsAliquotAmount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.IsAny<Query>())).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 4;
			var query = new Query { Count = 12 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(12);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_NoExceptionsNotAliquotAmount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.IsAny<Query>())).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 5;
			var query = new Query { Count = 12 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(12);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_ExceptionAtDoubleInitBatch_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count == 8))).Returns(true);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count != 8))).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 4;
			var query = new Query { Count = 21 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(21);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_ExceptionAtInitBatch_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count == 4))).Returns(true);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count != 4))).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 4;
			var query = new Query { Count = 21 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(21);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_NoExceptionsZeroCount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.IsAny<Query>())).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 10;
			var query = new Query { Count = 0 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(ItemsInStorage);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_NoExceptionsNotAliquotZeroCount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.IsAny<Query>())).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 19;
			var query = new Query { Count = 0 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(ItemsInStorage);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_ExceptionAtDoubleInitBatchZeroCount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count == 8))).Returns(true);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count != 8))).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 4;
			var query = new Query { Count = 0 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(ItemsInStorage);
			sequence.Should().BeInAscendingOrder();
		}

		[Fact]
		public void RetrieveByBatches_ExceptionAtInitBatchZeroCount_ReturnsExpectedAmount()
		{
			var strategySetup = new Mock<IExceptionStrategy>(MockBehavior.Strict);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count == 4))).Returns(true);
			strategySetup.Setup(str => str.ShouldThrowException(It.Is<Query>(q => q.Count != 4))).Returns(false);
			_exceptionStrategy = strategySetup.Object;

			var batchSize = 4;
			var query = new Query { Count = 0 };

			var sequence = _queryByBatches.RetrieveByBatches(query, batchSize).ToList();

			sequence.Should().HaveCount(ItemsInStorage);
			sequence.Should().BeInAscendingOrder();
		}
	}
}
