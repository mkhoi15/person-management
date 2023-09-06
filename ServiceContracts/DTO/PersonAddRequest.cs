using ServiceContracts.Enums;
using System;
using Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// Act as DTO to insert new person 
	/// </summary>
	public class PersonAddRequest
	{
		public Guid PersonID { get; set; }
		[Required(ErrorMessage ="Person Name can't be blank")]
		public string? PersonName { get; set; }
		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage ="Please fill the blank with proper email")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		[DataType(DataType.Date)]
		public DateTime? DateOfBirth { get; set; }
		[Required(ErrorMessage = "Please select your Gender")]
		public GenderOptions? Gender { get; set; }
		[Required(ErrorMessage ="Please select a country")]
		public Guid CountryID { get; set; }
		public string? Address { get; set; }

		/// <summary>
		/// Convert Person into PersonAddRequest
		/// </summary>
		/// <returns></returns>
		public Person ToPerson()
		{
			return new Person()
			{
				PersonName = PersonName,
				Email = Email,
				DateOfBirth = DateOfBirth,
				Gender = Gender.ToString(),
				CountryID = CountryID,
				Address = Address
			};
		}
		

	}
}
