using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class GameManager : MonoBehaviour
{
    private Animator playerAnimator;
    private Animator enemyAnimator;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    PlayerUnit playerUnit;
    EnemyUnit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public APIManager apiManager; // Reference to APIManager
    public GameObject healingPrefab;
    public GameObject electroPrefab;
    public GameObject explosionPrefab;
    public GameObject starPrefab;

    public AudioSource Attack;
    public AudioSource Heal;
    public AudioSource Skill;
    public AudioSource Ultimate;
    public AudioSource EnemyAttack;
    public AudioSource EnemySkill;

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {        
        
       
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           Exit();
        }
        Canvas.ForceUpdateCanvases();
    }
    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<PlayerUnit>();
        playerAnimator = playerGO.GetComponent<Animator>();

        Vector3 playerPosition = playerGO.transform.position;
        playerPosition.y += 1f; // Increase the Y position by 2 units
        playerGO.transform.position = playerPosition;

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<EnemyUnit>();
        enemyAnimator = enemyGO.GetComponent<Animator>();

        Vector3 enemyPosition = enemyGO.transform.position;
        enemyPosition.y += 1f; // Increase the Y position by 2 units
        enemyGO.transform.position = enemyPosition;

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
       
       
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";
        playerAnimator.SetBool("PlayerAttack", true);
        Attack.Play();
        yield return new WaitForSeconds(2f);
        Attack.Stop();
        playerAnimator.SetBool("PlayerAttack", false);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";
        enemyAnimator.SetBool("Attack", true);
        EnemyAttack.Play();
        Canvas.ForceUpdateCanvases();
        yield return new WaitForSeconds(1f);
        EnemyAttack.Stop();
        enemyAnimator.SetBool("Attack", false);
        // Example logic: Enemy may use skill attack with a certain probability
        if (Random.Range(0f, 1f) < 0.5f) // Adjust probability as needed
        {
            enemyUnit.EnemySkillAttack(playerUnit); // Call skill attack method
            dialogueText.text = enemyUnit.unitName + " uses skill attack!";
            Canvas.ForceUpdateCanvases();
            enemyAnimator.SetBool("Attack", true);
            EnemySkill.Play();
            electroPrefab.SetActive(true);
            yield return new WaitForSeconds(1f);
            EnemySkill.Stop();
            enemyAnimator.SetBool("Attack", false);
            electroPrefab.SetActive(false);
            bool isDead = playerUnit.TakeDamage(enemyUnit.skillDamage);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
        else
        {
            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
       
            playerHUD.SetHP(playerUnit.currentHP);
            dialogueText.text = enemyUnit.unitName + " uses normal attack!";
           
            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }        
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
           
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(7);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel renewed strength!";
        healingPrefab.SetActive(true);
        Heal.Play();
        yield return new WaitForSeconds(2f);
        Heal.Stop();
        healingPrefab.SetActive(false);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerSkillAttack()
    {
        
        bool isDead = enemyUnit.TakeDamage(playerUnit.skillDamage);
       
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "Player uses skill!";
        playerAnimator.SetBool("PlayerAttack", true);
        Skill.Play();
        starPrefab.SetActive(true);
        yield return new WaitForSeconds(2f);
        Skill.Stop();
        playerAnimator.SetBool("PlayerAttack", false);
        starPrefab.SetActive(false);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerUltimate()
    {
      
        bool isDead = enemyUnit.TakeDamage(playerUnit.ultimateDamage);
       
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "Player uses ultimate!";
        playerAnimator.SetBool("PlayerAttack", true);
        
        explosionPrefab.SetActive(true);
        Ultimate.Play();
        yield return new WaitForSeconds(2f);
        Ultimate.Stop();
        playerAnimator.SetBool("PlayerAttack", false);
        explosionPrefab.SetActive(false);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    public void OnAttackButton()
    {
        playerAnimator.SetBool("PlayerAttack", true);
        if (state != BattleState.PLAYERTURN)
            return;
        playerAnimator.SetBool("PlayerAttack", false);
        StartCoroutine(PlayerAttack()); 
    }

    public void OnHealButton()
    {
       
        if (state != BattleState.PLAYERTURN)
            return;
        //Debug.Log("heal");
        StartCoroutine(PlayerHeal());
        
    }

    public void OnSkillButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerSkillAttack());
    }
    public void OnUltimateButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerUltimate());
    }

    public void Exit()
    {
        Application.Quit();
    }
}
