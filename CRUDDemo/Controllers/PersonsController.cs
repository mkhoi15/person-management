using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDDemo.Controllers
{
    [Route("[controller]")]
	public class PersonsController : Controller
	{
		// private field
		private readonly IPersonService _personService;
        private readonly ICountriesServices _countriesService;

		// Contructor
		public PersonsController(IPersonService personService,ICountriesServices countriesServices)
		{
			_personService = personService;
            _countriesService = countriesServices;
		}

        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
      {
        { nameof(PersonResponse.PersonName), "Person Name" },
        { nameof(PersonResponse.Email), "Email" },
        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        { nameof(PersonResponse.Gender), "Gender" },
        { nameof(PersonResponse.CountryID), "Country" },
        { nameof(PersonResponse.Address), "Address" }
      };
            List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

		[Route("[action]")]
		[HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });

           
            return View();
        }

		[Route("[action]")]
		[HttpPost]
		public IActionResult Create(PersonAddRequest personAddRequest)
		{
            if (!ModelState.IsValid)
            {
				List<CountryResponse> countries = _countriesService.GetAllCountry();
				ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
			}
           
            PersonResponse personResponse = _personService.AddPPerson(personAddRequest);


			return RedirectToAction("Index", "Persons");
		}


        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse = _personService.GetPersonByID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }


        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = _personService.GetPersonByID(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
               PersonResponse updatedPerson = _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }

            else
            {
                List<CountryResponse> countries = _countriesService.GetAllCountry();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }  
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public IActionResult Delete(Guid? personID)
        {
            PersonResponse? personResponse = _personService.GetPersonByID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
          return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Delete(PersonUpdateRequest personUpdatResult)
        {
            PersonResponse? personResponse = _personService.GetPersonByID(personUpdatResult.PersonID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            _personService.DeletePerson(personUpdatResult.PersonID);

            return RedirectToAction("Index");
        }
    }
}
