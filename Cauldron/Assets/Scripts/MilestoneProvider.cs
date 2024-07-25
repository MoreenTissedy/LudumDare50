using System.Collections.Generic;
using UnityEngine;

public class MilestoneProvider
{
    private PlayerProgressProvider progressProvider;
    private List<string> milestones;

    public void Init(PlayerProgressProvider progressProvider)
    {
        this.progressProvider = progressProvider;
        milestones = progressProvider.GetMilestones();
    }

    public void SaveMilestone(string tag)
    {
        if (!milestones.Contains(tag))
        {
            milestones.Add(tag);
            progressProvider.SaveMilestones(milestones);
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
            progressProvider.SaveMilestones(milestones);
            return true;
        }

        return false;
    }
}
