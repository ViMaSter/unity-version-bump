﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityVersionBump.Core.Tests.Stubs;

namespace UnityVersionBump.Core.Tests.UnitTests.EditorPullRequestManager.SkipsIfNoNewerVersion;

class DeletesDanglingBranches
{
    private Core.EditorPullRequestManager.RepositoryInfo repositoryInfo;
    private Core.EditorPullRequestManager.CommitInfo commitInfo;
    private HttpClient mockedHTTPClient;

    public DeletesDanglingBranches()
    {
        repositoryInfo = new Core.EditorPullRequestManager.RepositoryInfo
        {
            RelativePathToUnityProject = "",
            UserName = "ViMaSter",
            RepositoryName = "unity-version-bump-test-fixture"
        };
        commitInfo = new Core.EditorPullRequestManager.CommitInfo
        {
            APIToken = "ghp_api",
            FullName = "UnityVersionBump (bot)",
            EmailAddress = "unity-version-bump@vincent.mahn.ke",
            PullRequestLabels = new[] { "autoupdate" },
            PullRequestPrefix = "unity-version-bump"
        };

        mockedHTTPClient = new HttpClient(new LocalFileMessageHandler("UnitTests.EditorPullRequestManager.Resources.ExistingDanglingBranches")).SetupGitHub(repositoryInfo, commitInfo);
    }

    [TestCase]
    public async Task RunWithNewerStableVersion()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "234567890abc", true);

        var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(mockedHTTPClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
        Assert.That(alreadyUpToDatePR, Is.Not.Null);

        Assert.That(await Core.EditorPullRequestManager.CreatePullRequestIfTargetVersionNewer(
            mockedHTTPClient,
            commitInfo,
            repositoryInfo,
            currentVersion,
            highestVersion
        ), Is.Empty);
    }

    [TestCase]
    public async Task RunWithNewerAlphaVersion()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "234567890abc", true);

        var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(mockedHTTPClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
        Assert.That(alreadyUpToDatePR, Is.Not.Null);

        Assert.That(await Core.EditorPullRequestManager.CreatePullRequestIfTargetVersionNewer(
            mockedHTTPClient,
            commitInfo,
            repositoryInfo,
            currentVersion,
            highestVersion
        ), Is.Empty);
    }

    [TestCase]
    public async Task RunWithIdenticalVersion()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "1234567890ab", true);

        var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(mockedHTTPClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
        Assert.That(alreadyUpToDatePR, Is.Not.Null);

        Assert.That(await Core.EditorPullRequestManager.CreatePullRequestIfTargetVersionNewer(
            mockedHTTPClient,
            commitInfo,
            repositoryInfo,
            currentVersion,
            highestVersion
        ), Is.Empty);
    }

    [TestCase]
    public async Task RunWithLowerVersion()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "234567890abc", true);

        var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(mockedHTTPClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
        Assert.That(alreadyUpToDatePR, Is.Not.Null);

        Assert.That(await Core.EditorPullRequestManager.CreatePullRequestIfTargetVersionNewer(
            mockedHTTPClient,
            commitInfo,
            repositoryInfo,
            currentVersion,
            highestVersion
        ), Is.Empty);
    }

    [TestCase]
    public async Task SucceedsIfNoLabel()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "234567890abc", true);

        var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(mockedHTTPClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
        Assert.That(alreadyUpToDatePR, Is.Not.Null);

        Assert.That(await Core.EditorPullRequestManager.CreatePullRequestIfTargetVersionNewer(
            mockedHTTPClient,
            new Core.EditorPullRequestManager.CommitInfo
            {
                APIToken = "ghp_api",
                FullName = "UnityVersionBump (bot)",
                EmailAddress = "unity-version-bump@vincent.mahn.ke",
                PullRequestLabels = Array.Empty<string>(),
                PullRequestPrefix = "unity-version-bump"
            },
            repositoryInfo,
            currentVersion,
            highestVersion
        ), Is.Empty);
    }

    [TestCase]
    public void FailsIfBranchCantBeDeleted()
    {
        var currentVersion = new Core.UnityVersion("2022.2.0p1", "1234567890ab", true);
        var highestVersion = new Core.UnityVersion("2022.2.0a17", "234567890abc", true);
        var httpClient = new HttpClient(new LocalFileMessageHandler("UnitTests.EditorPullRequestManager.Resources.FailedToDeleteBranch")).SetupGitHub(repositoryInfo, commitInfo);

        Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var alreadyUpToDatePR = await Core.EditorPullRequestManager.CleanupAndCheckForAlreadyExistingPR(httpClient, commitInfo, repositoryInfo, currentVersion, highestVersion);
            Assert.That(alreadyUpToDatePR, Is.Not.Null);
        });
    }
}
