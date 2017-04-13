// ReSharper disable once CheckNamespace
namespace Prism.Autofac.Forms
{
    /// <summary>
    /// Identifies a class (typically an implementation of IModule) that has Type/Page registration
    /// requirements that must be handled, prior to building the Prism Autofac container.
    /// </summary>
    public interface IPreRegisterTypes
    {
        /// <summary>
        /// Method that is executed during module cataloging to register any Types/Pages that are required by the module.
        /// </summary>
        /// <param name="container">The container that registration operations will be performed on</param>
        void RegisterTypes(IAutofacContainer container);
    }
}
