using ServiceContracts.Enums;
using System;
using Entities;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// Represents DTO class that is used as return type of most method
	/// of Person Service
	/// </summary>
	public class PersonResponse
	{
		public Guid PersonID { get; set; }
		public string? PersonName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public Guid? CountryID { get; set; }
		public string? Country { get; set; }
		public string? Address { get; set; }
		public double? Age {  get; set; }

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;

			if (obj.GetType() != typeof(PersonResponse)) return false;

			PersonResponse person = (PersonResponse)obj;
			return PersonID == person.PersonID && PersonName == person.PersonName
				&& Email == person.Email && DateOfBirth == person.DateOfBirth
				&& Gender == person.Gender && CountryID == person.CountryID
				&& Address == person.Address;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"Person ID: {PersonID}, PersonName:{PersonName},Email: {Email}," +
				$"Date of Birth: ${DateOfBirth?.ToString("dd MMM yyyy")}, Gender:{Gender}," +
				$"Address: {Address}, Country: {Country} ";
				
		}

		public PersonUpdateRequest ToPersonUpdateRequest()
		{
			return new PersonUpdateRequest() 
			{
				PersonID = PersonID, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth,
				Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true), Address = Address,
				CountryID = CountryID
			};
		}

	}

	public static class PersonExtension
	{
		/// <summary>
		/// Convert Person object into PersonResponse object
		/// </summary>
		/// <param name="person"></param>
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse()
			{
				PersonID = person.PersonID,
				PersonName = person.PersonName,
				Email = person.Email,
				DateOfBirth = person.DateOfBirth,
				Gender = person.Gender,
				Address = person.Address,
				CountryID = person.CountryID,
				Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
			};
		}
	}

}
