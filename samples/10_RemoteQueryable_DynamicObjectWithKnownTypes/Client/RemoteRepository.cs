﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.ServiceModel;

    public sealed class RemoteRepository : IRemoteRepository
    {
        private readonly Func<Expression, DynamicObject> _dataProvider;
        private readonly IDynamicObjectMapper _mapper;
        private readonly ChannelFactory<IQueryService> _channelFactory;

        public RemoteRepository(string uri)
        {
            _mapper = new DynamicObjectMapper(isKnownTypeProvider: new IsKnownTypeProvider());

            var binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);
            _dataProvider = expression =>
                {
                    using var proxy = _channelFactory.CreateServiceProxy();
                    var result = proxy.Service.ExecuteQuery(expression);
                    return result;
                };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider, mapper: _mapper);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ProductGroup>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider, mapper: _mapper);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider, mapper: _mapper);

        public void Dispose() => _channelFactory.Close();
    }
}
