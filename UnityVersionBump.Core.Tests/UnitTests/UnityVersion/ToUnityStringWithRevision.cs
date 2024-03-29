﻿using NUnit.Framework;

namespace UnityVersionBump.Core.Tests.UnitTests.UnityVersion;

public class ToUnityStringWithRevision
{
    private static readonly object[] Versions = {
        new[]{"2022.2.0f1", "6cf78cb77498"},
        new[]{"2021.2.0", "6cf78cb77498"},
        new[]{"2021.1.1b15", "6cf78cb77498"},
        new[]{"2021.1.0a15", "6cf78cb77498"}
    };

    [TestCaseSource(nameof(Versions))]
    public void ToUnityStringGeneratesUnityConformFormat(string unityVersion, string revision)
    {
        Assert.That(new Core.UnityVersion(unityVersion, revision, false).ToUnityStringWithRevision(), Is.EqualTo($"{unityVersion} ({revision})"));
    }
}