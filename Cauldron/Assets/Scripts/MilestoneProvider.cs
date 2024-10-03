using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    public class MilestoneProvider
    {
        public List<string> milestones;
        
        private readonly string fileName = "Milestones";
        private FileDataHandler<ListToSave<string>> fileDataHandler;

        public MilestoneProvider()
        {
            fileDataHandler  = new FileDataHandler<ListToSave<string>>(fileName);
            milestones = LoadMilestones();
        }

        public void Update()
        {
            milestones = LoadMilestones();
        }

        public void SaveMilestone(string tag)
        {
            if (!milestones.Contains(tag))
            {
                milestones.Add(tag);
                Save();
            }
        }

        private void Save()
        {
            fileDataHandler.Save(new ListToSave<string>(milestones));
        }

        public List<string> GetMilestones()
        {
            if (milestones.Count > 0)
            {
                RunCompatibilityUpdate();
                
                return milestones;
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
            if (milestones.Count == 0)
            {
                return false;
            }
            if (milestones.Remove(tag))
            {
                Save();
                return true;
            }

            return false;
        }

        private List<string> LoadMilestones()
        {
            if (TryLoadLegacy(out var legacyProgress))
            {
                return legacyProgress;
            }
            return fileDataHandler.IsFileValid() ? fileDataHandler.Load().list : new List<string>();
        }

        private bool TryLoadLegacy(out List<string> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                list = null;
                return false;
            }

            list = PlayerPrefs.GetString(PrefKeys.Milestones).Split(',').ToList();
            PlayerPrefs.DeleteKey(PrefKeys.Milestones);
            return true;
        }
    }
}