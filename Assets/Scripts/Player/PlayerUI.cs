using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text currentHealthText;
    [SerializeField] private Text maxHealthText;

    public Player player;

    private float currentHealth;
    private float maxHealth;

    public NetworkDamageable damageable;
    private IEnumerator UpdateUI()
    {
        while (true)
        {
            yield return new WaitUntil(() => player.baseMaxHealth != maxHealth || player.baseCurrentHealth != currentHealth);

            currentHealth = player.baseCurrentHealth;
            maxHealth = player.baseMaxHealth;

            // Update health texts.
            currentHealthText.text = ((int)currentHealth).ToString();
            maxHealthText.text = ((int)maxHealth).ToString();
        }
    }

    void Start()
    {
        StartCoroutine(UpdateUI());
    }
}
