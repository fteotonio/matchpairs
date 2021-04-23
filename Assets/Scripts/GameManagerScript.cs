using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManagerScript : MonoBehaviour {

    //Game Information Constants
    private const int totalTypes = 12;
    private const int typesPerSession = 12;
    private const int cardsPerGroup = 2;
    private const int scorePerTry = 5;

    //Positioning Constants
    private const int cardsPerRow = 6;
    private const int cardsPerColumn = 4;
    private const float cardBaseX = -9;
    private const float cardBaseY = 3;
    private const float cardBaseZ = -0.1f;
    private const float cardSpacingX = 2.4f;
    private const float cardSpacingY = -2.3f;

    private const int pairImagesPerRow = 3;
    private const int pairImagesPerColumn = 4;
    private const float pairImageBaseX = -130;
    private const float pairImageBaseY = 250;
    private const float pairImageBaseZ = 0;
    private const float pairImageSpacingX = 130;
    private const float pairImageSpacingY = -170;

    private const float leaderboardEntryBaseX = 0;
    private const float leaderboardEntryBaseY = 340;
    private const float leaderboardEntryBaseZ = 0;
    private const float leaderboardEntrySpacingY = -120;

    private const string cardTypeSpritePathPrefix = "Visuals/cards/sprCard";
    private const string saveGameFileSuffix = "/save.dat";

    public GameObject cardPrefab;
    public GameObject leaderboardEntryPrefab;
    public GameObject pairImagePrefab;
    public GameObject pairSidebar;

    public GameObject statsSidebar;
    public GameObject victoryScreen;
    public GameObject leaderboardScreen;

    private string playerName;

    private List<GameObject> allCards = new List<GameObject>();
    private List<GameObject> currentFlippedCards = new List<GameObject>();
    private List<GameObject> leaderboardEntries = new List<GameObject>();
    private List<GameObject> pairImages = new List<GameObject>();
    private List<int> groupsLocked;
    private int moves;
    private float elapsedSeconds;
    private bool isGameActive = false;

    private GameData currentGameData = null;

    public void setPlayerName(string newPlayerName) {
        playerName = newPlayerName;
    }

    private void Start() {
        if (typesPerSession > totalTypes) {
            Debug.LogError("TypesPerSession can't be bigger than TotalTypes.");
        }
        if (cardsPerRow * cardsPerColumn != typesPerSession * cardsPerGroup) {
            Debug.LogError("Number of cards doesn't match cards per group times type number.");
        }
        if (pairImagesPerRow * pairImagesPerColumn != typesPerSession) {
            Debug.LogError("Number of pair images doesn't match type number.");
        }
    }

    private void Update() {
        if (isGameActive)
            elapsedSeconds += Time.deltaTime;
        System.TimeSpan time = System.TimeSpan.FromSeconds(System.Math.Floor(elapsedSeconds));
        statsSidebar.GetComponent<StatsSidebarScript>().UpdateTimeText(time);
    }

    public void StartGame() {

        ClearGame();
        LoadGameData();

        if (currentGameData.GetRunningSessions().ContainsKey(playerName)) {
            if (!ResumePreviousSession(currentGameData.GetRunningSessions()[playerName]))
                StartNewSession();
        }  
        else StartNewSession();

        MakeLeaderboard();
        statsSidebar.GetComponent<StatsSidebarScript>().UpdatePlayerNameText(playerName);
        statsSidebar.GetComponent<StatsSidebarScript>().UpdateMovesText(moves);

        isGameActive = true;
    }

    private void ClearGame() {
        currentFlippedCards = new List<GameObject>();

        foreach (GameObject card in allCards) {
            Destroy(card);
        }
        allCards.Clear();

        foreach (GameObject pairImage in pairImages) {
            Destroy(pairImage);
        }
        pairImages.Clear();

        for (int ii = 0; ii < pairImagesPerColumn; ii++) {
            for (int i = 0; i < pairImagesPerRow; i++) {
                GameObject pairImage = Instantiate(pairImagePrefab);
                pairImage.transform.SetParent(pairSidebar.transform, false);
                pairImage.transform.localPosition = new Vector3(pairImageBaseX + i * pairImageSpacingX, pairImageBaseY + ii * pairImageSpacingY, pairImageBaseZ);
                pairImages.Add(pairImage);
            }
        }
    }

    private bool ResumePreviousSession(GameData.RunningSession currentSession) {
        //Check if number of cards from previous session matches current game.
        if (cardsPerColumn * cardsPerRow == currentSession.GetCardDataCount()) {
            moves = currentSession.GetMoves();
            elapsedSeconds = currentSession.GetElapsedTime();
            groupsLocked = currentSession.GetGroupsLocked();

            Dictionary<int, int> groupsToLock = new Dictionary<int, int>();

            int groupsIndex = 0;
            foreach(int cardType in groupsLocked) {
                pairImages[groupsIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>(cardTypeSpritePathPrefix + cardType);
                groupsIndex++;
            }

            //Creates cards according to information stored.
            int cardIndex = 0;
            for (int i = 0; i < cardsPerRow; i++) {
                for (int ii = 0; ii < cardsPerColumn; ii++) {

                    GameData.CardData currentCardData = currentSession.GetCardData(cardIndex);

                    GameObject card = Instantiate(
                        cardPrefab, 
                        new Vector3(cardBaseX + cardSpacingX * i, cardBaseY + cardSpacingY * ii, cardBaseZ), 
                        cardPrefab.transform.rotation
                    );

                    card.GetComponent<CardScript>().StartCard(currentCardData.GetCardType(), gameObject, currentCardData.GetCardState());

                    if (currentCardData.GetCardState() == CardScript.CardState.FLIPPED) {
                        currentFlippedCards.Add(card);
                    }
                    allCards.Add(card);

                    card.SetActive(true);

                    cardIndex++;
                }
            }
            return true;
        }
        else return false;
    }

    private void StartNewSession() {

        moves = 0;
        elapsedSeconds = 0;
        groupsLocked = new List<int>();

        System.Random random = new System.Random();

        List<int> chosenTypes = new List<int>();
        List<int> sessionCards = new List<int>();

        //Pick what card Images will appear
        while (chosenTypes.Count < typesPerSession) {
            int toAdd = random.Next(1, totalTypes+1);
            if (!chosenTypes.Contains(toAdd)) {
                chosenTypes.Add(toAdd);
            }
        }

        foreach (int n in chosenTypes) {
            for (int i = 0; i < cardsPerGroup; i++) {
                sessionCards.Add(n);
            }
        }

        //Create cards randomly according to images picked
        for (int i = 0; i < cardsPerRow; i++) {
            for (int ii = 0; ii < cardsPerColumn; ii++) {

                int chosenCard = random.Next(0, sessionCards.Count - 1);

                GameObject card = Instantiate(
                    cardPrefab, 
                    new Vector3(cardBaseX + cardSpacingX * i, cardBaseY + cardSpacingY * ii, cardBaseZ), 
                    cardPrefab.transform.rotation
                );

                card.GetComponent<CardScript>().StartCard(sessionCards[chosenCard], gameObject, CardScript.CardState.UNFLIPPED);

                sessionCards.RemoveAt(chosenCard);
                allCards.Add(card);

                card.SetActive(true);
            }
        }
    }

    private void MakeLeaderboard() {
        foreach(GameObject leaderboardEntry in leaderboardEntries) {
            leaderboardEntries.Remove(leaderboardEntry);
            Destroy(leaderboardEntry);
        }
        foreach(GameData.SessionResult gameResult in currentGameData.GetSessionResults()) {

            int index = currentGameData.GetSessionResults().IndexOf(gameResult);

            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab);
            leaderboardEntry.transform.SetParent(leaderboardScreen.transform, false);
            leaderboardEntry.transform.localPosition = new Vector3(leaderboardEntryBaseX, leaderboardEntryBaseY + leaderboardEntrySpacingY * index, leaderboardEntryBaseZ);

            leaderboardEntry.GetComponent<LeaderboardEntryScript>().UpdateEntry(
                gameResult.GetPlayerName(),
                gameResult.GetScore(),
                gameResult.GetMoves(),
                gameResult.GetTime(),
                index + 1
            );
        }
    }

    public void PauseGame(bool isPaused) {
        isGameActive = !isPaused;
    }

    public void ClickCard(GameObject card) {
        CardScript cardScript = card.GetComponent<CardScript>();
        if (currentFlippedCards.Count < cardsPerGroup && isGameActive) {
            cardScript.Flip();
            currentFlippedCards.Add(card);
            if(currentFlippedCards.Count == cardsPerGroup) {

                bool sameType = true;
                int lastType = currentFlippedCards[0].GetComponent<CardScript>().getCardType();

                foreach(GameObject currentCard in currentFlippedCards) {
                    if (currentCard.GetComponent<CardScript>().getCardType() != lastType)
                        sameType = false;
                }
                //Was a group
                if (sameType) {
                    foreach (GameObject currentCard in currentFlippedCards) {
                        currentCard.GetComponent<CardScript>().Lock();
                    }
                    groupsLocked.Add(currentFlippedCards[0].GetComponent<CardScript>().getCardType());
                    pairImages[groupsLocked.Count-1].GetComponent<Image>().sprite = Resources.Load<Sprite>(cardTypeSpritePathPrefix + groupsLocked[groupsLocked.Count-1]);
                }
                //Was not a group
                else {
                    foreach (GameObject currentCard in currentFlippedCards) {
                        currentCard.GetComponent<CardScript>().Unflip();
                    }
                }
                currentFlippedCards.Clear();
                moves++;
                statsSidebar.GetComponent<StatsSidebarScript>().UpdateMovesText(moves);

                CheckVictory();
            }
        }
    }

    public void CheckVictory() {
        if(groupsLocked.Count == typesPerSession) {
            int score = (int)System.Math.Floor(elapsedSeconds) + moves * scorePerTry;
            System.TimeSpan time = System.TimeSpan.FromSeconds(System.Math.Floor(elapsedSeconds));

            victoryScreen.GetComponent<VictoryScreenScript>().UpdateScreen(score, moves, time);
            victoryScreen.GetComponent<VictoryScreenScript>().Activate();

            isGameActive = false;
            AddSessionResult(playerName, score, moves, time);
        }
    }

    private void AddSessionResult(string playerName, int score, int moves, System.TimeSpan time) {
        currentGameData.RemoveRunningSession(playerName);
        currentGameData.UpdateSessionResult(playerName, score, moves, time);
        SaveGameData();
    }

    public void UpdateCurrentSession() {
        currentGameData.UpdateRunningSession(playerName, moves, elapsedSeconds, groupsLocked);
        foreach(GameObject card in allCards) {
            int cardType = card.GetComponent<CardScript>().getCardType();
            CardScript.CardState cardState = card.GetComponent<CardScript>().GetCardState();
            currentGameData.AddCardToRunningSession(playerName, cardType, cardState);
        }
        SaveGameData();
    }

    private void SaveGameData() {
        string destination = Application.persistentDataPath + saveGameFileSuffix;
        FileStream file;

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file, currentGameData);
        file.Close();
    }

    private void LoadGameData() {
        string destination = Application.persistentDataPath + saveGameFileSuffix;
        FileStream file;

        if (File.Exists(destination)) 
            file = File.OpenRead(destination);
        else {
            currentGameData = new GameData();
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        currentGameData = (GameData)binaryFormatter.Deserialize(file);
        file.Close();
    }
}
