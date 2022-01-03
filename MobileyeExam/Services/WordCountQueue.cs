using System.Collections.Concurrent;
using MobileyeExam.Entities;

namespace MobileyeExam.Services
{
	public class WordCountQueue :  IWordCountTaskQueue
	{
		private ConcurrentQueue<CountWordsTaskEntity> countWordsFromUrlQueue = new ConcurrentQueue<CountWordsTaskEntity>();
		public int Enqueue(CountWordsTaskEntity countWordsTask)
		{
			countWordsFromUrlQueue.Enqueue(countWordsTask);
			return countWordsFromUrlQueue.Count;
		}

		public bool TryDequeue(out CountWordsTaskEntity result)
		{
			return countWordsFromUrlQueue.TryDequeue(out result);
		}
	}
}