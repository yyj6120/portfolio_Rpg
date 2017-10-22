using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rpg.Character;
using UnityEngine.Events;
using System;

namespace Rpg.Item
{
    public class ItemCollection : TriggerGenericAction
    {
        [Header("--- Item Collection Options ---")]
        public float onSoundDelay;
        public float onCollectDelay;

        [Tooltip("Immediately equip the item ignoring the Equip animation")]
        public bool immediate = false;

        [Header("---Items List Data---")]
        public ItemListData itemListData;

        [Header("---Items Filter---")]
        public List<ItemType> itemsFilter = new List<ItemType>() { 0 };

        [HideInInspector]
        public List<ItemReference> items = new List<ItemReference>();

        public RewardWindow rewardWindow;
        public RewardWindow currentRewardWindow;
        public UnityAction rewardCloseButtonAction;
        public AudioSource audioSource;
        public AudioClip[] audioClips;
        protected override void Start()
        {
            base.Start();
            rewardCloseButtonAction = OffAction;
            if (destroyAfter && destroyDelay < onCollectDelay)
            {
                destroyDelay = onCollectDelay + 0.25f;
            }
        }

        public void CollectItems(ItemManager itemManager)
        {
            if (items.Count > 0)
            {
                StartCoroutine(SetItemsToItemManager(itemManager));
            }
        }

        IEnumerator SetItemsToItemManager(ItemManager itemManager)
        {
            yield return new WaitForSeconds(onSoundDelay);

            GameObject audioObject = null;
            if (audioSource != null)
                audioObject = Instantiate(audioSource.gameObject, transform.position, Quaternion.identity) as GameObject;

            if (audioObject != null)
            {
                var source = audioObject.gameObject.GetComponent<AudioSource>();
                source.PlayOneShot(audioClips[0]);
            }
            yield return new WaitForSeconds(onCollectDelay);

            if (currentRewardWindow != null)
            {
                currentRewardWindow.gameObject.SetActive(true);
            }
            else
            {
                RewardWindow reward = Instantiate(rewardWindow);
                currentRewardWindow = reward;
                reward.itemCollection = this;
                reward.closeWindow.onClick.AddListener(rewardCloseButtonAction);
                reward.CollectItem(items, itemManager);
                reward.CreateRewardWindow();
            }
        }

        public void OffAction()
        {
            OffDoAction.Invoke();
            GameObject audioObject = null;
            if (audioSource != null)
                audioObject = Instantiate(audioSource.gameObject, transform.position, Quaternion.identity) as GameObject;
            
            if (audioObject != null)
            {
                var source = audioObject.gameObject.GetComponent<AudioSource>();
                source.PlayOneShot(audioClips[1]);
            }
        }

    }
}

