﻿using ECaterer.Contracts;
using ECaterer.Contracts.Diets;
using ECaterer.Contracts.Meals;
using ECaterer.Contracts.Orders;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ECaterer.WebApi.Integration.Test
{
    [TestCaseOrderer("ECaterer.WebApi.Integration.Test.AlphabeticalOrderer", "ECaterer.WebApi.Integration.Test")]
    public class DietsTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;
        private static string dietId;
        private static string[] mealsIds;

        public DietsTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task AATestLoginProducer()
        {
            var request = new
            {
                Url = "/api/client/login",
                Body = CredentialResolver.ResolveProducer()
            };

            var response = await Client.PostAsJsonAsync(request.Url, request.Body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var auth = response.Headers.GetValues("api-key").FirstOrDefault();

            auth.Should().NotBe(default(string));

            TokenHandler.SetToken(auth);
        }

        [Fact]
        public async Task ABTestEnsureMealsExist()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/meals");
            var a = TokenHandler.GetToken();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new MealModel()
            {
                Name = "Meal 1",
                AllergentList = new string[] { "Alergen 1", "Alergen 2" },
                IngredientList = new string[] { "Ingr 1", "Ingr2", "Ingr3" },
                Calories = 500,
                Vegan = true
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/meals");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new MealModel()
            {
                Name = "Meal 2",
                AllergentList = new string[] { "Alergen 1", "Alergen 2" },
                IngredientList = new string[] { "Ingr 1", "Ingr2", "Ingr3" },
                Calories = 1000,
                Vegan = false
            });

            response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/meals");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new MealModel()
            {
                Name = "Meal 3",
                AllergentList = new string[] { "Alergen 1", "Alergen 2" },
                IngredientList = new string[] { "Ingr 1", "Ingr2", "Ingr3" },
                Calories = 700,
                Vegan = false
            });

            response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/meals");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            mealsIds = (await response.Content.ReadFromJsonAsync<GetMealsResponseModel[]>()).Where(meal => meal.Name == "Meal 1" || meal.Name == "Meal 2" || meal.Name == "Meal 3").Select(meals => meals.Id).ToArray();
        }

        [Fact]
        public async Task BATestAddDiet()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/diets");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = mealsIds,
                Price = 1200
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task BBTestGetDiets()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/diets");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var diets = await response.Content.ReadFromJsonAsync<GetDietModel[]>();

            var newDietId = diets.Where(diet => diet.Name == "Diet 1" && diet.Price == 1200).Select(diet => diet.Id).FirstOrDefault();
            newDietId.Should().NotBe(default(string));
            dietId = newDietId;
        }

        [Fact]
        public async Task BCTestEditDietPrice()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/diets/{dietId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = mealsIds,
                Price = 1000
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task BDTestDeleteDiet()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/diets/{dietId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CATestGetDietByIdUnauthorized()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/diets/{dietId}");

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CBTestGetUnexistingDietById()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/diets/some_invalid_id");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CATestAddDietAndGetId()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/diets");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = new string[] { mealsIds[1] },
                Price = 1200
            });

            requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/diets");

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var diets = await response.Content.ReadFromJsonAsync<GetDietModel[]>();

            var newDietId = diets.Where(diet => diet.Name == "Diet 1" && diet.Price == 1200).Select(diet => diet.Id).FirstOrDefault();
            dietId = newDietId;
        }

        [Fact]
        public async Task CBTestEditDietUnexistingMeal()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/diets/{dietId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = new string[] { mealsIds[0], mealsIds[1], "some_invalid_id" },
                Price = 1000
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CCTestDeleteDiet()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/diets/{dietId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CDTestDeleteUnexistingDiet()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/diets/some_invalid_id");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DATestAddDietAndGetId()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/diets");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = new string[] { mealsIds[1] },
                Price = 1200
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/diets");

            response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var diets = await response.Content.ReadFromJsonAsync<GetDietModel[]>();

            var newDietId = diets.Where(diet => diet.Name == "Diet 1" && diet.Price == 1200).Select(diet => diet.Id).FirstOrDefault();
            dietId = newDietId;
        }

        [Fact]
        public async Task DBTestEditDietUnauthorized()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/diets/{dietId}");
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = new string[] { mealsIds[0], mealsIds[1] },
                Price = 1000
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EATestGetDietsUnauthorized()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/diets");

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EBTestAddDietUnauthorized()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/diets");
            requestMessage.Content = JsonContent.Create(new Contracts.Diets.DietModel()
            {
                Name = "Diet 1",
                MealIds = new string[] { mealsIds[1] },
                Price = 1200
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);   
        }

        [Fact]
        public async Task FATestDeleteDiet()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/diets/{dietId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task FBTestEnsureDeleteMeals()
        {
            foreach (var mealId in mealsIds)
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/meals/{mealId}");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

                var response = await Client.SendAsync(requestMessage);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

    }
}
