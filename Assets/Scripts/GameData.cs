using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    private const int sessionResultLimit = 7;

    [System.Serializable]
    public struct SessionResult : System.IComparable<SessionResult> {
        private string playerName;
        private int score;
        private int moves;
        private System.TimeSpan time;

        public SessionResult(string newPlayerName, int newScore, int newMoves, System.TimeSpan newTime) {
            playerName = newPlayerName;
            score = newScore;
            moves = newMoves;
            time = newTime;
        }

        public int CompareTo(SessionResult sessionResult) {
            return this.score.CompareTo(sessionResult.score);
        }

        public string GetPlayerName() { return playerName; }
        public int GetScore() { return score; }
        public int GetMoves() { return moves; }
        public System.TimeSpan GetTime() { return time; }
    }

    [System.Serializable]
    public struct RunningSession {
        private int moves;
        private float elapsedTime;
        private List<int> groupsLocked;
        private List<CardData> cardDatas;

        public RunningSession(int newMoves, float newElapsedTime, List<int> newGroupsLocked) {
            moves = newMoves;
            elapsedTime = newElapsedTime;
            groupsLocked = new List<int>(newGroupsLocked);
            cardDatas = new List<CardData>();
        }

        public int GetMoves() { return moves; }
        public float GetElapsedTime() { return elapsedTime; }
        public List<int> GetGroupsLocked() { return groupsLocked; }
        public CardData GetCardData(int index) { return cardDatas[index]; }
        public int GetCardDataCount() { return cardDatas.Count; }
        public void AddCardData(CardData cardData) { cardDatas.Add(cardData); }
    }

    [System.Serializable]
    public struct CardData {
        private int cardType;
        private CardScript.CardState cardState;

        public CardData(int newCardType, CardScript.CardState newCardState) {
            cardType = newCardType;
            cardState = newCardState;
        }

        public int GetCardType() { return cardType; }
        public CardScript.CardState GetCardState() { return cardState; }
    }

    List<SessionResult> sessionResults;
    Dictionary<string, RunningSession> runningSessions;

    public GameData() {
        sessionResults = new List<SessionResult>();
        runningSessions = new Dictionary<string, RunningSession>();
    }

    public List<SessionResult> GetSessionResults() {
        return sessionResults;
    }

    public Dictionary<string, RunningSession> GetRunningSessions() {
        return runningSessions;
    }

    public void UpdateSessionResult(string newPlayerName, int newScore, int newMoves, System.TimeSpan newTime) {
        bool updated = false;
        foreach(SessionResult sessionResult in sessionResults) {
            if(sessionResult.GetPlayerName() == newPlayerName) {
                if (newScore < sessionResult.GetScore()) {
                    sessionResults.Remove(sessionResult);
                    sessionResults.Add(new SessionResult(newPlayerName, newScore, newMoves, newTime));
                    sessionResults.Sort();
                    updated = true;
                    break;
                }
                else return;
            }
        }
        if (!updated) {
            sessionResults.Add(new SessionResult(newPlayerName, newScore, newMoves, newTime));
            sessionResults.Sort();
        }
        while(sessionResults.Count > sessionResultLimit) {
            sessionResults.RemoveAt(sessionResultLimit);
        }
    }

    public void UpdateRunningSession(string newPlayerName, int newMoves, float newElapsedTime, List<int> groupsLocked){
        if (runningSessions.ContainsKey(newPlayerName)) {
            runningSessions.Remove(newPlayerName);
        }
        runningSessions.Add(newPlayerName, new RunningSession(newMoves, newElapsedTime, groupsLocked));
    }

    public void RemoveRunningSession(string playerName) {
        if (runningSessions.ContainsKey(playerName)) {
            runningSessions.Remove(playerName);
        }
    }

    public void AddCardToRunningSession(string playerName, int newCardType, CardScript.CardState newCardState) {
        if (runningSessions.ContainsKey(playerName)) {
            runningSessions[playerName].AddCardData(new CardData(newCardType, newCardState));
        }
    }
}
