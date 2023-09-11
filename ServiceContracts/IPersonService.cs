using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface IPersonService
	{
		/// <summary>
		/// Adds a new person into person into list of person
		/// </summary>
		/// <param name="request">Person object to add</param>
		/// <returns>A person object after added as personResponse object</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="AggregateException"></exception>
		Task<PersonResponse> AddPerson(PersonAddRequest? request);

		/// <summary>
		/// Return all persons
		/// </summary>
		/// <returns></returns>
		Task<List<PersonResponse>> GetAllPersons();

		Task<PersonResponse?> GetPersonByID(Guid? personID);

		Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

		/// <summary>
		/// Returns sorted lisy of persons
		/// </summary>
		/// <param name="allPersons"></param>
		/// <param name="sortBy"></param>
		/// <param name="sortOr"></param>
		/// <returns></returns>
		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOr);

		/// <summary>
		/// Updates the specified person details based on the given person ID
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update, including person id</param>
		/// <returns>Returns the person response object after updation</returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);


		/// <summary>
		/// Deletes a person based on the given person id
		/// </summary>
		/// <param name="PersonID">PersonID to delete</param>
		/// <returns>Returns true, if the deletion is successful; otherwise false</returns>
		Task<bool> DeletePerson(Guid? personID);
	}
}
