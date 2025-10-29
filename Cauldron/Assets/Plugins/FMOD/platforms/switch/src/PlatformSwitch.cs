using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_SWITCH && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "__Internal";
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "__Internal";
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformSwitch : Platform
    {
        static PlatformSwitch()
        {
            Settings.AddPlatformTemplate<PlatformSwitch>("dc6af03a6d487a943bd07c6a5a1aafcc");
        }

        internal override string DisplayName { get { return "Switch"; } }
        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.Switch, this);
        }

#if UNITY_EDITOR
        internal override Legacy.Platform LegacyIdentifier { get { return Legacy.Platform.Switch; } }
#endif

        internal override string GetBankFolder()
        {
            string bankFolder = base.GetBankFolder();

            if (bankFolder[0] == '/')
            {
                bankFolder = bankFolder.Substring(1);
            }

            return bankFolder;
        }

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.Switch;
        }

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("switch", "Plugins/Switch");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants, string suffix)
        {
            yield return new FileRecord(string.Format("libfmodstudiounityplugin{0}.a", suffix));
        }

        protected override IEnumerable<FileRecord> GetOptionalBinaryFiles(BuildTarget buildTarget, bool allVariants)
        {
            yield return new FileRecord("libresonanceaudio.a");
        }

        protected override IEnumerable<FileRecord> GetSourceFiles()
        {
            yield return new FileRecord("fmod_switch.cs");
        }

        internal override bool IsFMODStaticallyLinked { get { return true; } }

        internal override OutputType[] ValidOutputTypes
        {
            get
            {
                return sValidOutputTypes;
            }
        }

        private static OutputType[] sValidOutputTypes = {
           new OutputType() { displayName = "nn::audio", outputType = FMOD.OUTPUTTYPE.NNAUDIO },
        };

        internal override int CoreCount { get { return 3; } }
#endif
    }
}
