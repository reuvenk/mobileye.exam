using System.Threading.Tasks;
using MobileyeExam.Entities;

namespace MobileyeExam.Repositories
{
	public interface IWordCountRepository
	{
		Task<WordEntity> AddAsync(WordEntity word);
		Task<long> GetCountAsync(string word);
	}
}