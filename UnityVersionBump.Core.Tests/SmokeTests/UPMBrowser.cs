﻿using System;
using System.Net.Http;
using NUnit.Framework;

namespace UnityVersionBump.Core.Tests.SmokeTests
{
    class UPMBrowser
    {
        [TestCase]
        public void CanGetVersionFromOfficialPackageRepository()
        {
            var stubbedClient = new HttpClient();

            const string PACKAGE_NAME = "com.unity.2d.ik";
            var browser = new UPM.Browser(stubbedClient, UPM.Browser.UNITY_DEFAULT_PACKAGE_REPOSITORY_ROOT);

            Assert.DoesNotThrowAsync(async () =>
            {
                var version = await browser.GetLatestVersion(PACKAGE_NAME);
                Assert.Greater(version.GetVersionPart(PackageVersion.VersionPart.Major), 0);
            });

        }
        [TestCase]
        public void CanGetVersionFromOpenUPM()
        {
            var stubbedClient = new HttpClient();

            const string PACKAGE_NAME = "com.inklestudios.ink-unity-integration";
            var browser = new UPM.Browser(stubbedClient, "https://package.openupm.com");

            Assert.DoesNotThrowAsync(async () =>
            {
                var version = await browser.GetLatestVersion(PACKAGE_NAME);
                Assert.Greater(version.GetVersionPart(PackageVersion.VersionPart.Major), 0);
            });
        }
        [TestCase]
        public void ThrowsOn404FromOfficialPackageRepository()
        {
            var stubbedClient = new HttpClient();

            const string PACKAGE_NAME = "com.inklestudios.ink-unity-integration";
            var browser = new UPM.Browser(stubbedClient, UPM.Browser.UNITY_DEFAULT_PACKAGE_REPOSITORY_ROOT);

            var exception = Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                await browser.GetLatestVersion(PACKAGE_NAME);
            })!;
            StringAssert.Contains("Unexpected return code 404", exception.Message);

        }
        [TestCase]
        public void ThrowsOn404FromOpenUPM()
        {
            var stubbedClient = new HttpClient();

            const string PACKAGE_NAME = "com.unity.2d.ik";
            var browser = new UPM.Browser(stubbedClient, "https://package.openupm.com");

            var exception = Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                await browser.GetLatestVersion(PACKAGE_NAME);
            })!;
            StringAssert.Contains("Unexpected return code 404", exception.Message);
        }
    }
}
