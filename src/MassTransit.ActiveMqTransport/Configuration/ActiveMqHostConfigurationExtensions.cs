// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections.Generic;
    using Configurators;


    public static class ActiveMqHostConfigurationExtensions
    {

        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, IEnumerable<Uri> hostAddresses, Action<IActiveMqHostConfigurator> configure)
        {
            if (hostAddresses == null)
                throw new ArgumentNullException(nameof(hostAddresses));

            var hostConfigurator = new ActiveMqHostConfigurator(hostAddresses);

            configure(hostConfigurator);
            return configurator.Host(hostConfigurator.Settings);
        }


        /// <summary>
        ///     Configure a ActiveMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the ActiveMQ host (activemq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, Uri hostAddress, Action<IActiveMqHostConfigurator> configure)
        {
            if (hostAddress == null)
                throw new ArgumentNullException(nameof(hostAddress));

            var hostConfigurator = new ActiveMqHostConfigurator(hostAddress);

            configure(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, Action<IActiveMqHostConfigurator> configure)
        {
            return configurator.Host(new UriBuilder("activemq", hostName).Uri, configure);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, int port, Action<IActiveMqHostConfigurator> configure)
        {
            return configurator.Host(new UriBuilder("activemq", hostName, port).Uri, configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host,
            Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            var queueName = host.Topology.CreateTemporaryQueueName("receiveEndpoint-");

            configurator.ReceiveEndpoint(host, queueName, x =>
            {
                x.AutoDelete = true;
                x.Durable = false;

                configure(x);
            });
        }

        /// <summary>
        /// Registers a management endpoint on the bus, which can be used to control
        /// filters and other management control points on the bus.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host where the endpoint is to be created</param>
        /// <param name="configure">Configure additional values of the underlying receive endpoint</param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IActiveMqBusFactoryConfigurator configurator,
            IActiveMqHost host, Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var queueName = host.Topology.CreateTemporaryQueueName("manage-");

            IActiveMqReceiveEndpointConfigurator specification = null;

            configurator.ReceiveEndpoint(host, queueName, x =>
            {
                x.AutoDelete = true;
                x.Durable = false;

                configure?.Invoke(x);

                specification = x;
            });

            var managementEndpointConfigurator = new ManagementEndpointConfigurator(specification);

            return managementEndpointConfigurator;
        }
    }
}