global using System.Collections.Immutable;
global using System.Windows.Input;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
global using Uno.Extensions;
global using Uno.Extensions.Hosting;
global using Uno.Extensions.Logging;
global using Uno.Toolkit.UI;
global using Windows.ApplicationModel;
global using Windows.Networking.Connectivity;
global using Windows.Storage;
global using Prism;
#if PRISM_NATIVE_AOT_VALIDATION
global using PrismApplication = Prism.PrismApplicationBase;
#else
global using Prism.DryIoc;
#endif
global using Prism.Ioc;
global using Prism.Modularity;
global using Prism.Mvvm;
global using Prism.Navigation.Regions;
global using Application = Microsoft.UI.Xaml.Application;
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
