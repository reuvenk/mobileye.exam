using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileyeExam.Entities
{
	[Table("words", Schema = "mobileye")]
	public record WordEntity : IEquatable<WordEntity>, IComparable<WordEntity>
	{
		public Guid Id { get; init; }
		public string Name { get; init; }

		public long Count { get; init; }

		public WordEntity()
		{

		}

		public WordEntity(
			Guid id,
			string name,
			long count)
		{
			Id = id;
			Name = name;
			Count = count;
		}

		public int CompareTo(WordEntity? other)
		{
			// A null value means that this object is greater.
			if (other == null)
				return 1;

			else
				return Name.CompareTo(other.Name);
		}

	}
}
