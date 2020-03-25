using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private bool _startupAdShown = false;
    private bool _queueRewardedVideo = false;

    void Update()
    {
        if (!_startupAdShown)
        {
            if (Advertisement.IsReady())
            {
                //Advertisement.Show("StartupAd");
                _startupAdShown = true;
            }
        }

        if (_queueRewardedVideo)
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show("rewardedVideo",  new ShowOptions { resultCallback = OnRewardedVideoClosed });
                _queueRewardedVideo = false;
            }
        }
    }

    public void ShowRewardedVideo()
    {
        _queueRewardedVideo = true;
    }

    public void OnRewardedVideoClosed(ShowResult result)
    {
        if (result == ShowResult.Finished)
            GameManager.Instance.player.Reset();
    }
}
