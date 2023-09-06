using ServiceContracts;
using System;
using System.Collections.Generic;
using Xunit;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Linq;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		// private field
		private readonly IPersonService _personService;
		private readonly ICountriesServices _countriesServices;
		private readonly ITestOutputHelper _testOutputHelper;

		// contructor
		public PersonServiceTest(ITestOutputHelper testOutputHelper)
		{
			_personService = new PersonService();
			_countriesServices = new CountriesService(false);
			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson

		// When we supply null value as PersonAddRequest, it should throw ArgumentNullException
		[Fact]
		public void AddPerson_NullPerson()
		{
			// Arrrange 
			PersonAddRequest? personRequest = null;

			//Act
			Assert.Throws<ArgumentNullException>(() =>
			{
				_personService.AddPPerson(personRequest);
			});
		}

		// When we supply null value as PersonAddRequest, it should throw ArgumentNullException
		[Fact]
		public void AddPerson_PersonNameIsNull()
		{
			// Arrrange 
			PersonAddRequest? personRequest = new PersonAddRequest() { PersonName = null};

			//Act
			Assert.Throws<ArgumentException>(() =>
			{
				_personService.AddPPerson(personRequest);
			});
		}

		// When we supply proper person detail, it should insert person into the persons list
		[Fact]
		public void AddPerson_ProperPersonDetail()
		{
			// Arrrange 
			PersonAddRequest? personRequest = new PersonAddRequest()
			{
				PersonName = "Tyler",
				Email = "person@email.com",
				CountryID = Guid.NewGuid(),
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
			};

			//Act
			PersonResponse person_response_from_add = _personService.AddPPerson(personRequest);
			List<PersonResponse> person_list =  _personService.GetAllPersons();
			// Assert
			Assert.True(person_response_from_add.PersonID != Guid.Empty);

			Assert.Contains(person_response_from_add, person_list);
		}

		#endregion

		#region GetPersonByID

		// If we supply null as personID, it should return null as PersonResponse
		[Fact]
		public void GetPersonByPersonID_NullPersonID()
		{
			// Arrange 
			Guid? personID = null;

			// Act 
			PersonResponse? person_response_from_get = _personService.GetPersonByID(personID);

			// Assert
			Assert.Null(person_response_from_get);
		}

		// If we supply a valid person id, it should return the valid person details as PersonResponse obj 
		[Fact]
		public void GetPersonByPersonID_WithPersonID()
		{
			// Arrange 
			CountryAddRequest country_request = new CountryAddRequest()
			{
				CountryName = "India"
			};
			CountryResponse country_response = _countriesServices.AddCountry(country_request);

			PersonAddRequest person_request = new PersonAddRequest()
			{
				PersonName = "Pyke",
				Email = "person@email.com",
				CountryID = country_response.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "Tay Ninh Viet Nam"
			};

			PersonResponse person_response_from_add= _personService.AddPPerson(person_request);
			PersonResponse? person_response_from_get = _personService.GetPersonByID(person_response_from_add.PersonID);

			// Assert
			Assert.Equal(person_response_from_add, person_response_from_get);
		}
		#endregion

		#region GetAllPersons

		// The GetAllPersons() should return an empty list by default
		[Fact]
		public void GetAllPersons_EmptyList()
		{
			// Act 
			List<PersonResponse> person_from_get = _personService.GetAllPersons();

			// Assert
			Assert.Empty(person_from_get);
		}

		// First we add few person, and then when we call GetAllPersons(), it should return the same
		// person that were added
		[Fact]
		public void GetAllPersons_AddFewPersons() 
		{
			// Arrange 
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "Malaysia" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Singapore" };

			CountryResponse countryResponse1 = _countriesServices.AddCountry(countryAddRequest1);
			CountryResponse countryResponse2 = _countriesServices.AddCountry(countryAddRequest2);

			PersonAddRequest person_request1 = new PersonAddRequest()
			{
				PersonName = "Kyle",
				Email = "person@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "dong nai",
			};

			PersonAddRequest person_request2 = new PersonAddRequest()
			{
				PersonName = "Midd",
				Email = "person@email.com",
				CountryID = countryResponse2.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "qwdqwdqwdqwd",
			};

			PersonAddRequest person_request3 = new PersonAddRequest()
			{
				PersonName = "Diii",
				Email = "person333@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "Vung tau",
			};

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request1,person_request2,person_request3
			};

			List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

			foreach(PersonAddRequest person_request  in person_requests)
			{
				PersonResponse personResponse = _personService.AddPPerson(person_request);
				person_responses_from_add.Add(personResponse);
			}

			// Print Person_response_list_from Add 
			_testOutputHelper.WriteLine("Expected:");
			foreach(PersonResponse person_responseFromAdd in person_responses_from_add)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			// Act 
			List<PersonResponse> person_responses_from_get = _personService.GetAllPersons();

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_get)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			// Assert
			foreach (PersonResponse personResponsefromAdd in person_responses_from_add) 
			{
				Assert.Contains(personResponsefromAdd, person_responses_from_get);
			}
		}
		#endregion

		#region GetFilteredPersons
		// If the search text is empty and search by is "PersonName", it should return all person
		[Fact]
		public void GetFilteredPersons_EmptySearchText()
		{
			// Arrange 
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "Malaysia" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Singapore" };

			CountryResponse countryResponse1 = _countriesServices.AddCountry(countryAddRequest1);
			CountryResponse countryResponse2 = _countriesServices.AddCountry(countryAddRequest2);

			PersonAddRequest person_request1 = new PersonAddRequest()
			{
				PersonName = "Kyle",
				Email = "person@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "dong nai",
			};

			PersonAddRequest person_request2 = new PersonAddRequest()
			{
				PersonName = "Midd",
				Email = "person@email.com",
				CountryID = countryResponse2.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "qwdqwdqwdqwd",
			};

			PersonAddRequest person_request3 = new PersonAddRequest()
			{
				PersonName = "Diii",
				Email = "person333@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "Vung tau",
			};

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request1,person_request2,person_request3
			};

			List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse personResponse = _personService.AddPPerson(person_request);
				person_responses_from_add.Add(personResponse);
			}

			// Print Person_response_list_from Add 
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_responseFromAdd in person_responses_from_add)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			// Act 
			List<PersonResponse> person_responses_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName),"");

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_search)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			// Assert
			foreach (PersonResponse personResponsefromAdd in person_responses_from_add)
			{
				Assert.Contains(personResponsefromAdd, person_responses_from_search);
			}
		}

		// First we add few person; then we will search based on person name with some search string
		[Fact]
		public void GetFilteredPersons_SearchByPersonName()
		{
			// Arrange 
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "Malaysia" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Singapore" };

			CountryResponse countryResponse1 = _countriesServices.AddCountry(countryAddRequest1);
			CountryResponse countryResponse2 = _countriesServices.AddCountry(countryAddRequest2);

			PersonAddRequest person_request1 = new PersonAddRequest()
			{
				PersonName = "Kyle",
				Email = "person@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "dong nai",
			};

			PersonAddRequest person_request2 = new PersonAddRequest()
			{
				PersonName = "Midd",
				Email = "person@email.com",
				CountryID = countryResponse2.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "qwdqwdqwdqwd",
			};

			PersonAddRequest person_request3 = new PersonAddRequest()
			{
				PersonName = "Diii",
				Email = "person333@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "Vung tau",
			};

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request1,person_request2,person_request3
			};

			List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse personResponse = _personService.AddPPerson(person_request);
				person_responses_from_add.Add(personResponse);
			}

			// Print Person_response_list_from Add 
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_responseFromAdd in person_responses_from_add)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			// Act 
			List<PersonResponse> person_responses_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "i");

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_search)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			// Assert
			foreach (PersonResponse personResponsefromAdd in person_responses_from_add)
			{
				if(personResponsefromAdd.PersonName != null)
				{
					if (personResponsefromAdd.PersonName.Contains("i", StringComparison.OrdinalIgnoreCase))
					{
						Assert.Contains(personResponsefromAdd, person_responses_from_search);

					}
				}
				
			}

			
		}

		#endregion

		#region GetSortedPersons

		// If the search text is empty and search by is "PersonName", it should return all person
		[Fact]
		public void GetSortedPersons_SortedByPersonName()
		{
			// Arrange 
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "Malaysia" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Singapore" };

			CountryResponse countryResponse1 = _countriesServices.AddCountry(countryAddRequest1);
			CountryResponse countryResponse2 = _countriesServices.AddCountry(countryAddRequest2);

			PersonAddRequest person_request1 = new PersonAddRequest()
			{
				PersonName = "Kyle",
				Email = "person@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "dong nai",
			};

			PersonAddRequest person_request2 = new PersonAddRequest()
			{
				PersonName = "Midd",
				Email = "person@email.com",
				CountryID = countryResponse2.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "qwdqwdqwdqwd",
			};

			PersonAddRequest person_request3 = new PersonAddRequest()
			{
				PersonName = "Diii",
				Email = "person333@email.com",
				CountryID = countryResponse1.CountryID,
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2003-01-01"),
				Address = "Vung tau",
			};

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request1,person_request2,person_request3
			};

			List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse personResponse = _personService.AddPPerson(person_request);
				person_responses_from_add.Add(personResponse);
			}

			// Print Person_response_list_from Add 
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_responseFromAdd in person_responses_from_add)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			List<PersonResponse> allPersons = _personService.GetAllPersons();

			// Act 
			List<PersonResponse> person_responses_from_sorted = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_sorted)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			person_responses_from_add = person_responses_from_add.OrderByDescending(temp => temp.PersonName).ToList();

			// Assert
			for (int i = 0; i < person_responses_from_add.Count; i++)
			{
				Assert.Equal(person_responses_from_add[i], person_responses_from_sorted[i]);
			}
		}

		#endregion

		#region UpdatePerson

		//When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
		[Fact]
		public void UpdatePerson_NullPerson()
		{
			//Arrange
			PersonUpdateRequest? person_update_request = null;

			//Assert
			Assert.Throws<ArgumentNullException>(() => {
				//Act
				_personService.UpdatePerson(person_update_request);
			});
		}


		//When we supply invalid person id, it should throw ArgumentException
		[Fact]
		public void UpdatePerson_InvalidPersonID()
		{
			//Arrange
			PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

			//Assert
			Assert.Throws<ArgumentException>(() => {
				//Act
				_personService.UpdatePerson(person_update_request);
			});
		}


		//When PersonName is null, it should throw ArgumentException
		[Fact]
		public void UpdatePerson_PersonNameIsNull()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
			CountryResponse country_response_from_add = _countriesServices.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Email = "john@example.com", Address = "address...", Gender = GenderOptions.Male };

			PersonResponse person_response_from_add = _personService.AddPPerson(person_add_request);

			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = null;


			//Assert
			Assert.Throws<ArgumentException>(() => {
				//Act
				_personService.UpdatePerson(person_update_request);
			});

		}


		//First, add a new person and try to update the person name and email
		[Fact]
		public void UpdatePerson_PersonFullDetailsUpdation()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
			CountryResponse country_response_from_add = _countriesServices.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.Male};

			PersonResponse person_response_from_add = _personService.AddPPerson(person_add_request);

			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = "William";
			person_update_request.Email = "william@example.com";

			//Act
			PersonResponse person_response_from_update = _personService.UpdatePerson(person_update_request);

			PersonResponse? person_response_from_get = _personService.GetPersonByID(person_response_from_update.PersonID);

			//Assert
			Assert.Equal(person_response_from_get, person_response_from_update);

		}

		#endregion

		#region DeletePerson

		//If you supply an valid PersonID, it should return true
		[Fact]
		public void DeletePerson_ValidPersonID()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
			CountryResponse country_response_from_add = _countriesServices.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryID = country_response_from_add.CountryID, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male};

			PersonResponse person_response_from_add = _personService.AddPPerson(person_add_request);


			//Act
			bool isDeleted = _personService.DeletePerson(person_response_from_add.PersonID);

			//Assert
			Assert.True(isDeleted);
		}


		//If you supply an invalid PersonID, it should return false
		[Fact]
		public void DeletePerson_InvalidPersonID()
		{
			//Act
			bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

			//Assert
			Assert.False(isDeleted);
		}

		#endregion
	}
}
