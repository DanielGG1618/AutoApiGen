/**
 * @type {import('semantic-release').GlobalConfig}
 */

module.exports = {
  branches: [
    "main",
    "semantic-release-setup"
  ],
  plugins: [
    "@semantic-release/commit-analyzer",
    "@semantic-release/changelog",
    "@semantic-release/git",
    "@semantic-release/github",
    [
      "@semantic-release/exec",
      {
        verifyReleaseCmd: "echo version=${nextRelease.version} >> $GITHUB_ENV",
        publish: `dotnet nuget push ${process.env.ARTIFACTS_DIRECTORY}/*.nupkg --source "github" --skip-duplicate` 
      }
    ]
  ]
}
