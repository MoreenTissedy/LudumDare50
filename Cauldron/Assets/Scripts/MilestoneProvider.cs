using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    public class MilestoneProvider
    {
        private PlayerProgressProvider progressProvider;
        private List<string> milestones;

        public void Init(PlayerProgressProvider progressProvider)
        {
            this.progressProvider = progressProvider;
            milestones = progressProvider.Milestones;
        }

        public void SaveMilestone(string tag)
        {
            if (!milestones.Contains(tag))
            {
                milestones.Add(tag);
                progressProvider.SaveProgress();
            }
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
                progressProvider.SaveProgress();
                return true;
            }

            return false;
        }
    }
}