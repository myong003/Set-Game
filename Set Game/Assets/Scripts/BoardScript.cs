using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BoardScript : MonoBehaviour
{
    public GameObject cardOutline;
    public List<Card> selectedCards;
    public List<GameObject> selectedOutlines;

    public Card[] placeholders = new Card[21];
    public GameObject gameDeck;
    public Card[] deck = new Card[81];
    public int deckSize;
    public List<Card> board;
    public int boardSize;
    public int maxBoardSize;
    public GameObject Graveyard;

    public List<List<Card>> possibleSets;
    public Card templateCard;
    public TextMeshProUGUI timer;
    public float clock;
    public float score;
    public Text numberOfSets;
    public TextMeshProUGUI deckLeftText;


    // Start is called before the first frame update
    void Start()
    {
        selectedCards = new List<Card>();
        selectedOutlines = new List<GameObject>();
        possibleSets = new List<List<Card>>();
        deck = gameDeck.GetComponentsInChildren<Card>();
        placeholders = gameObject.GetComponentsInChildren<Card>();
        deckSize = 81;
        boardSize = 0;
        maxBoardSize = 12;
        while (board.Count < 21){
            board.Add(null);
        }
        shuffleDeck();
        fillBoard();
        UpdatePossibleSets();
    }

    void shuffleDeck(){
        int index;
        for (int i=0; i < deckSize-1; i++){
            index = Random.Range(i+1, deckSize);
            swapCards(index, i);
        }
    }

    void swapCards(int index1, int index2){
        Card temp = deck[index1];
        deck[index1] = deck[index2];
        deck[index2] = temp;
    }

    void fillBoard(){
        for (int i=0; i < board.Count; i++){
            if (deckSize == 0){
                return;
            }
            if (board[i] == null && boardSize < 12){
                addCard(i);
            }
        }
    }

    void addCard(int boardPosition){
        moveCard(ref deck[deckSize-1], boardPosition);
        boardSize++;
        deckSize--;
        deckLeftText.text = "Deck: " + deckSize;
    }

    void RemoveCard(Card c){
        Debug.Log("Removing " + c.name);
        board[c.position] = null;
        c.position = -2;
        c.transform.position = Graveyard.transform.position;
        boardSize--;
    }

    public void IncreaseBoard(){
        if (boardSize < 21 && deckSize > 0){
            Camera.main.orthographicSize += 1.6f;
            Camera.main.transform.position += new Vector3(3f, -1.25f, 0);
            addCard(boardSize);
            addCard(boardSize);
            addCard(boardSize);
            maxBoardSize += 3;
            UpdatePossibleSets();
            HideNumberOfSets();
        }
    }

    void DecreaseBoard(){
        Camera.main.orthographicSize -= 1.6f;
        Camera.main.transform.position -= new Vector3(3f, -1.25f, 0);
        maxBoardSize -= 3;
    }

    void moveCard(ref Card c, int position){
        Debug.Log("Moving " + c.name + " to position " + position);
        if (c.position != -1){
            board[c.position] = null;
        }
        c.position = position;
        c.transform.position = placeholders[position].transform.position;
        board[position] = c;
    }

    void UpdateTimer(){
        clock = (float)Mathf.Round(Time.timeSinceLevelLoad * 10f) / 10f;
        timer.text = clock + "";
    }

    GameObject checkObjectClicked(){
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null){
                return hit.collider.gameObject;
            }
            return null;
        }
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        GameObject ob = checkObjectClicked();
        if (ob != null){
            Card selectedCard = getBoardCard(ref ob);
            if (selectedCard != null){
                Debug.Log("Clicked on a card");
                SelectCard(ref selectedCard);
            }
            else{
                Debug.Log("Not a card");
            }
        }
        UpdateTimer();
    }

    void SelectCard(ref Card selectedCard){
        if (!selectedCards.Contains(selectedCard)){
            if (selectedCards.Count < 3) {
                selectedCards.Add(selectedCard);
                selectedOutlines.Add(Instantiate(cardOutline, selectedCard.transform.position + new Vector3(0, 0, -5), Quaternion.identity));
                if (selectedCards.Count == 3){
                    StartCoroutine(waitSet());
                }
            }
        }
        else{
            int i = 0;
            while (i < 3 && selectedCards[i].transform.position != selectedCard.transform.position){
                i++;
            }
            selectedCards.RemoveAt(i);
            Destroy(selectedOutlines[i]);
            selectedOutlines.RemoveAt(i);
        }
    }

    Card getBoardCard(ref GameObject ob){
        for (int i=0; i < board.Count; i++){
            if (board[i] != null && board[i].name == ob.name){
                return board[i];
            }
        }
        return null;
    }

    IEnumerator waitSet(){
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Clearing selected");
        for (int i=0; i < selectedOutlines.Count; i++){
            Destroy(selectedOutlines[i]);
        }
        if (checkIfSet(ref selectedCards)){
            Debug.Log("Set");
            settleBoard();
            fillBoard();
            UpdatePossibleSets();
            HideNumberOfSets();
        }
        else{
            Debug.Log("No set");
        }
        selectedCards.Clear();
        selectedOutlines.Clear();
    }

    void settleBoard(){
        foreach (Card c in selectedCards){
            RemoveCard(c);
        }
        if (maxBoardSize > 12){
            Debug.Log("Settling board");
            List<Card> outCards = new List<Card>();
            int countIndex = maxBoardSize-1;
            while (countIndex > maxBoardSize-4){
                if (board[countIndex] != null){
                    outCards.Add(board[countIndex]);
                }
                countIndex--;
            }
            int j=0;
            int i=0;
            while (i < maxBoardSize && j < outCards.Count){
                if (board[i] == null){
                    Card c = outCards[j];
                    moveCard(ref c, i);
                    j++;
                }
                i++;
            }
            DecreaseBoard();
        }
        else if(boardSize == 0 && deckSize == 0){
            EndGame();
        }
    }

    public void RevealNumberOfSets(){
        if (possibleSets.Count == 0 && deckSize == 0){
            EndGame();
        }
        else{
            numberOfSets.text = possibleSets.Count + " sets";
        }
    }

    void HideNumberOfSets(){
        numberOfSets.text = "Number of Sets";
    }

    void EndGame(){
        PlayerData.time = clock;
        SceneManager.LoadScene(2);
    }

    public void HighlistSet(){
        if (selectedCards.Count == 0){
            if (possibleSets.Count > 0){
                Card first = possibleSets[Random.Range(0, possibleSets.Count)][Random.Range(0, 3)];
                SelectCard(ref first);
            }
        }
    }

    void UpdatePossibleSets(){
        possibleSets.Clear();
        Card card1;
        Card card2;
        Card card3;
        Card setCard;
        int max = boardSize;
        if (boardSize < 12){
            max = 12;
        }
        for (int i=0; i < max-2; i++){
            card1 = board[i];
            if (card1 != null){
                for (int j=i+1; j < max-1; j++){
                    card2 = board[j];
                    if (card2 != null){
                        setCard = CompleteSetCard(card1, card2);
                        for (int k=j+1; k < max; k++){
                            card3 = board[k];
                            if (card3 != null && card3.equals(setCard)){
                                possibleSets.Add(new List<Card>{card1, card2, card3});
                            }
                        }
                    }
                }
            }
        }
    }

    Card CompleteSetCard(Card card1, Card card2){
        if (card1.equals(card2)){
            Debug.Log("Error: Tried to complete set from identical cards");
            return templateCard;
        }
        templateCard.color = getMissingColor(card1.color, card2.color);
        templateCard.pattern = getMissingPattern(card1.pattern, card2.pattern);
        templateCard.shape = getMissingShape(card1.shape, card2.shape);
        templateCard.num = getMissingNumber(card1.num, card2.num);
        return templateCard;
    }

    public CardColor getMissingColor(CardColor color1, CardColor color2){
        if (color1 == color2){
            return color1;
        }
        if (((color1 == CardColor.Red) && (color2 == CardColor.Purple)) || ((color1 == CardColor.Purple) && (color2 == CardColor.Red))){
            return CardColor.Green;
        }
        if (((color1 == CardColor.Green) && (color2 == CardColor.Purple)) || ((color1 == CardColor.Purple) && (color2 == CardColor.Green))){
            return CardColor.Red;
        }
        if (((color1 == CardColor.Red) && (color2 == CardColor.Green)) || ((color1 == CardColor.Green) && (color2 == CardColor.Red))){
            return CardColor.Purple;
        }
        Debug.Log("Huge color error");
        return CardColor.Red;
    }

    public CardPattern getMissingPattern(CardPattern pattern1, CardPattern pattern2){
        if (pattern1 == pattern2){
            return pattern1;
        }
        if (((pattern1 == CardPattern.Empty) && (pattern2 == CardPattern.Filled)) || ((pattern1 == CardPattern.Filled) && (pattern2 == CardPattern.Empty))){
            return CardPattern.Striped;
        }
        if (((pattern1 == CardPattern.Striped) && (pattern2 == CardPattern.Filled)) || ((pattern1 == CardPattern.Filled) && (pattern2 == CardPattern.Striped))){
            return CardPattern.Empty;
        }
        if (((pattern1 == CardPattern.Empty) && (pattern2 == CardPattern.Striped)) || ((pattern1 == CardPattern.Striped) && (pattern2 == CardPattern.Empty))){
            return CardPattern.Filled;
        }
        Debug.Log("Huge pattern error");
        return CardPattern.Empty;
    }

    public CardShape getMissingShape(CardShape shape1, CardShape shape2){
        if (shape1 == shape2){
            return shape1;
        }
        if (((shape1 == CardShape.Oval) && (shape2 == CardShape.Rhombus)) || ((shape1 == CardShape.Rhombus) && (shape2 == CardShape.Oval))){
            return CardShape.Squiggle;
        }
        if (((shape1 == CardShape.Squiggle) && (shape2 == CardShape.Rhombus)) || ((shape1 == CardShape.Rhombus) && (shape2 == CardShape.Squiggle))){
            return CardShape.Oval;
        }
        if (((shape1 == CardShape.Oval) && (shape2 == CardShape.Squiggle)) || ((shape1 == CardShape.Squiggle) && (shape2 == CardShape.Oval))){
            return CardShape.Rhombus;
        }
        Debug.Log("Huge shape error");
        return CardShape.Oval;
    }

    public int getMissingNumber(int num1, int num2){
        if (num1 == num2){
            return num1;
        }
        if ((num1 == 1 && num2 == 2) || (num1 == 2) && (num2 == 1)){
            return 3;
        }
        if ((num1 == 3 && num2 == 2) || (num1 == 2) && (num2 == 3)){
            return 1;
        }
        if ((num1 == 1 && num2 == 3) || (num1 == 3) && (num2 == 1)){
            return 2;
        }
        Debug.Log("Huge number error");
        return 0;
    }

    bool checkIfSet(ref List<Card> cards){
        if (matchingNumbers(ref cards) || unmatchingNumbers(ref cards)){
            if (matchingColors(ref cards) || unmatchingColors(ref cards)){
                if (matchingPattern(ref cards) || unmatchingPattern(ref cards)){
                    if (matchingShapes(ref cards) || unmatchingShapes(ref cards)){
                        return true;
                    }
                    else{
                        Debug.Log("Shapes don't match");
                    }
                }
                else{
                    Debug.Log("Patterns don't match");
                }
            }
            else{
                Debug.Log("Colors don't match");
            }
        }
        else{
            Debug.Log("Numbers don't match");
        }
        return false;
    }
    bool matchingNumbers(ref List<Card> cards){
        return (cards[0].num == cards[1].num && cards[0].num == cards[2].num && cards[1].num == cards[2].num);
    }
    bool unmatchingNumbers(ref List<Card> cards){
        return (cards[0].num != cards[1].num && cards[0].num != cards[2].num && cards[1].num != cards[2].num);
    }
    bool matchingColors(ref List<Card> cards){
        return (cards[0].color == cards[1].color && cards[0].color == cards[2].color && cards[1].color == cards[2].color);
    }
    bool unmatchingColors(ref List<Card> cards){
        return (cards[0].color != cards[1].color && cards[0].color != cards[2].color && cards[1].color != cards[2].color);
    }
    bool matchingPattern(ref List<Card> cards){
        return (cards[0].pattern == cards[1].pattern && cards[0].pattern == cards[2].pattern && cards[1].pattern == cards[2].pattern);
    }
    bool unmatchingPattern(ref List<Card> cards){
        return (cards[0].pattern != cards[1].pattern && cards[0].pattern != cards[2].pattern && cards[1].pattern != cards[2].pattern);
    }
    bool matchingShapes(ref List<Card> cards){
        return (cards[0].shape == cards[1].shape && cards[0].shape == cards[2].shape && cards[1].shape == cards[2].shape);
    }
    bool unmatchingShapes(ref List<Card> cards){
        return (cards[0].shape != cards[1].shape && cards[0].shape != cards[2].shape && cards[1].shape != cards[2].shape);
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene(0);
    }
}
