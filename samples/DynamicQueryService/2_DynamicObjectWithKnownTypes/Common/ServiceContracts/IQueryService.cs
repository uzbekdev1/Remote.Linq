﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common.Model;
using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System.Collections.Generic;
using System.ServiceModel;

namespace Common.ServiceContracts
{
    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        [ServiceKnownType(typeof(OrderItem))]
        [ServiceKnownType(typeof(Product))]
        [ServiceKnownType(typeof(ProductCategory))]
        IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression);
    }
}