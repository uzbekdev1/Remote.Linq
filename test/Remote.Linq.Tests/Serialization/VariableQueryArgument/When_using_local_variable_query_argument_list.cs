﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public abstract class When_using_local_variable_query_argument_list
    {
        public class BinaryFormatter : When_using_local_variable_query_argument_list
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_local_variable_query_argument_list
        {
            public DataContractSerializer()
                : base(x => DataContractSerializationHelper.SerializeExpression(x, new[] { typeof(List<int>) }))
            {
            }
        }

        public class JsonSerializer : When_using_local_variable_query_argument_list
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_local_variable_query_argument_list
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class ProtobufNetSerializer : When_using_local_variable_query_argument_list
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_local_variable_query_argument_list
        {
            public XmlSerializer()
                : base(x => XmlSerializationHelper.SerializeExpression(x, new[] { typeof(List<int>) }))
            {
            }
        }

        public class AType
        {
            public int Number { get; set; }
        }

        private readonly LambdaExpression _remoteExpression;

        private readonly LambdaExpression _serializedRemoteExpression;

        protected When_using_local_variable_query_argument_list(Func<LambdaExpression, LambdaExpression> serialize)
        {
            var values = new List<int> { 123, 456, 789 };

            System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => values.Contains(x.Number);

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Remote_expression_should_be_equal()
        {
            _remoteExpression.ShouldEqualRemoteExpression(_serializedRemoteExpression);
        }

        [Fact]
        public void System_expresison_should_be_equal()
        {
            var exp1 = _remoteExpression.ToLinqExpression<AType, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, bool>();

            exp1.ShouldEqualExpression(exp2);
        }
    }
}
