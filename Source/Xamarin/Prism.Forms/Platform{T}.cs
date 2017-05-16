using Prism.AppModel;
using System;

namespace Prism
{
	public interface IPlatform
	{
		RuntimePlatform RuntimePlatform { get; }

		Type ViewType { get; }
	}

	public class Platform<TView> : IPlatform
	{
		public RuntimePlatform RuntimePlatform { get; }

		public Type ViewType { get { return typeof(TView); } }

		public Platform(RuntimePlatform runtimePlatform)
		{
			RuntimePlatform = runtimePlatform;
		}
	}
}
