using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Instance;
    public bool isAlive;
    public int health;
    SpriteRenderer ship;
    public GameObject shipSprite;
    public GameObject gameOverMenu;

    public TextMeshProUGUI hpText;
    public readonly SyncVar<int> hp = new SyncVar<int>();

    private void OnHealthChanged(int oldValue, int newValue, bool asServer)
    {
        hpText.text = newValue.ToString();
    }

    private void Awake()
    {
        Instance = this;
        hp.OnChange += OnHealthChanged;
    }

    private void Start()
    {
        ship = GetComponentInChildren<SpriteRenderer>();
        health = 10;
        isAlive = true;
        hp.Value = 10;
    }

    private void Update()
    {
        if(health <= 0)
        {
            //ship.GetComponent<SpriteRenderer>().enabled = false;
            Despawn(shipSprite);
            isAlive = false;
            ShowGameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            health--;
            UpdateHealth();
            Despawn(other.gameObject);
            print(hp.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowGameOver()
    {
        RpcShowGameOver();
    }

    [ObserversRpc]
    private void RpcShowGameOver()
    {
        gameOverMenu.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealth()
    {
        UpdateHealthText();
    }

    [ObserversRpc]
    public void UpdateHealthText()
    {
        hp.Value -= 1;
        hpText.text = hp.Value.ToString();

        if (hp.Value <= 0)
        {
            hp.Value = 0;
        }
    }
}
