// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ActiveMqHostConfigurator :
        IActiveMqHostConfigurator
    {
        readonly ConfigurationHostSettings _settings;

        public ActiveMqHostConfigurator(Uri address)
            : this(new[] {address})
        {
        }

        public ActiveMqHostConfigurator(IEnumerable<Uri> addresses)
        {
            IEnumerable<string> invalidSchemes = addresses.Select(address => address.Scheme)
                .Where(scheme => string.Compare("activemq", scheme, StringComparison.OrdinalIgnoreCase) != 0)
                .Distinct();

            if (invalidSchemes.Any())
                throw new ActiveMqTransportConfigurationException($"The address scheme was invalid: {string.Join(",", invalidSchemes)}");

            _settings = new ConfigurationHostSettings
            {
                Nodes = addresses.Select(address => new Node
                {
                    Host = address.Host,
                    Port = !address.IsDefaultPort ? address.Port : 61616
                }).ToArray(),
                Username = "",
                Password = ""
            };

            string userInfo = addresses.FirstOrDefault()?.UserInfo;
            if (!string.IsNullOrEmpty(userInfo))
            {
                string[] parts = userInfo.Split(':');
                _settings.Username = parts[0];

                if (parts.Length >= 2)
                    _settings.Password = parts[1];
            }
        }

        public ActiveMqHostSettings Settings => _settings;

        public void Username(string username)
        {
            _settings.Username = username;
        }

        public void Password(string password)
        {
            _settings.Password = password;
        }

        public void UseSsl()
        {
            _settings.UseSsl = true;
            foreach (var node in _settings.Nodes)
            {
                if (node.Port == 61616)
                    node.Port = 61617;    
            }
        }

        public void UsePrimaryNodeFirst()
        {
            _settings.Randomize = false;
        }
    }
}