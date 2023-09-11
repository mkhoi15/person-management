using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using RepositoryContracts;

namespace Services
{
	public class CountriesService : ICountriesServices
	{
		// private field 
		private readonly ICountriesRepository _countriesRepository;

		// contrusctor 
		public CountriesService(ICountriesRepository countriesRepository )
		{
			_countriesRepository = countriesRepository;
		}
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
		{
            List<Country> countries = await _countriesRepository.GetAllCountries();
            return countries
              .Select(country => country.ToCountryResponse()).ToList();
        }


		public async Task<CountryResponse?> GetCountryByID(Guid? countryID)
		{
			if (countryID == null) return null;

			Country? country_response_from_List = await _countriesRepository.GetCountryByID(countryID.Value);

			if(country_response_from_List == null) return null;

			return country_response_from_List.ToCountryResponse();
		}
	}
}