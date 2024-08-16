module.exports = {
  branches: [ 
    "main",
    {
      name: "pre-release",
      prerelease: true
    }
  ],
  plugins: [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    "@semantic-release/changelog",
    [
      "@semantic-release/github",
      {
        "assets": [{
            "path": `${process.env.ARTIFACTS_DIRECTORY}/*.nupkg`,
            "label": "AutoApiGen.nupkg" 
        }]
      }
    ],
    [
      "@semantic-release/exec",
      {
        "verifyReleaseCmd": "echo VERSION=${nextRelease.version} >> $GITHUB_ENV",
        "publishCmd": `dotnet nuget push ${process.env.ARTIFACTS_DIRECTORY}/*.nupkg --source "github" --skip-duplicate` 
      }
    ]
  ]
}
