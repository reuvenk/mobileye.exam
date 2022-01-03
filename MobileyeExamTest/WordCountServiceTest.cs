using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileyeExam.Repositories;
using MobileyeExam.Services;

namespace MobileyeExamTest
{
	[TestClass]
	public class WordCountServiceTest
	{
		[TestMethod]
		public async Task TestCountWordsFromStringAsync()
		{
			//Given
			IWordCountRepository repo = new InMemDbWordCountRepository();
			WordCountService wordCountService = new WordCountService(repo);

			////When
			await wordCountService.CountWordsFromStringAsync(
				"Hi! My name is (what?), my name is (who?), my name is Slim Shady");

			////Then
			Assert.AreEqual(3, await repo.GetCountAsync("name"));
		}

		[TestMethod]
		public async Task TestGetWordStatisticsAsync()
		{
			//Given
			IWordCountRepository repo = new InMemDbWordCountRepository();
			WordCountService wordCountService = new WordCountService(repo);

			////When
			await wordCountService.CountWordsFromStringAsync(
				"Hi! My name is (what?), my name is (who?), my name is Slim Shady");

			var statistics = await wordCountService.GetWordStatisticsAsync("name");

			////Then
			Assert.AreEqual(3, statistics);
		}
	}
}
