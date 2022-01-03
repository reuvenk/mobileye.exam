using System.IO;
using System.Threading.Tasks;
using MobileyeExam.Entities;

namespace MobileyeExam.Services
{
	public interface IWordCountService
	{
		//Task<long> CountWordsFromStringAsync(string str);
		Task<long> CountWords(StreamReader sr);
		Task<long> CountWordsFromUrlAsync(string url);
		Task<long> CountWordsFromFileAsync(string file);
		Task<long> GetWordStatisticsAsync(string word);
	}
}