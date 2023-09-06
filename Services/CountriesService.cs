using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesService : ICountriesServices
	{
		// private field 
		private readonly List<Country> _countries;

		// contrusctor 
		public CountriesService(bool initialize = true)
		{
			_countries = new List<Country>();
			if (initialize)
			{
				_countries.AddRange(new List<Country>() {
		new Country() {  CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA" },

		new Country() { CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

		new Country() { CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

		new Country() { CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "India" },

		new Country() { CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
		});
			}
		}
		public CountryResponse AddCountry(CountryAddRequest? request)
		{
			// Validation: countryAddRequest parameter can't be null 
			if (request == null) throw new ArgumentNullException(nameof(request));

			//Validation: countryName can't be null
			if(request.CountryName == null) throw new ArgumentException(nameof(request.CountryName));

			// Validation: CountryName can't be duplicated 
			if(_countries.Where(temp => temp.CountryName == request.CountryName).Count() > 0)
			{
				throw new ArgumentException("Given country name that already exsits");
			}

			// convert object from CountryAddRequest into Country type 
			Country country = request.ToCountry();

			// generate countryID
			country.CountryID = Guid.NewGuid();

			// add country object into _countries list 
			_countries.Add(country);

			return country.ToCountryResponse();
		}

		public List<CountryResponse> GetAllCountry()
		{
			return _countries.Select(country => country.ToCountryResponse()).ToList();
		}


		public CountryResponse? GetCountryByID(Guid? countryID)
		{
			if (countryID == null) return null;

			Country? country_response_from_List = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

			if(country_response_from_List == null) return null;

			return country_response_from_List.ToCountryResponse();
		}
	}
}