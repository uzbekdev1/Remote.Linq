﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.SimpleAsyncStreamProtocol
{
    public readonly struct NextResponse<T>
    {
        public long SequenceNumber { get; init; }

        public bool HasNext { get; init; }

        public T Item { get; init; }
    }
}
