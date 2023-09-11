using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entities.
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a persons obj to the data store.
        /// </summary>
        /// <param name="person">Persons obj to add</param>
        /// <returns>Returns the persons obj after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons from the data store.
        /// </summary>
        /// <returns>List of persons obj from the table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a persons obj base on the given personID.
        /// </summary>
        /// <param name="personID">PersonID (guid) to search</param>
        /// <returns>A person obj or null</returns>
        Task<Person?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns all person obj based on the given expression.
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person obj from the data store based on the given personID.
        /// </summary>
        /// <param name="personID">PersonID (guid) to search</param>
        /// <returns>Returns true if the deletion is successful otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid? personID);

        /// <summary>
        /// Updates a person obj in the data store.
        /// </summary>
        /// <param name="person">Person obj to update</param>
        /// <returns>Returns the updated person obj</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
