using System;
using MobileyeExam.Controllers;

namespace MobileyeExam.Entities
{
	public class CountWordsTaskEntity
	{
		public Guid Id { get; set; }
		public StringSourceTypeEnum SourceType { get; set; }
		public string Source { get; set; }

		public CountWordsTaskEntity(StringSourceTypeEnum sourceType, string source)
		{
			Id = Guid.NewGuid();
			SourceType = sourceType;
			Source = source;
		}
	}
}