module.exports = 
{
    verifyConditions: 
    [
        '@semantic-release/changelog',
        '@semantic-release/git',
        '@semantic-release/github',
    ],
  
    publish: 
    [
        '@semantic-release/changelog',
        '@semantic-release/git',
        {
            path: '@semantic-release/exec',
            cmd: `dotnet nuget push ${process.env.ARTIFACTS_DIRECTORY}/*.nupkg --source "github" --skip-duplicate`
        },
        {
            path: '@semantic-release/github',
            assets: `${process.env.ARTIFACTS_DIRECTORY}/*.nupkg`,
        },
    ],
}
