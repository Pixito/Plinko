using System.Collections;
using System.Collections.Generic;
using TikTokLiveSharp.Events;
using TikTokLiveSharp.Events.Objects;
using TikTokLiveUnity;
using TikTokLiveUnity.Utils;
using UnityEngine;
using UnityEngine.UI;

public class PlinkoManager : MonoBehaviour
{
    public GameObject Player;
    private TikTokLiveManager mgr => TikTokLiveManager.Instance;

    public void OnGift(TikTokGift gift)
    {
        if (gift.Gift.Type == 1) // Streak // rose, donuts...
        {
            gift.OnStreakFinished += OnStreakFinish;
        }
        else // Unstreak // chapeau cowboy, gant de boxe, tiktok gun...
        {
            CreatePlayer(gift.Gift.DiamondCost, gift.Sender.AvatarThumbnail);
            gift.OnAmountChanged += OnAmountChanged;
        }
    }

    private void OnAmountChanged(TikTokGift gift, long change, long newAmount)
    {
        CreatePlayer(gift.Gift.DiamondCost, gift.Sender.AvatarThumbnail);
    }

    private void OnStreakFinish(TikTokGift gift, long args)
    {
        CreatePlayer(gift.Gift.DiamondCost * (int)args, gift.Sender.AvatarThumbnail);
        gift.OnStreakFinished -= OnStreakFinish;
    }

    public void OnLike(Like like)
    {
        if (like.Count >= 15) // 15 likes = 1 player
            CreatePlayer(1, like.Sender.AvatarThumbnail);
    }

    public void OnShare(Share share)
    {
        CreatePlayer(1, share.User.AvatarThumbnail);
    }

    public void OnFollow(Follow follow)
    {
        CreatePlayer(1, follow.User.AvatarThumbnail);
    }

    private int playerNumber;

    private void CreatePlayer(int quantity, Picture picture)
    {
        if (quantity > 100)
        {
            quantity = 100;
        }

        for (int i = 0; i < quantity; i++)
        {
            var ball = Instantiate(Player, transform);
            ball.transform.localPosition = new Vector3(Random.Range(-100f, 100f), transform.position.y, 0f);
            RequestImage(ball.GetComponent<SetImage>().SetPlayerImage(), picture);
        }

        playerNumber += quantity;
        if (playerNumber > 200)
        {
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach(var player in allPlayers)
            {
                Destroy(player);
            }
            playerNumber = 0;
        }
    }

    private void RequestImage(Image img, Picture picture)
    {
        Dispatcher.RunOnMainThread(() =>
        {
            mgr.RequestSprite(picture, spr =>
            {
                if (img != null && img.gameObject != null && img.gameObject.activeInHierarchy)
                    img.sprite = spr;
            });
        });
    }
}
