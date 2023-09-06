using System;
using Entities;
namespace ServiceContracts.DTO
{
	public class CountryResponse
	{
		public Guid CountryID { get; set; }
		public string? CountryName { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;

			if(obj.GetType() != typeof(CountryResponse)) return false; 

			CountryResponse compare_obj = (CountryResponse)obj;

			return (this.CountryID ==  compare_obj.CountryID && this.CountryName == compare_obj.CountryName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}



	public static class CountryExtensions
	{
		//Converts from Country object to CountryResponse object
		public static CountryResponse ToCountryResponse(this Country country)
		{
			return new CountryResponse() { CountryID = country.CountryID, CountryName = country.CountryName };
		}
	}
}


