using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MobileyeExam.Entities;
using MobileyeExam.Services;

namespace MobileyeExam.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WordController : ControllerBase
	{
		private readonly IWordCountService wordCountService;
		private readonly IWordCountTaskQueue wordCountQueue;
		public WordController(IWordCountService wordCountService, IWordCountTaskQueue wordCountQueue)
		{
			this.wordCountService = wordCountService;
			this.wordCountQueue = wordCountQueue;
		}

		[HttpPost("Count/{sourceType}")]
		public async Task<string> CountWords( 
			//[FromBody] string str, 
			StringSourceTypeEnum sourceType)
		{
			string result = "";
			switch (sourceType)
			{
				case StringSourceTypeEnum.Body:
					long totalWords = 0;
					using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
					{
						totalWords = await wordCountService.CountWords(reader);
					}
					result = $"Total words in string: {totalWords}";
					break;
				case StringSourceTypeEnum.Url:
				case StringSourceTypeEnum.FilePath:
					var str = await GetStringFromBody();
					var taskEntity = new CountWordsTaskEntity(sourceType, str);
					var placeInQ = wordCountQueue.Enqueue(new CountWordsTaskEntity(sourceType, str));
					result = $"place in Queue: {placeInQ}, request ID: {taskEntity.Id}";
					break;
				default:
					throw new Exception($"unsupported source Type: {sourceType}");
			}

			return result;
		}

		private async Task<string> GetStringFromBody()
		{
			using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
			{
				return await reader.ReadToEndAsync();
			}
		}

		[HttpGet("statistics/{word}")]
		public async Task<long> WordStatistics([FromRoute] string word)
		{
			return await wordCountService.GetWordStatisticsAsync(word);
		}
	}
}
