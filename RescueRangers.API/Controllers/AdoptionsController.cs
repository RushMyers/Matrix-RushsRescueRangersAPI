﻿using RescueRangers.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueRangers.API.DataStores;

namespace RescueRangers.API.Controllers
{
    [Route("api/[controller]")]
    public class AdoptionsController : Controller
    {
        [HttpGet()]
        public IActionResult GetAdoptions()
        {
            return Ok(AdoptionsDataStore.Current.Adoptions);
        }

        [HttpPost()]
        public IActionResult CreateAdoption([FromBody] AdoptionBody adoptionBody)
        {
            if (adoptionBody.Animal == null || adoptionBody.Adopter == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var CurrentAdopters = DataStores.AdoptersDataStore.Current.Adopters;

            uint HighestAdopterId;
            if (CurrentAdopters.Any())
            {
                HighestAdopterId = CurrentAdopters.Max(adopter => adopter.Id);
            } else
            {
                HighestAdopterId = 0;
            }
            
            var NewAdopter = new AdopterDto()
            {
                Id = ++HighestAdopterId,
                FirstName = adoptionBody.Adopter.FirstName,
                LastName = adoptionBody.Adopter.LastName, 
                Address = adoptionBody.Adopter.Address,
                City = adoptionBody.Adopter.City,
                Zipcode = adoptionBody.Adopter.Zipcode,
                PhoneNo = adoptionBody.Adopter.PhoneNo
            };
            AdoptersDataStore.Current.Adopters.Add(NewAdopter);

            var CurrentAdoptions = DataStores.AdoptionsDataStore.Current.Adoptions;
            uint HighestAdoptionId;

            if ( CurrentAdoptions.Any())
            {
                HighestAdoptionId = DataStores.AdoptionsDataStore.Current.Adoptions.Max(adoption => adoption.Id);
            } else
            {
                HighestAdoptionId = 0;
            }

            var NewAdoption = new AdoptionDto()
            {
                Id = ++HighestAdoptionId,
                AdopterId = NewAdopter.Id,
                AnimalId = adoptionBody.Animal.Id,
                Date = DateTime.Today
            };
            AdoptionsDataStore.Current.Adoptions.Add(NewAdoption);

            var AnimalToUpdate = AnimalsDataStore.Current.Animals.FirstOrDefault(animal => animal.Id == adoptionBody.Animal.Id);
            if ( AnimalToUpdate == null )
            {
                return NotFound();
            }

            AnimalToUpdate.IsAdopted = true;
            AnimalToUpdate.AdoptionId = NewAdoption.Id;

            return Ok();
        }
    }
}