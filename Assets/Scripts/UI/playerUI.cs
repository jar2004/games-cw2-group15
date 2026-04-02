using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("UI References")]
    public Slider hpSlider;
    public Slider xpSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI xpText;

    public GameObject levelUpPanel;
    public GameObject gameOverPanel;
    public GameObject mainMenu;
    public GameObject UI;


    
    [Header("Data Reference")]
    public playerStats stats; 

    bool lockCursorNextFrame;
    bool warnedMissingUIBindings;

    bool HasUIBindings()
    {
        return UI != null
            && mainMenu != null
            && hpSlider != null
            && xpSlider != null
            && hpText != null
            && xpText != null;
    }

    bool TryForwardToBoundUIInstance(string methodName)
    {
        if (HasUIBindings()) return false;

        playerUI[] allUI = FindObjectsByType<playerUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allUI.Length; i++)
        {
            playerUI candidate = allUI[i];
            if (candidate == this) continue;
            if (!candidate.HasUIBindings()) continue;

            if (methodName == nameof(startGame))
            {
                candidate.startGame();
            }
            else if (methodName == nameof(MainMenu))
            {
                candidate.MainMenu();
            }

            Debug.LogWarning($"playerUI.{methodName}: forwarded call from unbound instance '{name}' to '{candidate.name}'.");
            return true;
        }

        return false;
    }

    void Start()
    {
        stats.ResetStats();
        Time.timeScale = 1;

        MainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) TakeDamage(15);
        if (Keyboard.current.xKey.wasPressedThisFrame) GainXP(20);

        // Unity UI clicks can override cursor state this frame, so re-apply once on next frame.
        if (lockCursorNextFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lockCursorNextFrame = false;
        }
    }


    
public void UpdateUI()
    {

        hpSlider.maxValue = stats.maxHP; 
        xpSlider.maxValue = stats.xpToNextLevel;

        hpSlider.value = stats.currentHP;
        xpSlider.value = stats.currentXP;

        hpText.text = stats.currentHP + " / " + stats.maxHP;
        xpText.text = stats.currentXP + " / " + stats.xpToNextLevel;
    }

    public void TakeDamage(int damage)
    {
        stats.currentHP -= damage;
        if(stats.currentHP <= 0)
        {
            TriggerGameOver();
        }

        UpdateUI();
    }

    public void GainXP(int xp)
    {
        stats.currentXP += xp;

        if(stats.currentXP >= stats.xpToNextLevel)
        {
            stats.currentXP -= stats.xpToNextLevel; //leftover xp will carry across
            stats.currentLevel++;
            stats.xpToNextLevel = (int)math.round(stats.xpToNextLevel*1.25); //increase next level requirements

            //re-set max xp to new one and set to maxhp on ding
            xpSlider.maxValue = stats.xpToNextLevel;
            stats.currentHP = stats.maxHP;
            TriggerLevelUp();
        }

        UpdateUI();
    }

    void TriggerGameOver()
    {
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    void TriggerLevelUp()
    {
        levelUpPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void IncreaseDamage(){
        stats.damage += 5;
        Time.timeScale = 1;
        levelUpPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateUI();
    }

    public void IncreaseHP(){
        stats.maxHP += 20;
        stats.currentHP += 20;
        Time.timeScale = 1;
        levelUpPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateUI();
    }

    public void IncreaseMS(){
        stats.moveSpeed += 10;
        Time.timeScale = 1;
        levelUpPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateUI();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {

        UI.SetActive(false);
        mainMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void startGame()
    {
        if (TryForwardToBoundUIInstance(nameof(startGame))) return;

        // Apply gameplay state first so null UI refs do not block unpause/lock.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        if (mainMenu != null) mainMenu.SetActive(false);
        if (UI != null)
        {
            UI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("playerUI.startGame: UI reference is missing on this playerUI instance.");
        }

        lockCursorNextFrame = true;
        UpdateUI();
    }

    public void E()
    {
        stats.maxHP = 150;
        stats.currentHP = 150;

    }

    public void M()
    {
        stats.maxHP = 100;
        stats.currentHP = 100;

    }

    public void H()
    {
        stats.maxHP = 50;
        stats.currentHP = 50;

    }


}
