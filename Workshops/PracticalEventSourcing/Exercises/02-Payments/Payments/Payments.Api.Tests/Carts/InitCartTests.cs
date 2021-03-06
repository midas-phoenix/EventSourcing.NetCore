using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Testing;
using FluentAssertions;
using Payments.Api.Requests.Carts;
using Payments.Payments.Events;
using Shipments.Api.Tests.Core;
using Xunit;

namespace Payments.Api.Tests.Payments
{
    public class InitPaymentFixture: ApiFixture<Startup>
    {
        protected override string ApiUrl { get; } = "/api/Payments";

        public readonly Guid OrderId = Guid.NewGuid();

        public readonly decimal Amount = new Random().Next(100);

        public readonly DateTime TimeBeforeSending = DateTime.UtcNow;

        public HttpResponseMessage CommandResponse;

        public override async Task InitializeAsync()
        {
            CommandResponse = await PostAsync(new RequestPaymentRequest {OrderId = OrderId, Amount = Amount});
        }
    }

    public class InitPaymentTests: IClassFixture<InitPaymentFixture>
    {
        private readonly InitPaymentFixture fixture;

        public InitPaymentTests(InitPaymentFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Exercise")]
        public async Task CreateCommand_ShouldReturn_CreatedStatus_With_PaymentId()
        {
            var commandResponse = fixture.CommandResponse;
            commandResponse.EnsureSuccessStatusCode();
            commandResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // get created record id
            var commandResult = await commandResponse.Content.ReadAsStringAsync();
            commandResult.Should().NotBeNull();

            var createdId = commandResult.FromJson<Guid>();
            createdId.Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Exercise")]
        public async Task CreateCommand_ShouldPublish_PaymentInitializedEvent()
        {
            var createdId = await fixture.CommandResponse.GetResultFromJSON<Guid>();

            fixture.PublishedInternalEventsOfType<PaymentRequested>()
                .Should()
                .HaveCount(1)
                .And.Contain(@event =>
                    @event.PaymentId == createdId
                    && @event.OrderId == fixture.OrderId
                    && @event.Amount == fixture.Amount
                );
        }

        // [Fact]
        // [Trait("Category", "Exercise")]
        // public async Task CreateCommand_ShouldCreate_Payment()
        // {
        //     var createdId = await fixture.CommandResponse.GetResultFromJSON<Guid>();
        //
        //     // prepare query
        //     var query = $"{createdId}";
        //
        //     //send query
        //     var queryResponse = await fixture.GetAsync(query);
        //     queryResponse.EnsureSuccessStatusCode();
        //
        //     var queryResult = await queryResponse.Content.ReadAsStringAsync();
        //     queryResult.Should().NotBeNull();
        //
        //     var paymentDetails = queryResult.FromJson<Payment>();
        //     paymentDetails.Id.Should().Be(createdId);
        //     paymentDetails.OrderId.Should().Be(fixture.ClientId);
        //     paymentDetails.SentAt.Should().BeAfter(fixture.TimeBeforeSending);
        //     paymentDetails.ProductItems.Should().NotBeEmpty();
        //     paymentDetails.ProductItems.All(
        //         pi => fixture.ProductItems.Exists(
        //             expi => expi.ProductId == pi.ProductId && expi.Quantity == pi.Quantity))
        //         .Should().BeTrue();
        // }
    }
}
