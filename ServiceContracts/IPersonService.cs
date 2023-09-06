using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
	public interface IPersonService
	{
		/// <summary>
		/// Adds a new person into person into list of person
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		PersonResponse AddPPerson(PersonAddRequest? request);

		/// <summary>
		/// Return all persons
		/// </summary>
		/// <returns></returns>
		List<PersonResponse> GetAllPersons();

		PersonResponse? GetPersonByID(Guid? personID);

		List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

		/// <summary>
		/// Returns sorted lisy of persons
		/// </summary>
		/// <param name="allPersons"></param>
		/// <param name="sortBy"></param>
		/// <param name="sortOr"></param>
		/// <returns></returns>
		List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOr);

		/// <summary>
		/// Updates the specified person details based on the given person ID
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update, including person id</param>
		/// <returns>Returns the person response object after updation</returns>
		PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);


		/// <summary>
		/// Deletes a person based on the given person id
		/// </summary>
		/// <param name="PersonID">PersonID to delete</param>
		/// <returns>Returns true, if the deletion is successful; otherwise false</returns>
		bool DeletePerson(Guid? personID);
	}
}
