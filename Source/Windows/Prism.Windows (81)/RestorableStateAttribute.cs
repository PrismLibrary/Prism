// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Prism.Mvvm
{
    /// <summary>
    /// This attribute indicates that the marked property will have its state saved on suspension.
    /// </summary>
    // Documentation on handling suspend, resume, and activation is at http://go.microsoft.com/fwlink/?LinkID=288819&clcid=0x409

    [AttributeUsage(System.AttributeTargets.Property,
                    AllowMultiple = false)]
    public sealed class RestorableStateAttribute : Attribute
    {

    }
}
