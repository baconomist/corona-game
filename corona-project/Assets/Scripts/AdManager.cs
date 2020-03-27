using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    public bool startupAdShown = false;
    public bool queueRewardedVideo = false;

    void Update()
    {
        if (!startupAdShown)
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show("StartupAd", new ShowOptions() {resultCallback = OnStartupAdClosed});
            }
        }

        if (queueRewardedVideo)
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show("rewardedVideo",  new ShowOptions { resultCallback = OnRewardedVideoClosed });
                queueRewardedVideo = false;
            }
        }
    }

    public void ShowRewardedVideo()
    {
        queueRewardedVideo = true;
    }

    public void OnStartupAdClosed(ShowResult result)
    {
        startupAdShown = true;
    }

    public void OnRewardedVideoClosed(ShowResult result)
    {
        if (result == ShowResult.Finished)
            GameManager.Instance.player.Reset();
    }
}
