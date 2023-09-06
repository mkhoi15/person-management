using ServiceContracts.DTO;

namespace ServiceContracts
{
	public interface ICountriesServices
	{
		CountryResponse AddCountry(CountryAddRequest? request);

		/// <summary>
		/// Return all country from the list 
		/// </summary>
		/// <returns></returns>
		List<CountryResponse> GetAllCountry();

		/// <summary>
		/// Return a country base on CountryID
		/// </summary>
		/// <param name="countryID"></param>
		/// <returns></returns>
		CountryResponse? GetCountryByID(Guid? countryID);
	}
}