using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MobileyeExam.Entities;
using MobileyeExam.Repositories;

namespace MobileyeExam.Services
{
	public class WordCountService : IWordCountService
	{
		private readonly IWordCountRepository wordCountRepository;
		private const int MaxWordCountBuffer = 100;

		//This controls how many bytes to read at a time and send to the client
		const int BytesToRead = 10000000;
		private byte[] buffer = new Byte[BytesToRead];

		public WordCountService(IWordCountRepository wordCountRepository)
		{
			this.wordCountRepository = wordCountRepository;
		}

		public async Task<long> CountWordsFromStringAsync(string str)
		{
			using StreamReader sr = new StreamReader(GenerateStreamFromString(str));
			return await CountWords(sr);
			
		}

		public async Task<long> CountWordsFromFileAsync(string file)
		{
			long totalWords;
			// Create a stream for the file

			Stream stream = null;

			// The number of bytes read
			try
			{
				stream = new FileStream(file, FileMode.Open, FileAccess.Read);

				totalWords = await CountWord(stream);
			}
			finally
			{
				if (stream != null)
				{
					//Close the input stream
					stream.Close();
				}
			}

			return totalWords;
		}

		public async Task<long> CountWord(Stream stream)
		{
			long totalWords = 0;
			int length;
			do
			{
				// Verify that the client is connected.
				// Read data into the buffer.
				length = stream.Read(buffer, 0, BytesToRead);

				var streemReader = new StreamReader(new MemoryStream(buffer), Encoding.Default);

				totalWords += await this.CountWords(streemReader);
				// and write it out to the response's output stream

				//Clear the buffer
				buffer = new Byte[BytesToRead];
			} while (length > 0); //Repeat until no data is read

			return totalWords;
		}

		public async Task<long> CountWordsFromUrlAsync(string url)
		{
			long totalWords = 0;
			// Create a stream for the file
 
			Stream stream = null;

			// The number of bytes read
			try
			{
				//Create a WebRequest to get the file
				var fileReq = WebRequest.Create(url);

				//Create a response for this request
				var fileResp = await fileReq.GetResponseAsync();

				if (fileReq.ContentLength > 0)
					fileResp.ContentLength = fileReq.ContentLength;

				//Get the Stream returned from the response
				stream = fileResp.GetResponseStream();

				int length;
				do
				{
					// Verify that the client is connected.
						// Read data into the buffer.
						if (stream == null)
						{
							throw new Exception($"Unable to fetch file{url}");
						}
						length = await stream.ReadAsync(buffer, 0, BytesToRead);

						var streamReader = new StreamReader(new MemoryStream(buffer), Encoding.Default);

						totalWords += await this.CountWords(streamReader);
						// and write it out to the response's output stream

						//Clear the buffer
						buffer = new Byte[BytesToRead];
				} while (length > 0); //Repeat until no data is read
			}
			finally
			{
				if (stream != null)
				{
					//Close the input stream
					stream.Close();
				}
			}

			return totalWords;
		}

		public async Task<long> CountWords(StreamReader sr)
		{

			long totalWords = 0;
			Dictionary<string, int> wordCount = new Dictionary<string, int>();
			while (true) //After testing I noticed performance improved using a fix buffer read instead of line read
			{
				var line = await sr.ReadLineAsync();
				if (line == null)
				{
					//continue;
					break;
				}

				var lineWords = line.Split('@', ' ', '(', ')', '?', '!', ',', '.', ';', '\'');

				foreach (var word in lineWords)
				{
					if (String.IsNullOrEmpty(word))
					{
						continue;
					}

					if (wordCount.ContainsKey(word))
					{
						wordCount[word]++;
					}
					else
					{
						wordCount[word] = 1;
					}

					if (wordCount.Count == MaxWordCountBuffer)
					{
						totalWords += await UpdateWordRepository(wordCount);

						wordCount = new Dictionary<string, int>();
					}
				}
				totalWords += await UpdateWordRepository(wordCount);
			}

			return totalWords;
		}

		private async Task<long> UpdateWordRepository(Dictionary<string, int> wordCount)
		{
			long totalWords = 0;
			foreach (var wordCountPair in wordCount)
			{
				var wordEntity = new WordEntity(
					Guid.NewGuid(),
					wordCountPair.Key,
					wordCountPair.Value);
				await wordCountRepository.AddAsync(wordEntity);
				totalWords += wordEntity.Count;
			}

			return totalWords;
		}

		private Stream GenerateStreamFromString(string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}


		public async Task<long> GetWordStatisticsAsync(string word)
		{
			return await wordCountRepository.GetCountAsync(word);
		}
	}
}
