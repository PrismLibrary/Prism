

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Prism.Modularity
{
    [Serializable]
    public partial class ModularityException
    {
        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ModularityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ModuleName = info.GetValue("ModuleName", typeof(string)) as string;
        }

        /// <summary>
        /// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleName", this.ModuleName);
        }
    }
}
