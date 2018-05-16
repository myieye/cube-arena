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
	public class DeviceManagerTest : BaseSettingsTest {

		private DeviceManager deviceManager;

		[SetUp]
		public void SetUp () {
			deviceManager = DeviceManager.Instance;
			deviceManager.ResetDevices ();
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
		public void TestEnoughDevices (int numPlayers, 
				int numAndroidDevices, int numHoloLensDevices, int numDesktopDevices, bool validConfig) {
			var devicesByType = DeviceTestUtil.GetDevicesByType (numAndroidDevices, numHoloLensDevices, numDesktopDevices);
			DeviceTestUtil.RegisterDevices (deviceManager, devicesByType);

			var enoughDevices = deviceManager.EnoughDevicesAvailableForUserStudy (numPlayers);

			Assert.That (enoughDevices, Is.EqualTo (validConfig));
		}
	}
}