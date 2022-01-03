using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MobileyeExam.Controllers;
using MobileyeExam.Entities;

namespace MobileyeExam.Services
{
	public class WordCounterBackgroundService : BackgroundService
	{
		private ILogger<WordCounterBackgroundService> logger;
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
					CountWordsTaskEntity task;
					if (!wordCountTaskQueue.TryDequeue(out task))
					{
						continue;
					}

					if (task.SourceType == StringSourceTypeEnum.Url)
					{
						long parsedWords = await wordCountService.CountWordsFromUrlAsync(task.Source);
					}else if (task.SourceType == StringSourceTypeEnum.FilePath)
					{
						long parsedWords = await wordCountService.CountWordsFromFileAsync(task.Source);
					}
					logger.LogInformation($"Finished parsing {task.Source}");

				}
				catch (Exception e)
				{
					logger.LogError($"Got an error during Word Counter processing: {e}");
				}
			}
		}
	}
}
