using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObjects/Create new ball")]
public class BallData : ScriptableObject
{
    public Sprite ballSprite;
    public GameObject ballEffectPrefab;
    public AudioClip ballCollisionSound;
    public TypeValue typeValue;
    public int amount;
}

public enum TypeValue
{
    Money = 1, Ads = 2
}