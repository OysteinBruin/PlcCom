#tool nuget:?package=7-Zip.CommandLine
#addin nuget:?package=Cake.7zip&version=1.0.2

// ARGUMENTS

var target = Argument("target", "Default");
var solutionPath = Argument<string>("solutionPath");
var buildPath = Argument<string>("buildPath");
var appId  = Argument<string>("appId");
var appVersion = Argument<string>("appVersion");


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

// BUILD

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory(buildPath);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(solutionPath, settings =>
        settings.SetConfiguration("Release"));
});

// TEST - TODO: add test task here


// GET REQUIRED TOOLS AND FILES
Task("DownloadAzureCopy")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (!DirectoryExists("./AzureCopyTool"))
    {
        CreateDirectory("./AzureCopyTool");
    }

    using (var wc = new System.Net.WebClient())
    {
        Information("Downloading azcopy");
        DownloadFile(
            "https://autodeploytools.blob.core.windows.net/tools/azcopy.7z",
            "./AzureCopyTool/azcopy.7z" 
        );
    }
});

Task("UnzipAzureCopy")
    .IsDependentOn("DownloadAzureCopy")
    .Does(() =>
{

    SevenZip(m => m
      .InExtractMode()
      .WithArchive(File("./AzureCopyTool/azcopy.7z"))
      .WithArchiveType(SwitchArchiveType.SevenZip)
      .WithOutputDirectory("./AzureCopyTool/"));
});

Task("DownloadAutoReleaseTool")
    .IsDependentOn("UnzipAzureCopy")
    .Does(() =>
{
    if (!DirectoryExists("./AutoReleaseTool"))
    {
        CreateDirectory("./AutoReleaseTool");
    }

    using (var wc = new System.Net.WebClient())
    {
        Information("Downloading AutoReleaseTool");
        DownloadFile(
            "https://autodeploytools.blob.core.windows.net/tools/AutoReleaseTool.7z",
            "./AutoReleaseTool/AutoReleaseTool.7z" 
        );
    }

});

Task("UnzipAutoReleaseTool")
    .IsDependentOn("DownloadAutoReleaseTool")
    .Does(() =>
{

    SevenZip(m => m
      .InExtractMode()
      .WithArchive(File("./AutoReleaseTool/AutoReleaseTool.7z"))
      .WithArchiveType(SwitchArchiveType.SevenZip)
      .WithOutputDirectory("./AutoReleaseTool/"));
});

Task("DownloadPreviousReleaseFiles")
    .IsDependentOn("UnzipAutoReleaseTool")
    .Does(() =>
{
    Information("Creating folder: ./releases, downloading previous release files");
    
    
    FilePath azcopyPath = "./AzureCopyTool/azcopy.exe";
    StartProcess(azcopyPath, new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("copy")
            .Append("https://plccom.blob.core.windows.net/releases/")
            .Append("./")
            .Append("--recursive")
    });

});

// PREPARE 
//Task("CopyPreviousFilesToReleasesDir")
//    .IsDependentOn("DownloadPreviousReleaseFiles")
//    .Does(() => 
//{
//    Information("Copying retrived files from previous release up one level");
// 	  var files = GetFiles("./releases/releases/*");
//    CopyFiles(files, "./releases/", true);
//});


Task("Package")
    .IsDependentOn("DownloadPreviousReleaseFiles")
    .Does(() => 
{
    if (!DirectoryExists("./releases"))
    {
        CreateDirectory("./releases");
    }
    FilePath autoReleasePath = "./AutoReleaseTool/AutoReleaseTool.exe";
    StartProcess(autoReleasePath, new ProcessSettings {
    Arguments = new ProcessArgumentBuilder()
        .Append(buildPath)
        .Append(appId)
        .Append(appVersion)
    });
});


// EXECUTION
Task("Default").IsDependentOn("Package");

RunTarget(target);
