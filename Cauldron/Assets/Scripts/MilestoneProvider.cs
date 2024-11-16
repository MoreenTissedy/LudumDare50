using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    public class MilestoneProvider
    {
        public List<string> Milestones;

        private readonly string fileName = "Milestones";
        private readonly FileDataHandler<ListToSave<string>> fileDataHandler;

        public MilestoneProvider()
        {
            fileDataHandler = new FileDataHandler<ListToSave<string>>(fileName);
        }

        public void SaveMilestone(string tag)
        {
            if (!Milestones.Contains(tag))
            {
                Milestones.Add(tag);
                Save();
            }
        }

        private void Save()
        {
            fileDataHandler.Save(new ListToSave<string>(Milestones));
        }

        public List<string> GetMilestones()
        {
            if (Milestones.Count > 0)
            {
                RunCompatibilityUpdate();

                return Milestones;
            }

            return new List<string>();
        }

        private void RunCompatibilityUpdate()
        {
            if (RemoveMilestone("bishops sister cured"))
            {
                Debug.Log("[Compatibility] Removed 'bishops sister cured' milestone");
            }
        }

        public bool RemoveMilestone(string tag)
        {
            if (Milestones.Count == 0)
            {
                return false;
            }

            if (Milestones.Remove(tag))
            {
                Save();
                return true;
            }

            return false;
        }

        public void LoadMilestones()
        {
            if (TryLoadLegacy(out var legacyProgress))
            {
                Milestones = legacyProgress;
                Save();
            }

            Milestones = fileDataHandler.IsFileValid() ? fileDataHandler.Load().list : new List<string>();
        }

        private bool TryLoadLegacy(out List<string> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                list = null;
                return false;
            }

            list = JsonUtility.FromJson<StringListWrapper>(PlayerPrefs.GetString(PrefKeys.Milestones)).list;
            PlayerPrefs.DeleteKey(PrefKeys.Milestones);
            return true;
        }
    }
}