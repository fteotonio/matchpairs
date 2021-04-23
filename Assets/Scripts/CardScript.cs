using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CardScript : MonoBehaviour {

    private const float positionSpeed = 4f;
    private const float anglesSpeed = 6f;
    private const float lockTime = 0.5f;
    private const float unflipTime = 0.8f;
    private const string cardTypePathPrefix = "Materials/Cards/CardFronts/Card";
    private const string lockedCardTypePathPrefix = "Materials/Cards/CardFrontLocks/CardLock";

    public enum CardState {UNFLIPPED, FLIPPED, LOCKED};

    public GameObject cardGlow;
    private GameObject gameManager;
    private CardState cardState;
    private int cardType;

    private Vector3 normalPosition;
    private Vector3 normalAngles;
    private Vector3 flippedPosition;
    private Vector3 flippedAngles;

    private AudioSource audioSource;

    private bool lockTimerActive = false;
    private bool unflipTimerActive = false;
    private float lockTimer;
    private float unflipTimer;

    public void StartCard(int newCardType, GameObject newGameManager, CardState newCardState) {

        audioSource = gameObject.GetComponent<AudioSource>();
        cardState = newCardState;
        cardType = newCardType;
        gameManager = newGameManager;

        normalPosition = gameObject.transform.localPosition;
        normalAngles = gameObject.transform.eulerAngles;
        flippedPosition = new Vector3(normalPosition.x, normalPosition.y, normalPosition.z - 1f);
        flippedAngles = new Vector3(normalAngles.x, normalAngles.y, normalAngles.z - 180);

        lockTimer = lockTime;
        unflipTimer = unflipTime;

        //Set correr position and card front
        if(cardState == CardState.FLIPPED) {
            transform.localPosition = flippedPosition;
            transform.eulerAngles = flippedAngles;

            Material cardFrontMaterial = Resources.Load<Material>(cardTypePathPrefix + cardType);
            gameObject.GetComponentInChildren<MeshRenderer>().material = cardFrontMaterial;
        }
        else if(cardState == CardState.LOCKED) {
            transform.eulerAngles = flippedAngles;

            Material cardFrontMaterial = Resources.Load<Material>(lockedCardTypePathPrefix + cardType);
            gameObject.GetComponentInChildren<MeshRenderer>().material = cardFrontMaterial;
        }
        else if(cardState == CardState.UNFLIPPED) {
            Material cardFrontMaterial = Resources.Load<Material>(cardTypePathPrefix + cardType);
            gameObject.GetComponentInChildren<MeshRenderer>().material = cardFrontMaterial;
        }
    }

    public CardState GetCardState() {
        return cardState;
    }
    public int getCardType() {
        return cardType;
    }

    private void Update() {

        UpdateTimers();

        Vector3 targetPosition;
        Vector3 targetAngles;

        if(cardState == CardState.LOCKED) {
            targetPosition = normalPosition;
            targetAngles = flippedAngles;
        }
        else if(cardState == CardState.FLIPPED) {
            targetPosition = flippedPosition;
            targetAngles = flippedAngles;
        }
        else {
            targetPosition = normalPosition;
            targetAngles = normalAngles;
        }

        //Move to correct position depending on state
        if(transform.localPosition != targetPosition || transform.eulerAngles != targetAngles) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, positionSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetAngles, anglesSpeed * Time.deltaTime);
        }

    }

    //Update timers used to pause card group before state transition
    private void UpdateTimers() {
        if (lockTimerActive) {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0) {
                cardState = CardState.LOCKED;

                Material cardFrontMaterial = Resources.Load<Material>(lockedCardTypePathPrefix + cardType);
                gameObject.GetComponentInChildren<MeshRenderer>().material = cardFrontMaterial;

                lockTimerActive = false;
                lockTimer = lockTime;
            }
        }

        if (unflipTimerActive) {
            unflipTimer -= Time.deltaTime;
            if (unflipTimer <= 0) {
                audioSource.Play();
                cardState = CardState.UNFLIPPED;
                unflipTimerActive = false;
                unflipTimer = unflipTime;
            }
        }
    }

    private void OnMouseEnter() {
        cardGlow.SetActive(true);
    }
    private void OnMouseExit() {
        cardGlow.SetActive(false);
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0) && cardState == CardState.UNFLIPPED)
            gameManager.GetComponent<GameManagerScript>().ClickCard(gameObject);
    }

    public void Flip() { 
        audioSource.Play(); 
        cardState = CardState.FLIPPED;
    }

    public void Unflip() { 
        unflipTimerActive = true; 
    }

    public void Lock() { 
        lockTimerActive = true; 
    }
}
