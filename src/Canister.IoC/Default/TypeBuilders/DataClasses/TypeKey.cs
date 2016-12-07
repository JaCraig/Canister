/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Linq;
using System.Text;

namespace Canister.IoC.Default.TypeBuilders.DataClasses
{
    /// <summary>
    /// Type array key class
    /// </summary>
    public class TypeKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeKey"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        public TypeKey(Type[] keys)
        {
            Keys = keys;
        }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public Type[] Keys { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var TempObj = obj as TypeKey;
            if (TempObj == null)
                return false;
            if (Keys.Length != TempObj.Keys.Length)
                return false;
            return Keys.All(TempObj.Keys.Contains);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Keys.Sum(x => x.GetHashCode());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            var Builder = new StringBuilder();
            string Seperator = "";
            foreach (var Key in Keys)
            {
                Builder.AppendFormat("{0}{1}", Seperator, Key.Name);
                Seperator = ",";
            }
            return Builder.ToString();
        }
    }
}