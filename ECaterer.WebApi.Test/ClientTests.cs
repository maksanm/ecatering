﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ECaterer.Contracts;
using FluentAssertions;
using Xunit;
using ECaterer.WebApi.Data;
using System.Net.Http.Json;
using ECaterer.Contracts.Client;
using System.Linq;
using System.Net.Http.Headers;
using System.Collections.Generic;
using ECaterer.Contracts.Orders;
using ECaterer.Contracts.Diets;
using System.Net;

namespace ECaterer.WebApi.Integration.Test
{
    [TestCaseOrderer("ECaterer.WebApi.Integration.Test.AlphabeticalOrderer", "ECaterer.WebApi.Integration.Test")]
    public class ClientTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;

        private static string random = new Random().Next().ToString();
        private string orderId;

        public ClientTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task AATestRegisterUser()
        {
            TokenHandler.SetToken("authtokene");
            var request = new
            {
                Url = "/api/client/register",
                Body = new ClientModel()
                {
                    Email = $"{random}@gmail.com",
                    Address = new Contracts.Client.AddressModel()
                    {
                        City = "Strzelno",
                        BuildingNumber = "1",
                        Street = "Kościuszki",
                        PostCode = "88-320"
                    },
                    PhoneNumber = "+48-666-666-666",
                    Name = "Jan",
                    LastName = "Kowalski",
                    Password = "1234!Aaaa"
                }
            };

            var response = await Client.PostAsJsonAsync<ClientModel>(request.Url, request.Body);
            response.EnsureSuccessStatusCode();

            var auth = response.Headers.GetValues("api-key").FirstOrDefault();

            auth.Should().NotBeNull();

            TokenHandler.SetToken(auth);

        }

        [Fact]
        public async Task ABTestLoginUser()
        {
            var request = new
            {
                Url = "/api/client/login",
                Body = new LoginUserModel()
                {
                    Email = $"{random}@gmail.com",
                    Password = "1234!Aaaa"
                }
            };

            var response = await Client.PostAsJsonAsync<LoginUserModel>(request.Url, request.Body);

            response.EnsureSuccessStatusCode();

            var auth = response.Headers.GetValues("api-key").FirstOrDefault();

            auth.Should().NotBeNull();

            TokenHandler.SetToken(auth);

        }

        [Fact]
        public async Task ACTestGetAccountUser()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/client/account");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var clientContent = await response.Content.ReadFromJsonAsync<ClientModel>();

