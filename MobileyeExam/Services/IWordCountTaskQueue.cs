using MobileyeExam.Entities;

namespace MobileyeExam.Services
{
	public interface IWordCountTaskQueue
	{
		int Enqueue(CountWordsTaskEntity countWordsTask);
		bool TryDequeue(out CountWordsTaskEntity result);
	};
}