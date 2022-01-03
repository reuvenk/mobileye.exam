using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileyeExam.Entities;


namespace MobileyeExam.Repositories
{


	public class InMemDbWordCountRepository : IWordCountRepository
	{
		//Simulate a DB with an index on the Word string 
		//each row would have an ID (uniq), a word(Non Uniq but indexed) & a count 
		//Done using a simple list (For faster response on getCount ) that allows duplicated keys
		private readonly List<WordEntity> wordCount = new ();

		public async Task<WordEntity> AddAsync(WordEntity word)
		{
			await Task.Run(() =>
			{
				wordCount.Add(word);
			});
			return word;
		}

		public async Task<long> GetCountAsync(string word)
		{
			long totalWordCount = 0;
			await Task.Run(() =>
			{
				var words = wordCount
					.Where(w => w.Name == word)
					.ToList();

				foreach (var wordEntity in words)
				{
					totalWordCount += wordEntity.Count;
				}
			});
			return totalWordCount;
		}
	}
}
