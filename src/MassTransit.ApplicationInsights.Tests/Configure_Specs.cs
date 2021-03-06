﻿// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.ApplicationInsights.Tests
{
    using Microsoft.ApplicationInsights;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_Specs
    {
        [Test]
        public void Test()
        {
            var telemetryClient = new TelemetryClient();

            IBusControl CreateBus() =>
                Bus.Factory.CreateUsingInMemory(x =>
                {
                    x.UseApplicationInsights(telemetryClient, (operation, context) => operation.Telemetry.Properties.Add("prop", "v"));
                });

            Assert.DoesNotThrow(() => CreateBus());
        }
    }
}
