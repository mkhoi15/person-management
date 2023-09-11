﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using RepositoryContracts;

namespace Services
{
	public class PersonService : IPersonService
	{
		// private filed
		private readonly IPersonsRepository _personsRepository;
		
        // Contrsuctor
        public PersonService(IPersonsRepository personsRepository)
		{
			_personsRepository = personsRepository;
		}

		public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
		{
			if (request == null) throw new ArgumentNullException(nameof(PersonAddRequest));

			//if(string.IsNullOrEmpty(request.PersonName)) throw new ArgumentException(nameof(request.PersonName));	
			// Model Validation
			ValidationHelper.ModelValidation(request);

			// Convert personAddrequet into Person type 
			Person person = request.ToPerson();

			// Generate new PersonID 
			person.PersonID = Guid.NewGuid();

			// Add person object to persons list 
			await _personsRepository.AddPerson(person);

			//_countriesRepository.sp_InsertPerson(person);

			// Convert the Person obj into PersonResponse obj

			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetAllPersons()
		{
			var persons = await _personsRepository.GetAllPersons();

			return persons.Select(temp => temp.ToPersonResponse()).ToList();
			//return _countriesRepository.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
		}
		public async Task<PersonResponse?> GetPersonByID(Guid? personID)
		{
			if (personID == null) return null;

			Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
				
			if(person == null) return null;

			return person.ToPersonResponse();
		}

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryID) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                await _personsRepository.GetFilteredPersons(temp =>
                temp.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			if (string.IsNullOrEmpty(sortBy))
				return allPersons;

			List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
			{
				(nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

				(nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

				(nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

				(nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				_ => allPersons
			};

			return sortedPersons;
		}

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			if (personUpdateRequest == null)
				throw new ArgumentNullException(nameof(Person));

			//validation
			ValidationHelper.ModelValidation(personUpdateRequest);

			//get matching person object to update
			Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
			if (matchingPerson == null)
			{
				throw new ArgumentException("Given person id doesn't exist");
			}

			//update all details
			matchingPerson.PersonName = personUpdateRequest.PersonName;
			matchingPerson.Email = personUpdateRequest.Email;
			matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
			matchingPerson.Gender = personUpdateRequest.Gender.ToString();
			matchingPerson.CountryID = personUpdateRequest.CountryID;
			matchingPerson.Address = personUpdateRequest.Address;

			await _personsRepository.UpdatePerson(matchingPerson); // Update

			return matchingPerson.ToPersonResponse();
		}

		public async Task<bool> DeletePerson(Guid? personID)
		{
			if (personID == null)
			{
				throw new ArgumentNullException(nameof(personID));
			}

			Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
			if (person == null)
				return false;

			await _personsRepository.DeletePersonByPersonID(personID.Value);
			
			return true;
		}
	}
}
