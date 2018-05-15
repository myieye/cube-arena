using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.Tests.MyScripts.Utils.Constants;
using NUnit.Framework;

namespace CubeArena.Assets.Tests.MyScripts.Utils {
	public abstract class BaseSettingsTest {

		protected MockSettings settings { get; private set; }

		[SetUp]
		protected void InitSettings () {
			Settings.Instance = settings = new MockSettings ();
		}
	}
}