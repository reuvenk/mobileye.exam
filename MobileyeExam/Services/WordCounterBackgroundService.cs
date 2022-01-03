using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MobileyeExam.Controllers;

namespace MobileyeExam.Services
{
	public class WordCounterBackgroundService : BackgroundService
	{
		private readonly ILogger<WordCounterBackgroundService> logger;
		private readonly IWordCountService wordCountService;
		private readonly IWordCountTaskQueue wordCountTaskQueue;

		public WordCounterBackgroundService(
			ILogger<WordCounterBackgroundService> logger, 
			IWordCountService wordCountService, 
			IWordCountTaskQueue wordCountTaskQueue)
		{
			this.logger = logger;
			this.wordCountService = wordCountService;
			this.wordCountTaskQueue = wordCountTaskQueue;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Started");
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(5000);
				try
				{
					if (!wordCountTaskQueue.TryDequeue(out var task))
					{
						continue;
					}

					long parsedWords;
					switch (task.SourceType)
					{
						case StringSourceTypeEnum.Url:
							parsedWords = await wordCountService.CountWordsFromUrlAsync(task.Source);
							break;
						case StringSourceTypeEnum.FilePath:
							parsedWords = await wordCountService.CountWordsFromFileAsync(task.Source);
							break;
						default:
							throw new Exception($"Unable to handle source type: {task.SourceType}");
					}
					logger.LogInformation($"Finished parsing {task.Source} parsed word count:{parsedWords}");

				}
				catch (Exception e)
				{
					logger.LogError($"Got an error during Word Counter processing: {e}");
				}
			}
		}
	}
}
