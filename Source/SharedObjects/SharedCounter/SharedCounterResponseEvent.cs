﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Coyote.Machines;

namespace Microsoft.Coyote.SharedObjects
{
    /// <summary>
    /// Event containing the value of a shared counter.
    /// </summary>
    internal class SharedCounterResponseEvent : Event
    {
        /// <summary>
        /// Value.
        /// </summary>
        internal int Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedCounterResponseEvent"/> class.
        /// </summary>
        internal SharedCounterResponseEvent(int value)
        {
            this.Value = value;
        }
    }
}