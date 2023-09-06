using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;

namespace CRUDTests
{
	public class CountriesServicesTest
	{
		private readonly ICountriesServices _countriesService;

		public CountriesServicesTest()
		{
			_countriesService = new CountriesService(false);
		}

		#region AddCountry

		// When CountryAddRequest is null it should throw ArgumentException 
		[Fact]
		public void AddCountry_NullCountry()
		{
			// Arrange 
			CountryAddRequest? request = null;

			//Assert 
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act
				_countriesService.AddCountry(request);
			});
		}

		// When CountryName is null it should throw ArgumentException 
		[Fact]
		public void AddCountry_CountryNameIsNull()
		{
			// Arrange 
			CountryAddRequest? request = new CountryAddRequest()
			{
				CountryName = null
			};

			//Assert 
			Assert.Throws<ArgumentException>(() =>
			{
				//Act
				_countriesService.AddCountry(request);
			});
		}

		// When CountryName is duplicated it should throw ArgumentException 
		[Fact]
		public void AddCountry_DuplicateContryName()
		{
			// Arrange 
			CountryAddRequest? request1 = new CountryAddRequest()
			{
				CountryName = "VietNam"
			};
			CountryAddRequest? request2 = new CountryAddRequest()
			{
				CountryName = "VietNam"
			};
			//Assert 
			Assert.Throws<ArgumentException>(() =>
			{
				//Act
				_countriesService.AddCountry(request1);
				_countriesService.AddCountry(request2);
			});
		}

		// When assign a proper countryName it should add 
		[Fact]
		public void AddCountry_ProperCountryDetails()
		{
			// Arrange 
			CountryAddRequest? request = new CountryAddRequest() { CountryName = "USA"};

			//Act 
			CountryResponse response = _countriesService.AddCountry(request);
			List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountry();

			//Assert 
			Assert.True(response.CountryID != Guid.Empty);
			Assert.Contains(response, countries_from_GetAllCountries);
			
		}

		#endregion

		#region GetAllCountry

		[Fact]
		// The list of countries should be empty by default
		public void GetAllCountry_EmptyList()
		{
			// Act 
			List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountry();

			// Assert 
			Assert.Empty(actual_country_response_list);
		}

		[Fact]
		public void GetAllCountry_AddFewCountries()
		{
			// Arrange
			List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
			{
				new CountryAddRequest(){ CountryName = "USA"},
				new CountryAddRequest(){ CountryName = "UK"}
			};

			// Act
			List<CountryResponse> countries_list_from_addCountry = new List<CountryResponse>();

			foreach (CountryAddRequest countryRequest in countryAddRequests)
			{
				countries_list_from_addCountry.Add(_countriesService.AddCountry(countryRequest));
			}

			List<CountryResponse> actual_countries_list = _countriesService.GetAllCountry();

			// read each element
			foreach (CountryResponse expected_country in countries_list_from_addCountry)
			{
				Assert.Contains(expected_country, actual_countries_list);
			}
		}

		#endregion

		#region GetCountryByCountryID

		[Fact]
		// If we supply null value it should error
		public void GetCountryByID_NullCountryID()
		{
			// Arrange 
			Guid? countryID = null;

			//Act 
			CountryResponse? country_response_from_get_method = _countriesService.GetCountryByID(countryID);

			// Assert
			Assert.Null(country_response_from_get_method);

		}

		[Fact]
		// If we supply a valid id, if it match with country in the list it will return the country name 
		public void GetCountryByID_ValidCountryID()
		{
			// Arrange 
			CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China" };
			CountryResponse? country_response_from_add = _countriesService.AddCountry(country_add_request);

			// Act 
			CountryResponse? country_response_from_get_method = _countriesService.GetCountryByID(country_response_from_add.CountryID);

			//Assert
			Assert.Equal(country_response_from_add, country_response_from_get_method);
		}

		#endregion
	}
}
