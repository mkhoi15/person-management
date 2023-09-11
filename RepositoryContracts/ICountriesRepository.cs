using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entities.
    /// </summary>
   
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country to the data store.
        /// </summary>
        /// <param name="country">Country obj to add</param>
        /// <returns>Returns the country obj after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries from the data store.
        /// </summary>
        /// <returns>All the countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country by its ID.
        /// </summary>
        /// <param name="countryID">CountryID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByID(Guid? countryID);

        /// <summary>
        /// Returns a coutry obj based on the country name
        /// </summary>
        /// <param name="countryName">Country name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}