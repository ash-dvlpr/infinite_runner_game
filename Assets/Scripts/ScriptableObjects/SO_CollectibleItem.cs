using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CollectibleItem", menuName = "Collectibles/Item Definition", order = 1)]
public class SO_CollectibleItem : ScriptableObject {
    public enum ResourceType : uint {
        None   = 0,
        Score  = 1,
        Health = 2, 
    }

    // ========================= Variables ==========================
    [SerializeField] private ResourceType  resource;
    [SerializeField] private int       resourceAmount;
    [SerializeField] private AudioClip interactionSound;

    #region "Properties"
    public ResourceType Resource { get => resource; }
    public int GrantedStatAmount { get => resourceAmount; }
    public AudioClip InteractionSound { get => interactionSound; }


    #endregion

    // ========================= Unity Code =========================
    // ========================= Custom Code =========================
    public virtual void Collect(PlayerController player) {
        switch (Resource) {
            case ResourceType.Score:  player.AddScore(GrantedStatAmount);  break;
            case ResourceType.Health: player.AddHealth(GrantedStatAmount); break;
        }

        if (InteractionSound) { 
            // TODO: Play audio clip
        }
    }
}
