﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class DynamicAsyncQueryResultMapper : DynamicQueryResultMapper
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly Func<Type[], MethodInfo> _MapGroupToDynamicObjectGraphMethod = genericTypeArguments =>
             typeof(DynamicAsyncQueryResultMapper)
                 .GetMethod(nameof(MapAsyncGroupToDynamicObjectGraph), PrivateStatic)
                 .MakeGenericMethod(genericTypeArguments);

        private static readonly Func<Type[], MethodInfo> _MapAsyncEnumerableMethod = genericTypeArguments =>
             typeof(DynamicAsyncQueryResultMapper)
                 .GetMethod(nameof(MapAsyncEnumerable), PrivateStatic)
                 .MakeGenericMethod(genericTypeArguments);

        protected override bool ShouldMapToDynamicObject(IEnumerable collection)
            => collection.CheckNotNull(nameof(collection)).GetType().Implements(typeof(IAsyncGrouping<,>))
            || base.ShouldMapToDynamicObject(collection);

        protected override DynamicObject? MapToDynamicObjectGraph(object? obj, Func<Type, bool> setTypeInformation)
        {
            var genericTypeArguments = default(Type[]);
            if (obj?.GetType().Implements(typeof(IAsyncGrouping<,>), out genericTypeArguments) is true)
            {
                obj = _MapGroupToDynamicObjectGraphMethod(genericTypeArguments!).Invoke(null, new[] { obj });
            }
            else if (obj?.GetType().Implements(typeof(IAsyncEnumerable<>), out genericTypeArguments) is true)
            {
                obj = _MapAsyncEnumerableMethod(genericTypeArguments!).Invoke(null, new[] { obj });
            }

            return base.MapToDynamicObjectGraph(obj, setTypeInformation);
        }

        private static AsyncEnumerable<T> MapAsyncEnumerable<T>(IAsyncEnumerable<T> source)
            => source is AsyncEnumerable<T> asyncEnumerable
            ? asyncEnumerable
            : new AsyncEnumerable<T> { Elements = EnumerateBlocking(source).ToArray() };

        private static AsyncGrouping<TKey, TElement> MapAsyncGroupToDynamicObjectGraph<TKey, TElement>(IAsyncGrouping<TKey, TElement> group)
            => group is AsyncGrouping<TKey, TElement> remoteLinqGroup
            ? remoteLinqGroup
            : new AsyncGrouping<TKey, TElement> { Key = group.Key, Elements = EnumerateBlocking(group).ToArray(), };

        private static IEnumerable<T> EnumerateBlocking<T>(IAsyncEnumerable<T> source)
        {
            if (source is IEnumerable<T> elements)
            {
                return elements;
            }

            var list = new List<T>();

            Task.Run(async () =>
                {
                    await foreach (var item in source)
                    {
                        list.Add(item);
                    }
                })
                .Wait();

            return list;
        }
    }
}
