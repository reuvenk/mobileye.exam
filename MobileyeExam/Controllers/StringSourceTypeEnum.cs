using System.Text.Json.Serialization;

namespace MobileyeExam.Controllers
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum StringSourceTypeEnum
	{
		Body,
		Url,
		FilePath
	}
}