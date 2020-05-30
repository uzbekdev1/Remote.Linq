﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Client;
    using Server;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Entity Framework Core");
            const int port = 8899;

            PrintSetup("Starting TCP service...");
            using var serviceHost = new TcpServer(port);
            serviceHost.RunAsyncQueryService(new QueryService().ExecuteQueryAsync);

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            new Demo(() => new RemoteRepository("localhost", port)).RunAsync().Wait();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}