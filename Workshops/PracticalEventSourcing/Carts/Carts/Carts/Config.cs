using Carts.Carts.Commands;
using Carts.Pricing;
using Core.Repositories;
using Core.Storage;
using Marten;
using Marten.Pagination;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Carts.Carts
{
    internal static class CartsConfig
    {
        internal static void AddCarts(this IServiceCollection services)
        {
            services.AddScoped<IProductPriceCalculator, RandomProductPriceCalculator>();

            services.AddScoped<IRepository<Cart>, MartenRepository<Cart>>();

            AddCommandHandlers(services);
            AddQueryHandlers(services);
        }

        private static void AddCommandHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<InitCart, Unit>, CartCommandHandler>();
            services.AddScoped<IRequestHandler<AddProduct, Unit>, CartCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveProduct, Unit>, CartCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmCart, Unit>, CartCommandHandler>();
        }

        private static void AddQueryHandlers(IServiceCollection services)
        {
            // services.AddScoped<IRequestHandler<GetCartById, CartDetails>, CartQueryHandler>();
            // services.AddScoped<IRequestHandler<GetCartAtVersion, CartDetails>, CartQueryHandler>();
            // services.AddScoped<IRequestHandler<GetCarts, IPagedList<CartShortInfo>>, CartQueryHandler>();
            // services
            //     .AddScoped<IRequestHandler<GetCartHistory, IPagedList<CartHistory>>, CartQueryHandler>();
        }

        internal static void ConfigureCarts(this StoreOptions options)
        {
            // Snapshots
            options.Events.InlineProjections.AggregateStreamsWith<Cart>();
            // options.Schema.For<Cart>().Index(x => x.SeatId, x =>
            // {
            //     x.IsUnique = true;
            //
            //     // Partial index by supplying a condition
            //     x.Where = "(data ->> 'Status') != 'Cancelled'";
            // });
            // options.Schema.For<Cart>().Index(x => x.Number, x =>
            // {
            //     x.IsUnique = true;
            //
            //     // Partial index by supplying a condition
            //     x.Where = "(data ->> 'Status') != 'Cancelled'";
            // });
            //
            //
            // // options.Schema.For<Cart>().UniqueIndex(x => x.SeatId);
            //
            // // projections
            // options.Events.InlineProjections.Add<CartDetailsProjection>();
            // options.Events.InlineProjections.Add<CartShortInfoProjection>();
            //
            // // transformation
            // options.Events.InlineProjections.TransformEvents<TentativeCartCreated, CartHistory>(new CartHistoryTransformation());
            // options.Events.InlineProjections.TransformEvents<CartSeatChanged, CartHistory>(new CartHistoryTransformation());
            // options.Events.InlineProjections.TransformEvents<CartConfirmed, CartHistory>(new CartHistoryTransformation());
            // options.Events.InlineProjections.TransformEvents<CartCancelled, CartHistory>(new CartHistoryTransformation());
        }
    }
}