﻿namespace TestcontainerExample.Tests.Infrastructure;

[CollectionDefinition("IntegrationTest")]
public class IntegrationTestCollection : ICollectionFixture<TestcontainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
