namespace SeeGit.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public static class GitExtensions
    {
        public static IEnumerable<string> GetUnreachableCommitShas(string gitRepositoryPath)
        {
            var process = new Process
                {
                    StartInfo =
                        {
                            FileName = "git",
                            Arguments = "fsck --unreachable --no-reflogs",
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            WorkingDirectory = gitRepositoryPath
                        }
                };
            process.Start();
            var commitShas = process.StandardOutput.ReadToEnd()
                                    .Split('\n')
                                    .Where(r => r.StartsWith("unreachable commit"))
                                    .Select(r => r.Replace("unreachable commit", String.Empty).Trim());
            process.WaitForExit();
            // PRN throw on non-zero exit code? i.e. if git repo trust issue with git repo when run git fsck
            return commitShas;
        }
    }
}