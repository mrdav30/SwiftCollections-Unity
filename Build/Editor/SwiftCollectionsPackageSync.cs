#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SwiftCollections.Build.Editor
{
    /// <summary>
    /// Synchronizes shared managed source files from Build overlays into package variants.
    /// </summary>
    public static class SwiftCollectionsPackageSync
    {
        // These paths are intentionally explicit so package-specific files can live beside
        // shared code without being deleted during sync.
        private static readonly ManagedEntry[] BaseManagedEntries =
        {
            ManagedEntry.Directory("Runtime/GameObjectPool"),
            ManagedEntry.File("Runtime/BoundVolume.Extensions.cs"),
            ManagedEntry.Directory("Samples/SwiftCollectionsDemo/Scripts")
        };

        private static readonly ManagedEntry[] FixedMathSharpManagedEntries =
        {
            ManagedEntry.File("Runtime/FixedBoundVolume.Extensions.cs")
        };

        private static readonly PackageOverlay[] PackageOverlays =
        {
            new PackageOverlay(
                "Assets/Packages/Build/Base",
                new[]
                {
                    "Assets/Packages/com.mrdav30.swiftcollections",
                    "Assets/Packages/com.mrdav30.swiftcollections.lean"
                },
                BaseManagedEntries),
            new PackageOverlay(
                "Assets/Packages/Build/FixedMathSharp",
                new[]
                {
                    "Assets/Packages/com.mrdav30.swiftcollections.fixedmathsharp",
                    "Assets/Packages/com.mrdav30.swiftcollections.fixedmathsharp.lean"
                },
                FixedMathSharpManagedEntries)
        };

        [MenuItem("Tools/SwiftCollections/Sync Managed Package Files")]
        public static void SyncPackagesMenu()
        {
            SyncPackages();
        }

        public static void SyncPackagesBatchMode()
        {
            var exitCode = 0;

            try
            {
                SyncPackages();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                exitCode = 1;
            }

            EditorApplication.Exit(exitCode);
        }

        public static void SyncPackages()
        {
            ValidatePaths();

            var summary = new SyncSummary();

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var packageOverlay in PackageOverlays)
                {
                    foreach (var packageRootAssetPath in packageOverlay.PackageRootAssetPaths)
                        summary.Merge(SyncPackage(packageOverlay, packageRootAssetPath));
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            Debug.Log(
                $"SwiftCollections package sync complete. " +
                $"Copied {summary.CopiedFiles} files, deleted {summary.DeletedFiles} files, " +
                $"removed {summary.DeletedDirectories} directories across " +
                $"{PackageOverlays.Length} overlays and {CountPackageRoots()} packages.");
        }

        private static void ValidatePaths()
        {
            foreach (var packageOverlay in PackageOverlays)
            {
                if (!Directory.Exists(ToAbsolutePath(packageOverlay.SourceRootAssetPath)))
                {
                    throw new DirectoryNotFoundException(
                        $"Shared package source folder not found: {packageOverlay.SourceRootAssetPath}");
                }

                foreach (var packageRootAssetPath in packageOverlay.PackageRootAssetPaths)
                {
                    if (!Directory.Exists(ToAbsolutePath(packageRootAssetPath)))
                        throw new DirectoryNotFoundException($"Package folder not found: {packageRootAssetPath}");
                }
            }
        }

        private static SyncSummary SyncPackage(PackageOverlay packageOverlay, string packageRootAssetPath)
        {
            var summary = new SyncSummary();

            foreach (var managedEntry in packageOverlay.ManagedEntries)
            {
                summary.Merge(managedEntry.Kind == ManagedEntryKind.Directory
                    ? SyncManagedDirectory(packageOverlay.SourceRootAssetPath, packageRootAssetPath, managedEntry.RelativePath)
                    : SyncManagedFile(packageOverlay.SourceRootAssetPath, packageRootAssetPath, managedEntry.RelativePath));
            }

            return summary;
        }

        private static SyncSummary SyncManagedDirectory(
            string sourceRootAssetPath,
            string packageRootAssetPath,
            string relativePath)
        {
            var sourceDirectory = ToAbsolutePath(CombineAssetPath(sourceRootAssetPath, relativePath));
            var destinationDirectory = ToAbsolutePath(CombineAssetPath(packageRootAssetPath, relativePath));
            var summary = new SyncSummary();

            var sourceFiles = Directory.Exists(sourceDirectory)
                ? CollectManagedFiles(sourceDirectory)
                : new Dictionary<string, string>(StringComparer.Ordinal);
            var destinationFiles = Directory.Exists(destinationDirectory)
                ? CollectManagedFiles(destinationDirectory)
                : new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var destinationRelativePath in destinationFiles.Keys)
            {
                if (sourceFiles.ContainsKey(destinationRelativePath))
                    continue;

                DeleteManagedFile(destinationFiles[destinationRelativePath], ref summary);
            }

            foreach (var pair in sourceFiles)
            {
                var destinationPath = Path.Combine(destinationDirectory, pair.Key);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? destinationDirectory);

                if (FilesAreEqual(pair.Value, destinationPath))
                    continue;

                File.Copy(pair.Value, destinationPath, true);
                summary.CopiedFiles++;
            }

            RemoveEmptyManagedDirectories(destinationDirectory, ref summary);
            return summary;
        }

        private static SyncSummary SyncManagedFile(
            string sourceRootAssetPath,
            string packageRootAssetPath,
            string relativePath)
        {
            var sourcePath = ToAbsolutePath(CombineAssetPath(sourceRootAssetPath, relativePath));
            var destinationPath = ToAbsolutePath(CombineAssetPath(packageRootAssetPath, relativePath));
            var summary = new SyncSummary();

            if (!File.Exists(sourcePath))
            {
                DeleteManagedFile(destinationPath, ref summary);
                return summary;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? ToAbsolutePath(packageRootAssetPath));

            if (!FilesAreEqual(sourcePath, destinationPath))
            {
                File.Copy(sourcePath, destinationPath, true);
                summary.CopiedFiles++;
            }

            return summary;
        }

        private static Dictionary<string, string> CollectManagedFiles(string rootPath)
        {
            var files = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var filePath in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories))
            {
                if (IsMetaFile(filePath))
                    continue;

                var relativePath = Path.GetRelativePath(rootPath, filePath)
                    .Replace('\\', '/');
                files[relativePath] = filePath;
            }

            return files;
        }

        private static void DeleteManagedFile(string filePath, ref SyncSummary summary)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                summary.DeletedFiles++;
            }

            DeleteMetaFileIfPresent(filePath);
        }

        private static void RemoveEmptyManagedDirectories(string rootPath, ref SyncSummary summary)
        {
            if (!Directory.Exists(rootPath))
                return;

            foreach (var directoryPath in EnumerateDirectoriesDeepestFirst(rootPath))
            {
                if (DirectoryHasEntries(directoryPath))
                    continue;

                DeleteMetaFileIfPresent(directoryPath);
                Directory.Delete(directoryPath);
                summary.DeletedDirectories++;
            }

            if (!Directory.Exists(rootPath) || DirectoryHasEntries(rootPath))
                return;

            DeleteMetaFileIfPresent(rootPath);
            Directory.Delete(rootPath);
            summary.DeletedDirectories++;
        }

        private static IEnumerable<string> EnumerateDirectoriesDeepestFirst(string rootPath)
        {
            var directories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);
            Array.Sort(directories, (left, right) => string.CompareOrdinal(right, left));
            return directories;
        }

        private static bool DirectoryHasEntries(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return false;

            foreach (var _ in Directory.EnumerateFileSystemEntries(directoryPath))
                return true;

            return false;
        }

        private static void DeleteMetaFileIfPresent(string assetPath)
        {
            var metaPath = assetPath + ".meta";
            if (File.Exists(metaPath))
                File.Delete(metaPath);
        }

        private static bool FilesAreEqual(string sourcePath, string destinationPath)
        {
            if (!File.Exists(destinationPath))
                return false;

            var sourceInfo = new FileInfo(sourcePath);
            var destinationInfo = new FileInfo(destinationPath);
            if (sourceInfo.Length != destinationInfo.Length)
                return false;

            var sourceBytes = File.ReadAllBytes(sourcePath);
            var destinationBytes = File.ReadAllBytes(destinationPath);
            if (sourceBytes.Length != destinationBytes.Length)
                return false;

            for (var i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] != destinationBytes[i])
                    return false;
            }

            return true;
        }

        private static int CountPackageRoots()
        {
            var packageRoots = new HashSet<string>(StringComparer.Ordinal);

            foreach (var packageOverlay in PackageOverlays)
            {
                foreach (var packageRootAssetPath in packageOverlay.PackageRootAssetPaths)
                    packageRoots.Add(packageRootAssetPath);
            }

            return packageRoots.Count;
        }

        private static bool IsMetaFile(string path)
        {
            return path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase);
        }

        private static string CombineAssetPath(string left, string right)
        {
            return $"{left.TrimEnd('/')}/{right.TrimStart('/')}";
        }

        private static string ToAbsolutePath(string assetPath)
        {
            if (!assetPath.StartsWith("Assets/", StringComparison.Ordinal) &&
                !string.Equals(assetPath, "Assets", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Expected an Assets-relative path, got {assetPath}");
            }

            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            return Path.GetFullPath(Path.Combine(projectRoot, assetPath));
        }

        private enum ManagedEntryKind
        {
            File,
            Directory
        }

        private struct ManagedEntry
        {
            public ManagedEntryKind Kind { get; }
            public string RelativePath { get; }

            private ManagedEntry(ManagedEntryKind kind, string relativePath)
            {
                Kind = kind;
                RelativePath = relativePath;
            }

            public static ManagedEntry File(string relativePath)
            {
                return new ManagedEntry(ManagedEntryKind.File, relativePath);
            }

            public static ManagedEntry Directory(string relativePath)
            {
                return new ManagedEntry(ManagedEntryKind.Directory, relativePath);
            }
        }

        private struct PackageOverlay
        {
            public string SourceRootAssetPath { get; }
            public string[] PackageRootAssetPaths { get; }
            public ManagedEntry[] ManagedEntries { get; }

            public PackageOverlay(
                string sourceRootAssetPath,
                string[] packageRootAssetPaths,
                ManagedEntry[] managedEntries)
            {
                SourceRootAssetPath = sourceRootAssetPath;
                PackageRootAssetPaths = packageRootAssetPaths;
                ManagedEntries = managedEntries;
            }
        }

        private struct SyncSummary
        {
            public int CopiedFiles;
            public int DeletedFiles;
            public int DeletedDirectories;

            public void Merge(SyncSummary other)
            {
                CopiedFiles += other.CopiedFiles;
                DeletedFiles += other.DeletedFiles;
                DeletedDirectories += other.DeletedDirectories;
            }
        }
    }
}
#endif
