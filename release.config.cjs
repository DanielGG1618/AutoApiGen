module.exports = {
  branches: [ "main" ],
  plugins: [
    "@semantic-release/commit-analyzer",
    "@semantic-release/changelog",
    "@semantic-release/git",
    [
      "@semantic-release/github",
      {
        "assets": [{
            "path": `${process.env.ARTIFACTS_DIRECTORY}/*.nupkg`,
            "label": "AutoApiGen" 
        }]
      }
    ],
    [
      "@semantic-release/exec",
      {
        verifyReleaseCmd: "echo VERSION=${nextRelease.version} >> $GITHUB_ENV",
        publish: `dotnet nuget push ${process.env.ARTIFACTS_DIRECTORY}/*.nupkg --source "github" --skip-duplicate` 
      }
    ]
  ]
}
