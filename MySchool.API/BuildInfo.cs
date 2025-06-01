// -----------------------------------------------------------------------------
// This file is auto-generated. Do not modify directly.
//------------------------------------------------------------------------------
using System.Runtime.InteropServices;
public static class BuildInfo
{
public const string BuildTime = "2025-06-01 12:21:37";
public static string Platform =>
RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown";

public static string Framework => RuntimeInformation.FrameworkDescription;
}
