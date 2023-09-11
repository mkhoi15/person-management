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
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using RepositoryContracts;
using System.Linq.Expressions;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		// private field
		private readonly IPersonService _personService;
		private readonly ICountriesServices _countriesServices;

		private readonly Mock<IPersonsRepository> _personsRepositoryMock;
		private readonly IPersonsRepository _personsRepository;

		private readonly ITestOutputHelper _testOutputHelper;
		private readonly IFixture _fixture;

		// contructor
		public PersonServiceTest(ITestOutputHelper testOutputHelper)
		{
			_fixture = new Fixture();
			_personsRepositoryMock = new Mock<IPersonsRepository>();
			_personsRepository = _personsRepositoryMock.Object;

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

			// Create mock for DbContext
            DbContextMock<ApplicationDbContext> dbContextMock =
                new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

			// Acess Mock DbContext Object
            var dbContext = dbContextMock.Object;

			// Create mock for DbSet
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
			dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesServices = new CountriesService(null);

            _personService = new PersonService(_personsRepository);
		
			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson

		// When we supply null value as PersonAddRequest, it should throw ArgumentNullException
		[Fact]
		public async Task AddPerson_NullPerson_ToBeArgumentNullException()
		{
			// Arrrange 
			PersonAddRequest? personRequest = null;

			//Act
			Func<Task> action = async () =>
			{
				await _personService.AddPerson(personRequest);
			};

			await action.Should().ThrowAsync<ArgumentNullException>();
		}

		// When we supply null value as PersonAddRequest, it should throw ArgumentNullException
		[Fact]
		public async void AddPerson_PersonNameIsNull_ToBeArgumentException()
		{
			// Arrrange 
			PersonAddRequest? personRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

			Person person = personRequest.ToPerson();

			// When we supply any argument value to the AddPerson method, it should return the same return value
			_personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

			//Assert
			Func<Task> action = async () =>
			{
				//Act
				await _personService.AddPerson(personRequest);
			};

			await action.Should().ThrowAsync<ArgumentException>();
		}

		// When we supply proper person detail, it should insert person into the persons list
		[Fact]
		public async Task AddPerson_FullPersonDetail_ToBeSuccessful()
		{
			// Arrrange 
			PersonAddRequest? personRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

			Person person = personRequest.ToPerson();
			PersonResponse person_response_expected = person.ToPersonResponse();

			// If we supply any argument value to the AddPerson method, it should return the same return value
			_personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

			//Act
			PersonResponse person_response_from_add = await _personService.AddPerson(personRequest);
			person_response_expected.PersonID = person_response_from_add.PersonID;
            // Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

			person_response_from_add.Should().Be(person_response_expected);
		}

		#endregion

		#region GetPersonByID

		// If we supply null as personID, it should return null as PersonResponse
		[Fact]
		public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
		{
			// Arrange 
			Guid? personID = null;

			// Act 
			PersonResponse? person_response_from_get = await _personService.GetPersonByID(personID);

			// Assert
			//Assert.Null(person_response_from_get);
			person_response_from_get.Should().BeNull();
		}

		// If we supply a valid person id, it should return the valid person details as PersonResponse obj 
		[Fact]
		public async Task GetPersonByPersonID_WithPersonID_ToBeSucessful()
		{
			// Arrange 
			Person person = _fixture.Build<Person>().With(temp => temp.Email, "testEmail@gmail.com").With(temp => temp.Country, null as Country).Create();
			PersonResponse person_response_expected = person.ToPersonResponse();

			_personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

			// Act
			PersonResponse? person_response_from_get = await _personService.GetPersonByID(person.PersonID);

			// Assert
			person_response_from_get.Should().Be(person_response_expected);
		}
		#endregion

		#region GetAllPersons

		// The GetAllPersons() should return an empty list by default
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{

			// Arrange
			var persons = new List<Person>();
			_personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

			// Act 
			List<PersonResponse> person_from_get = await _personService.GetAllPersons();

			// Assert
			//Assert.Empty(person_from_get);
			person_from_get.Should().BeEmpty();
		}

		// First we add few person, and then when we call GetAllPersons(), it should return the same
		// person that were added
		[Fact]
		public async void GetAllPersons_AddFewPersons_ToBeSuccessful() 
		{
			// Arrange 
			List<Person> persons = new List<Person>()
			{
				_fixture.Build<Person>().With(temp => temp.Email, "sample1@gmail.com")
				.With(temp => temp.Country, null as Country)
				.Create(),

				_fixture.Build<Person>().With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

				_fixture.Build<Person>().With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
			};

			List<PersonResponse> person_responses_from_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

			// Print Person_response_list_from Expected
			_testOutputHelper.WriteLine("Expected:");
			foreach(PersonResponse person_responseFromAdd in person_responses_from_expected)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			_personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

			// Act 
			List<PersonResponse> person_responses_from_get = await _personService.GetAllPersons();

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_get)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			// Assert
			/*foreach (PersonResponse personResponsefromAdd in person_responses_from_expected) 
			{
                Assert.Contains(personResponsefromAdd, person_responses_from_get);
			}*/
            person_responses_from_get.Should().BeEquivalentTo(person_responses_from_expected);
        }
		#endregion

		#region GetFilteredPersons
		// If the search text is empty and search by is "PersonName", it should return all person
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
		{
            // Arrange 
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "sample1@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_responses_from_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

			// Print Person_response_list_from Expected
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_expected in person_responses_from_expected)
			{
				_testOutputHelper.WriteLine(person_expected.ToString());
			}

			_personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

			// Act 
			List<PersonResponse> person_responses_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName),"sa");

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_search)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			// Assert
			/*foreach (PersonResponse personResponsefromAdd in person_responses_from_expected)
			{
				Assert.Contains(personResponsefromAdd, person_responses_from_search);
			}*/
			person_responses_from_search.Should().BeEquivalentTo(person_responses_from_expected);
		}

		// First we add few person; then we will search based on person name with some search string
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
		{
            // Arrange 
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "sample1@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_responses_from_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // Print Person_response_list_from Expected
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_expected in person_responses_from_expected)
            {
                _testOutputHelper.WriteLine(person_expected.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            // Act 
            List<PersonResponse> person_responses_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print Person_response_list_from Get 
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_responseFromGet in person_responses_from_search)
            {
                _testOutputHelper.WriteLine(person_responseFromGet.ToString());
            }

            // Assert
            /*foreach (PersonResponse personResponsefromAdd in person_responses_from_expected)
			{
				Assert.Contains(personResponsefromAdd, person_responses_from_search);
			}*/
            person_responses_from_search.Should().BeEquivalentTo(person_responses_from_expected);


        }

		#endregion

		#region GetSortedPersons

		// If the search text is empty and search by is "PersonName", it should return all person
		[Fact]
		public async Task GetSortedPersons_SortedByPersonName()
		{
            // Arrange 
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesServices.AddCountry(countryAddRequest2);

            PersonAddRequest person_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email1@gmail.com")
                .With(temp => temp.PersonName, "Ange")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest person_request2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "email2@gmail.com")
                .With(temp => temp.PersonName, "Olivia")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest person_request3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "email3@gmail.com")
                .With(temp => temp.PersonName, "Sarah")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new()
			{
				person_request1,person_request2,person_request3
			};

			List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse personResponse = await _personService.AddPerson(person_request);
				person_responses_from_add.Add(personResponse);
			}

			// Print Person_response_list_from Add 
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_responseFromAdd in person_responses_from_add)
			{
				_testOutputHelper.WriteLine(person_responseFromAdd.ToString());
			}

			List<PersonResponse> allPersons = await _personService.GetAllPersons();

			// Act 
			List<PersonResponse> person_responses_from_sorted = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

			// Print Person_response_list_from Get 
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_responseFromGet in person_responses_from_sorted)
			{
				_testOutputHelper.WriteLine(person_responseFromGet.ToString());
			}

			/*person_responses_from_expected = person_responses_from_expected.OrderByDescending(temp => temp.PersonName).ToList();

			// Assert
			for (int i = 0; i < person_responses_from_expected.Count; i++)
			{
				Assert.Equal(person_responses_from_expected[i], person_responses_from_sorted[i]);
			} */
			person_responses_from_sorted.Should().BeInDescendingOrder(temp => temp.PersonName);
		}

		#endregion

		#region UpdatePerson

		//When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			//Arrange
			PersonUpdateRequest? person_update_request = null;

			Func<Task> action = async () => {
				//Act
				await _personService.UpdatePerson(person_update_request);
			};
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
		}


		//When we supply invalid person id, it should throw ArgumentException
		[Fact]
		public async void UpdatePerson_InvalidPersonID()
		{
			//Arrange
			PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>().Create();

			
			Func<Task> action = async () => {
				//Act
				await _personService.UpdatePerson(person_update_request);
			};
            //Assert
			await action.Should().ThrowAsync<ArgumentException>();
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
		public async Task UpdatePerson_PersonNameIsNull()
		{
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            
            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryAddRequest1);
            
            PersonAddRequest person_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email1@gmail.com")
                .With(temp => temp.PersonName, "Piii")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request1);

			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = null;

			Func<Task> action =  async () => {
				//Act
				await _personService.UpdatePerson(person_update_request);
			};

			//Assert
            await action.Should().ThrowAsync<ArgumentException>();

		}


		//First, add a new person and try to update the person name and email
		[Fact]
		public async Task UpdatePerson_PersonFullDetailsUpdation()
		{
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryAddRequest1);

            PersonAddRequest person_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email1@gmail.com")
                .With(temp => temp.PersonName, "Piii")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();


            PersonResponse person_response_from_add = await _personService.AddPerson(person_request1);

			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = "William";
			person_update_request.Email = "william@example.com";

			//Act
			PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

			PersonResponse? person_response_from_get = await _personService.GetPersonByID(person_response_from_update.PersonID);

			//Assert
			//Assert.Equal(person_response_from_get, person_response_from_update);
			person_response_from_update.Should().Be(person_response_from_get);
		}

		#endregion

		#region DeletePerson

		//If you supply an valid PersonID, it should return true
		[Fact]
		public async Task DeletePerson_ValidPersonID()
		{
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryAddRequest1);

            PersonAddRequest person_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email1@gmail.com")
                .With(temp => temp.PersonName, "Piii")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request1);


			//Act
			bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

			//Assert
			//Assert.True(isDeleted);
			isDeleted.Should().BeTrue();
		}


		//If you supply an invalid PersonID, it should return false
		[Fact]
		public async Task DeletePerson_InvalidPersonID()
		{
			//Act
			bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

			//Assert
			//Assert.False(isDeleted);
			isDeleted.Should().BeFalse();
		}

		#endregion
	}
}
