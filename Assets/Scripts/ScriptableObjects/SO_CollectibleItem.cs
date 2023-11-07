using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CollectibleItem", menuName = "Collectibles/Item Definition", order = 1)]
public class SO_CollectibleItem : ScriptableObject {
    public enum ItemType : uint {
        None   = 0,
        COIN   = 1,
        HEALTH = 2, 
    }

    // ========================= Variables ==========================
    [SerializeField] private int       resourceAmount   = 0;
    [SerializeField] private ItemType  resourceType     = ItemType.COIN;
    [SerializeField] private AudioClip interactionSound = null;

    #region "Properties"
    public ItemType ResourceType { get => resourceType; }
    public int GrantedStatAmount { get => resourceAmount; }
    public AudioClip InteractionSound { get => interactionSound; }


    #endregion

    // ========================= Unity Code =========================
    // ========================= Custom Code =========================
    public virtual void Collect(PlayerController player) {
        switch (ResourceType) {
            case ItemType.COIN:  player.AddScore(GrantedStatAmount);  break;
            case ItemType.HEALTH: player.AddHealth(GrantedStatAmount); break;
        }

        if (InteractionSound) { 
            // TODO: Play audio clip
        }
    }
}
