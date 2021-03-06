// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class ActiveMqHostEqualityComparer :
        IEqualityComparer<ActiveMqHostSettings>
    {
        public static IEqualityComparer<ActiveMqHostSettings> Default { get; } = new ActiveMqHostEqualityComparer();

        public bool Equals(ActiveMqHostSettings x, ActiveMqHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;
            //TODO: FO - Fix this - Compare the collections, not the first!!
            return string.Equals(x.Nodes.FirstOrDefault().Host, y.Nodes.FirstOrDefault().Host, StringComparison.OrdinalIgnoreCase) && 
                                 x.Nodes.FirstOrDefault().Port == y.Nodes.FirstOrDefault().Port;
        }

        public int GetHashCode(ActiveMqHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.Nodes.FirstOrDefault().Host?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Nodes.FirstOrDefault().Port;
                return hashCode;
            }
        }
    }
}