            clientContent.Email.Should().Be($"{random}@gmail.com");

        }

        [Fact]
        public async Task ADTestPutAccountUser()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/client/account");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new ClientModel()
            {
                Email = $"{random}@gmail.com",
                Address = new Contracts.Client.AddressModel()
                {
                    City = "Warszawa",
                    BuildingNumber = "1",
                    Street = "Kościuszki",
                    PostCode = "88-320"
                },
                PhoneNumber = "+48-666-666-666",
                Name = "Jan",
                LastName = "Kowalski",
                Password = "1234!Aaaa"
            });

            var response = await Client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task BA_AddOrder_OK()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            var diets = await Client.GetFromJsonAsync<GetDietModel[]>("/api/diets?limit=3");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/client/orders");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new AddOrderModel()
            {
                DietIDs = diets.Select(d => d.Id).ToArray(),
                DeliveryDetails = new DeliveryDetailsModel()
                {
                    Address = new AddressModel()
                    {
                        Street = "Koszykowa",
                        BuildingNumber = "75",
                        ApartmentNumber = "304",
                        City = "Warszawa",
                        PostCode = "00-662"
                    },
                    PhoneNumber = "226219312",
                    CommentForDeliverer = "Sample comment"
                },
                StartDate = DateTime.Now.Date.AddDays(+1).AddHours(14),
                EndDate = DateTime.Now.Date.AddDays(+5).AddHours(14)
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task BB_AddOrder_Unauthorized()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/client/orders");
            requestMessage.Content = JsonContent.Create(new AddOrderModel()
            {
                DietIDs = new string[0],
                DeliveryDetails = new DeliveryDetailsModel()
                {
                    Address = new AddressModel()
                    {
                        Street = "Koszykowa",
                        BuildingNumber = "75",
                        ApartmentNumber = "304",
                        City = "Warszawa",
                        PostCode = "00-662"
                    },
                    PhoneNumber = "226219312",
                    CommentForDeliverer = "Sample comment"
                },
                StartDate = DateTime.Now.Date.AddDays(+1).AddHours(14),
                EndDate = DateTime.Now.Date.AddDays(+5).AddHours(14)
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task BC_AddOrder_BadRequest()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/client/orders");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            requestMessage.Content = JsonContent.Create(new AddOrderModel()
            {
                DietIDs = null,
                DeliveryDetails = new DeliveryDetailsModel()
                {
                    Address = new AddressModel()
                    {
                        Street = "Koszykowa",
                        BuildingNumber = "75",
                        ApartmentNumber = "304",
                        City = "Warszawa",
                        PostCode = "00-662"
                    },
                    PhoneNumber = "226219312",
                    CommentForDeliverer = "Sample comment"
                },
                StartDate = DateTime.Now.Date.AddDays(-10).AddHours(14),
                EndDate = DateTime.Now.Date.AddDays(+5).AddHours(14)
            });

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CA_GetOrders_OK()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());
            var orders = await Client.GetFromJsonAsync<OrderModel[]>("/api/orders");
            orders.Should().NotBeNull();
            orders.Count().Should().NotBe(0);

            orderId = orders.Last().Id;

            orderId.Should().NotBeNull();
        }

        [Fact]
        public async Task CB_GetOrders_Unauthorized()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/client/orders$limit=10");

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CC_GetOrders_BadRequest()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/client/orders$limit=10");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CA_PayOrder_OK()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/client/orders/{orderId}/pay");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CB_PayOrder_Unauthorized()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/client/orders/{orderId}/pay");

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CC_PayOrder_NotFound()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/client/orders/unexisting-meal/pay");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("api-key", TokenHandler.GetToken());

            var response = await Client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DATestLoginUnexistingUser()
        {
            var request = new
            {
                Url = "/api/client/login",
                Body = new LoginUserModel()
                {
                    Email = $"blabla@gmail.com",
                    Password = "1234"
                }
            };

            var response = await Client.PostAsJsonAsync(request.Url, request.Body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DBTestLoginInvalidPassword()
        {
            var request = new
            {
                Url = "/api/client/login",
                Body = new LoginUserModel()
                {
                    Email = $"{random}@gmail.com",
                    Password = "1234!bla"
                }
            };

            var response = await Client.PostAsJsonAsync(request.Url, request.Body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task EATestRegisterUserBadEmail()
        {
            var request = new
            {
                Url = "/api/client/register",
                Body = new ClientModel()
                {
                    Email = $"{random}gmail.com",
                    Address = new Contracts.Client.AddressModel()
                    {
                        City = "Strzelno",
                        BuildingNumber = "1",
                        Street = "Kościuszki",
                        PostCode = "88-320"
                    },
                    PhoneNumber = "+48-666-666-666",
                    Name = "Jan",
                    LastName = "Kowalski",
                    Password = "1234!Aaaa"
                }
            };

            var response = await Client.PostAsJsonAsync(request.Url, request.Body);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task EBTestRegisterUserBadPassword()
        {
            var request = new
            {
                Url = "/api/client/register",
                Body = new ClientModel()
                {
                    Email = $"{random}@gmail.com",
                    Address = new Contracts.Client.AddressModel()
                    {
                        City = "Strzelno",
                        BuildingNumber = "1",
                        Street = "Kościuszki",
                        PostCode = "88-320"
                    },
                    PhoneNumber = "+48-666-666-666",
                    Name = "Jan",
                    LastName = "Kowalski",
                    Password = "1234"
                }
            };

            var response = await Client.PostAsJsonAsync(request.Url, request.Body);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}