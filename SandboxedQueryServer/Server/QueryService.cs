﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    internal class QueryService
    {
        private static readonly Func<Type, IQueryable> _queryableProvider = type =>
        {
            var single = Array.CreateInstance(type, 1);
            var queryable = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(x => string.Equals(x.Name, nameof(Queryable.AsQueryable)) && x.IsGenericMethodDefinition)
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { single });
            return (IQueryable)queryable;
        };

        [SecuritySafeCritical]
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            return queryExpression.Execute(_queryableProvider);
        }
    }
}
