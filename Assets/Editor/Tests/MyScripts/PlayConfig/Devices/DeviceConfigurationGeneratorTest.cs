using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.Tests.MyScripts.Utils;
using CubeArena.Assets.Tests.MyScripts.Utils.Constants;
using CubeArena.Assets.Tests.MyScripts.Utils.Devices;
using NUnit.Framework;
using Rhino.Mocks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
	public class DeviceConfigurationGeneratorTest : BaseSettingsTest {

		private IDeviceManager deviceManager;
		private DeviceConfigurationGenerator configGenerator;

		[SetUp]
		public void SetUp () {
			deviceManager = MockRepository.GenerateStub<IDeviceManager> ();
			configGenerator = new DeviceConfigurationGenerator (deviceManager);
		}

		[Test]
		public void TestNotEnoughDevices () {
			List<List<DeviceConfig>> config;
			int numPlayers = 1;

			deviceManager.Stub (dm => dm.EnoughDevicesAvailable (numPlayers)).Return (false);

			LogAssert.Expect (LogType.Error, "Not enough devices!");
			var success = configGenerator.TryGenerateDeviceRoundConfigs (numPlayers, out config);

			Assert.That (success, Is.False);
		}

		[TestCase (1, 1, 1, 1, true)]
		[TestCase (2, 1, 1, 1, true)]

		[TestCase (1, 2, 2, 1, true)]
		[TestCase (2, 2, 2, 1, true)]
		[TestCase (3, 2, 2, 1, true)]
		[TestCase (4, 2, 2, 1, true)]

		[TestCase (1, 3, 3, 1, true)]
		[TestCase (2, 3, 3, 1, true)]
		[TestCase (3, 3, 3, 1, true)]
		[TestCase (4, 3, 3, 1, true)]
		[TestCase (5, 3, 3, 1, true)]
		[TestCase (6, 3, 3, 1, true)]

		[TestCase (1, 0, 1, 1, false)]
		[TestCase (1, 1, 0, 1, false)]
		[TestCase (1, 0, 0, 1, false)]

		[TestCase (2, 0, 1, 1, false)]
		[TestCase (2, 1, 0, 1, false)]
		[TestCase (2, 0, 0, 1, false)]
		
		[TestCase (3, 2, 1, 1, false)]
		[TestCase (3, 2, 0, 1, false)]
		[TestCase (3, 1, 2, 1, false)]
		[TestCase (3, 0, 2, 1, false)]
		[TestCase (3, 1, 1, 1, false)]
		[TestCase (3, 1, 0, 1, false)]
		[TestCase (3, 0, 1, 1, false)]
		[TestCase (3, 0, 0, 1, false)]
		public void TestDeviceConfigGeneration (int numPlayers,
				int numAndroidDevices, int numHoloLensDevices, int numDesktopDevices, bool validConfig) {
			List<List<DeviceConfig>> config;
			var devicesByType = DeviceTestUtil.GetDevicesByType (numAndroidDevices, numHoloLensDevices, numDesktopDevices);
			//settings.LogDeviceRoundConfig = true;

			deviceManager.Stub (dm => dm.EnoughDevicesAvailable (numPlayers)).Return (true);
			deviceManager.Stub (dm => dm.DevicesByType).Return (devicesByType);

			var success = configGenerator.TryGenerateDeviceRoundConfigs (numPlayers, out config);

			Assert.That (success, Is.EqualTo (validConfig));

			if (validConfig) {
				Assert.That (config.Count, Is.EqualTo (UIModeHelpers.TestUIModes.Count));
				Assert.That (config, Is.Not.Empty);
				Assert.That (config, Is.All.Not.Empty);
				Assert.That (config, Is.All.Property ("Count").EqualTo (numPlayers));

				// All modes are used the right number of times
				foreach (var uiMode in UIModeHelpers.TestUIModes) {
					int numUiModeUsages = CountUIModeUsagesInConfig (config, uiMode);
					numUiModeUsages = numPlayers;
				}

				// Each player uses each ui mode exactly once
				for (int p = 0; p < numPlayers; p++) {
					var uiModes = new List<UIMode> (UIModeHelpers.TestUIModes);
					foreach (var rConfig in config) {
						Assert.That (uiModes, Does.Contain (rConfig[p].UIMode));
						uiModes.Remove (rConfig[p].UIMode);
					}
					Assert.That (uiModes, Is.Empty);
				}

				// Never more than the maximum number of devices per type
				foreach (var rConfig in config) {
					foreach (var deviceType in devicesByType.Keys) {
						var deviceCount = rConfig.Count (dConfig => dConfig.Device.Type == deviceType);
						Assert.That (deviceCount, Is.LessThanOrEqualTo (devicesByType[deviceType].Count));
					}
				}

				// Each device is used at most once per round
				foreach (var rConfig in config) {
					foreach (var dConfig in rConfig) {
						int numUses = rConfig.Count (dc => dc.Device.Id == dConfig.Device.Id);
						Assert.That (numUses, Is.LessThanOrEqualTo (1));
					}
				}
			}
		}

		private int CountUIModeUsagesInConfig (List<List<DeviceConfig>> config, UIMode uiMode) {
			int count = 0;
			foreach (var rConfig in config) {
				foreach (var dConfig in rConfig) {
					if (dConfig.UIMode == uiMode) {
						count++;
					}
				}
			}
			return count;
		}

		/*
				// A UnityTest behaves like a coroutine in PlayMode
				// and allows you to yield null to skip a frame in EditMode
				[UnityTest]
				public IEnumerator DeviceConfigurationGeneratorTestWithEnumeratorPasses () {
					// Use the Assert class to test conditions.
					// yield to skip a frame
					yield return null;
				}
				 */
	}
}