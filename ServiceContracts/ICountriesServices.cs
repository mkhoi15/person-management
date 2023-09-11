using ServiceContracts.DTO;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface ICountriesServices
	{
		Task<CountryResponse> AddCountry(CountryAddRequest? request);

		/// <summary>
		/// Return all country from the list 
		/// </summary>
		/// <returns></returns>
		Task<List<CountryResponse>> GetAllCountries();

		/// <summary>
		/// Return a country base on CountryID
		/// </summary>
		/// <param name="countryID"></param>
		/// <returns></returns>
		Task<CountryResponse?> GetCountryByID(Guid? countryID);
	}
}