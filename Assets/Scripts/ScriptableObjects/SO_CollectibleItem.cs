using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CollectibleItem", menuName = "Collectibles/Item Definition", order = 1)]
public class SO_CollectibleItem : ScriptableObject {
    public enum ResourceType : uint {
        None   = 0,
        Coin   = 1,
        Health = 2, 
    }

    // ========================= Variables ==========================
    [SerializeField] private ResourceType  resource;
    [SerializeField] private int       resourceAmount;
    [SerializeField] private int       grantedScore;
    [SerializeField] private AudioClip interactionSound;

    #region "Properties"
    public ResourceType Resource { get => resource; }
    public int GrantedStatAmount { get => resourceAmount; }
    public int GrantedScore { get => grantedScore; }
    public AudioClip InteractionSound { get => interactionSound; }


    #endregion

    // ========================= Unity Code =========================
    // ========================= Custom Code =========================
    public virtual void Collect(PlayerController player) {
        switch (Resource) {
            case ResourceType.Coin:   player.AddCoins(GrantedStatAmount); break;
            case ResourceType.Health: player.AddHealth(GrantedStatAmount); break;
        }

        player.AddScore(GrantedScore);
        if (InteractionSound) { 
            // TODO: Play audio clip
        }
    }
}
