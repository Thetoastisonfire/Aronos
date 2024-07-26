using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcGuyInteract : MonoBehaviour, Interactible //npc guy 
{   

#region imports
     [Header ("choiceBox Objects")]
    [SerializeField] private choicePanelBox choiceScript;
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private CanvasGroup choicePanel;  // CanvasGroup of the choice panel

     [Header ("Misc. Objects")]
    [SerializeField] private playerHealth player;
    [SerializeField] private DialogScript dialogueScript;
    [SerializeField] private PlayerMovement pMove;

    [Header ("Variables")]
    public Stack<(int indexer, string[] dialogue)> currentItem = new Stack<(int, string[])>();
    [SerializeField] private int botherDamage = 10;
    public bool warningGiven = false;
    private bool stopFreakingPromptingTheChoiceBoxISwearToGosh = false;
    [SerializeField] private string whichNPC = "";

    private Dictionary<int, Dictionary<int, string[]>> dialogues; //line dictionary

#endregion


#region makingDialogue
    void Awake() { //where the dictionary of lines is initialized
       dialogues = new Dictionary<int, Dictionary<int, string[]>>();

        if (whichNPC == "hoodedOne") { //if npc is hooded guy
            Debug.Log("hooded one loaded");
            dialogues[1] = new Dictionary<int, string[]> { //day 1
                { 0, new string[] //1
                    {
                        "...", //insert dark souls breathing sounds 
                        "....."
                    }
                }, 
                { 1, new string[] //2
                    {
                        "Ah, another lost soul cast into the Abyss. ",
                        "Welcome to the depths where light dares not tread. ",
                        "The surface rejected you, but... ",
                        "Here, you may find what you seek... Or be devoured by it. ",
                        "Trust is precious, down here. ",
                        "  " // dark souls laugh here
                    }
                },
                { 2, new string[] //3
                    {
                        "As for meat, you may ask me, though I am sore to part with mine. ",
                        "I would not think you a thief, I hope."
                    }
                },
                { 3, new string[] //4
                    {
                        "Oh, bother.",
                        "I grow tired of this. You are beginning to bore me."
                    }
                },
                { 10, new string[] //5 //rusty pipe
                    {
                        "Ah, I see you've acquired a piece of metal.",
                        "These stone walls stand thick and tough, but..",
                        "Perhaps this would aid your efforts to reach the hatch?",
                        "  " // dark souls laugh here
                    }
                },
                { 11, new string[] //6 //dull knife
                    {
                        "...I see you've found a rotted blade.",
                        "Blasphemous things. Can't stand them.",
                        "  ", // dark souls hmmmm here
                        "You wouldn't... Harm me with that...",
                        "..Would you?"
                    }
                },
                { 12, new string[] //7 //misshapen meat lump
                    {
                        "Ahh, a delicious chunk of flesh!", //like aahh you were by my side all along
                        "I myself, have no need of such things.",
                        "  " // dark souls laugh here
                    }
                },
                { 13, new string[] //8 //old cloth puppet
                    {
                        "...", //insert dark souls hmm  sound
                        "..Oh... My goodness.", // dark souls surprise sound
                        "This poppet... I thought it lost.",
                        "Thank you, for bringing this to me.",
                        "To think.. He was by my side, all along...",
                        "   " //relieved laugh
                    }
                },
                { 14, new string[] //9 //old tire
                    {
                        "Ha, sifting through rubbish, I see..",
                        "It seems this time, thine luck has yielded aught.",
                        "This place has the tendency to sap such things.",
                        "Why do you think I'm still here?",
                        "   " //resigned laugh
                    }
                },
                { 15, new string[] //10 //tattered hat
                    {
                        "Is that...?",
                        "How foppish of you.",
                        "They always wear it a different way..",
                        "   ", //dark chuckle
                    }
                },
                { 16, new string[] //11 //broken geometric trinket
                    {
                        "Ahh, so this too was cast aside.",
                        "They are merciless, to us.", //murmuring
                        "But even their broken failures, have promise.",
                        "Even now, it abates, I'm sure.."
                    }
                },
                { 17, new string[] //12 //cracked egg
                    {
                        "Is that...?",
                        "I thank you, for the reminder.", //hesitant but thoughtful
                        "Time plays us all, as fools.",
                        "But we must always remember what once was.",
                        "Or we too, will forget ourselves..."
                    }
                },
                { 18, new string[] //13 //cracked mask
                    {
                        "..What wretched thing is this...", //what woeful ash is this
                        "You would wish ruin upon this place?",
                        "And yet...",
                        "So... beautiful..."
                    }
                },
                { 19, new string[] //14 //weathered timepiece
                    {
                        "Hmmm, a curious relic.",
                        "Those above have long used its like",
                        "To wade through the eons untouched.",
                        "While I am made to merely wait.."
                    }
                },
                { 20, new string[] //15 //nothing!!!
                    {
                        "The well is used up.",
                        "Such is the nature of all things, in the end.",
                        "   " //sad chuckle
                    }
                },
                { 21, new string[] //16 //HATCH
                    {
                        "Oh?",
                        "You've found a way up, I see.",
                        "I see... And they would never expect a failure to..",
                        "Hmm...",
                        "If you can truly manage it..."
                    }
                },
                { 22, new string[] //17 //too much hatch
                    {
                        "I would advise caution, ambitious one.",
                        "For those above to see their judgement overthrown...",
                        "One cannot know how they will react",
                        "When their broken, remains unbroken."
                    }
                },
                { 30, new string[] //18
                    {
                        "You need not hide from me, by the way.",
                        "It doesn't work very well, regardless..",
                        "   " //mischevious chuckle
                    }
                },
                { 31, new string[] //19
                    {
                        "How frightened are you..?",
                        "There's naught to fear, little one.",
                        "Even watching is exhausting..." //huff
                    }
                },
                { 40, new string[] //20
                    {
                        "A frightened little bird",
                        "flitting around frantically..",
                        "It is hard, for new arrivals.",
                        "Just breathe, now..."
                    }
                },
                { 41, new string[] //21
                    {
                        "My my, how productive of you.",
                        "But that frail vessel will not withstand such strain..",
                        "Have you seen the nest?"
                    }
                },
                { 42, new string[] //22
                    {
                        "Ahh, to be young again.",
                        "Flitting around, desperate for survival",
                        "And yet the new day will always come.",
                        "Have you seen the nest?"
                    }
                },
                { 43, new string[] //23
                    {
                        "Ahh, to be young again.",
                        "Frightened, clawing to escape",
                        "And yet the new day will always come.",
                        "Have you seen the nest?"
                    }
                },
                { 44, new string[] //24
                    {
                        "   ", //amused chuckle
                        "Wearing thin from your scavenge, I see.",
                        "Have you seen the nest?",
                        "I have no use for it anymore."
                    }
                },
                { 45, new string[] //25
                    {
                        "Ah, the hatch, is it?",
                        "Worn so thin.. Yet braver than I.",
                        "Visit the nest... I have no use for it anymore."
                    }
                },
                { 46, new string[] //26
                    {
                        "How brave of you.",
                        "Fighting for your brave escape..",
                        "But I doubt your efficacy, worn as you are.",
                        "Visit the nest... I have no use for it anymore."
                    }
                },
                { 47, new string[] //27
                    {
                        "I can see you, you know.",
                        "Have you seen the nest? Take heed of it.",
                        "Death awaits the frightened and weak."
                    }
                },
                { 48, new string[] //28
                    {
                        "   ", //amused chuckle
                        "You're going to fall out of that crate.",
                        "I hope you've seen the nest..",
                        "It is softer than the crate, I assure you."
                    }
                },
                { 50, new string[] //29  //meat dialogue
                    {
                        "Of course.",
                        "If you would, avert your eyes for a moment.",
                        "   ", //grarr aaugghhh gmmmmrnrghrhgn //cutscene is 50,3
                        "...",
                        "There you are. Cherish it.. While you can." //like here you go
                    }
                },
                { 51, new string[] //29  //meat dialogue NO cause hungry
                    {
                        "you aint hungy enough",
                        "come back when your tummy is rubblin"
                    }
                },
                { 52, new string[] //29  //meat dialogue NO cause angry
                    {
                        "Grrr",
                        "I'm like supes mad at you rn"
                    }
                },
                { 60, new string[] //29  //hunger warning
                    {
                        "How unfortunate...",
                        "Struggling to survive, yet so frail...",
                        "My poor ward, you must eat, lest you fade away."
                    }
                },
                { 61, new string[] //29  //hunger warning but friendship is low
                    {
                        "Good.",
                        "Starve."
                    }
                },
                { 62, new string[] //29  //friendship warning
                    {
                        "You are testing my patience, now.",
                        "Begin to show gratitude for my aid,",
                        "Or I will spare you no longer."
                    }
                }
            
            };
        }
        else if (whichNPC == "triangleHeaven") { //if npc is trangle guy
            Debug.Log("triangle heaven loaded");
            dialogues[2] = new Dictionary<int, string[]> { //triangleGuyLines
                { 0, new string[] //1
                    {
                        "...", //insert dark souls breathing sounds 
                        "....."
                    }
                },
                { 1, new string[] //1
                    {
                        "...", //insert dark souls breathing sounds 
                        "....."
                    }
                },
                { 2, new string[] //1
                    {
                        "...", //insert dark souls breathing sounds 
                        "....."
                    }
                },
                { 3, new string[] //1
                    {
                        "...", //insert dark souls breathing sounds 
                        "....."
                    }
                }
            };
        }
        
    } //end of awake

    public void GetDialogue(int day, int indexer) {
        if (dialogues.ContainsKey(day) && dialogues[day].ContainsKey(indexer))
        {
                currentItem.Push((indexer, dialogues[day][indexer]));
                Debug.Log("dialog found successfully: " + day + " " + indexer);
        }
        else
        {
            Debug.Log("GetDialogue error");//dialogues[day][0]; //"..." if nothing else found
        }
    }
#endregion

#region interacting
     public void Interact() {
        if (whichNPC == "hoodedOne") { //hooded npc
            if (GlobalData.Instance.npcCounter >= 2 && !GlobalData.Instance.doingSomething) {
                GlobalData.Instance.doingSomething = true;
                if (!stopFreakingPromptingTheChoiceBoxISwearToGosh){ 
                    choiceBox.SetActive(true);
                    choiceScript.PromptChoiceBox(pMove.transform.position);
                    stopFreakingPromptingTheChoiceBoxISwearToGosh = true;
                }
            } else if (GlobalData.Instance.npcCounter < 2) InteractYes();
        } 
        else if (whichNPC == "triangleHeaven") { //triangle npc
            Debug.Log("triangle intereact");
            InteractYes();
        }
     }

    public void InteractYes(){
        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false

        if (whichNPC == "hoodedOne") {
            dialogueScript.dialogueTag = "npcGuy";
        //  player.takeDamage(10);
            if (currentItem.Count == 0) npcUpdate();
            InteractDialogueMediary();
        }
        else if (whichNPC == "triangleHeaven") {
            Debug.Log("triangle intereact yes");
            dialogueScript.dialogueTag = "npcGuy"; //just for getting dialog
            if (currentItem.Count == 0) triangleUpdate();
            else Debug.Log("not updating");

            InteractDialogueMediary();
        }
    }

    public void InteractDialogueMediary() { //does everything that the interact methods do at the end
        if (whichNPC == "hoodedOne") GlobalData.Instance.npcCounter += 1;
        GlobalData.Instance.currentlyInteracting = true;
        stopFreakingPromptingTheChoiceBoxISwearToGosh = false;
        dialogueScript.subsequentStart();
    }
    
    public void InteractMeat() {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false
        
        if (GlobalData.Instance.currentHunger > 80) {
            dialogueScript.dialogueTag = "npcGuyMeatNo";
            GetDialogue(1, 51);
        }
        else if (GlobalData.Instance.currentFriendship < 30) {
            dialogueScript.dialogueTag = "npcGuyMeatAngry";
            GetDialogue(1, 52);
        }
        else {
            dialogueScript.dialogueTag = "npcGuyMeatYes";
            GetDialogue(1, 50);
        }
        
        InteractDialogueMediary();
    }

#endregion

#region updatingDialogue
    private void npcUpdate() {
        player.takeEmotionalDamage((-botherDamage)); //gain friendship when talking to him
        switch (GlobalData.Instance.dayCount){ //current day
            case 1: //day 1
                switch (GlobalData.Instance.npcCounter) { //need at least case 0 and case 1
                    case 0: //npc counter == 0
                        GetDialogue(1, 1);  //first lines
                        break;
                    case 1:
                    case 2:
                    case 3:  //npc counter <= 3
                        GetDialogue(1, 2); //second line
                        break;
                    case 4:  //npc counter == 4
                        player.takeEmotionalDamage((botherDamage) + botherDamage);
                        GetDialogue(1, 3); //bothered line
                        break;
                    default:  //npc counter > 4
                        player.takeEmotionalDamage((botherDamage/5) + botherDamage);
                        GetDialogue(1, 0); //exhausted dialogue
                        break;
                }
                if (GlobalData.Instance.npcCounter >= 2) { //after essential dialogue, should stop duplicates
                    afterEssentialDialogue();
                } //end of extra interaction if statement
                break; //end of day 1

            default:// case 2: //day 2
        /*do I need this?*/GetDialogue(1, 0);
                afterEssentialDialogue(); //GetDialogue(1, 0);  //just the same for now
                break;
        }
    } //end of npcUpdate

    private void triangleUpdate() {
        Debug.Log("triangle updated");
        GetDialogue(2, 1);
    } 

    private void tiredDefault() { //tired logic
        if (GlobalData.Instance.junkCounter > 0 && 
          GlobalData.Instance.hatchCounter > 0 && GlobalData.Instance.hideCounter > 0) { // tired: everything
        GetDialogue(1, 40);
        }
        else if (GlobalData.Instance.junkCounter > 0 && 
            GlobalData.Instance.hatchCounter > 0) { //tired: junk and hatch
        GetDialogue(1, 41);
        }
        else if (GlobalData.Instance.junkCounter > 0 && 
            GlobalData.Instance.hideCounter > 0) { //tired: junk and hide
        GetDialogue(1, 42);
        }
        else if (GlobalData.Instance.hatchCounter > 0 && 
            GlobalData.Instance.hideCounter > 0) { //tired: hatch and hide
        GetDialogue(1, 43);
        }
        else if (GlobalData.Instance.junkCounter > 0) {
        GetDialogue(1, 44); //tired: only junk
        }
        else if (GlobalData.Instance.hatchCounter > 0) {
        if (GlobalData.Instance.hideCounter <= 3) GetDialogue(1, 45); //tired: hatch
        else GetDialogue(1, 46);  //tired: too much hatch
        }
        else if (GlobalData.Instance.hideCounter > 0) {
        if (GlobalData.Instance.hideCounter <= 3)GetDialogue(1, 47); //tired: hide
        else GetDialogue(1, 48); //tired: too much hide
        }

    }

    private void hatchAndHide() { //hide and hatch logic
        switch(GlobalData.Instance.hatchCounter) {
            case 0: break; //if none, no dialogue
            case 1: 
            case 2:
            case 3:
                GetDialogue(1, 21); //CHANGE: hatch interact dialog
                break;
            default:
                GetDialogue(1, 22); //CHANGE: hatch interact dialog
                break;
        }
        switch(GlobalData.Instance.hideCounter) {
            case 0: break; //if none, no dialogue
            case 1: 
            case 2:
            case 3:
                GetDialogue(1, 30); //CHANGE: hide interact dialog
                break;
            default:
                GetDialogue(1, 31); //CHANGE: if player hides a lot
                break;    
        } 
    }

    private void afterEssentialDialogue() { //logic for after essential dialogue
        switch(GlobalData.Instance.junkCounter) {
            case 0: break; //if none, no dialogue
            default:
            //SWITCH STATEMENT FOR WHICH JUNK IS ACCESSED
                if (!GlobalData.Instance.alreadyMentionedJunk){
                    switch(GlobalData.Instance.currentJunkItem){
                        case 1: 
                            GetDialogue(1, 10); //junk item 1
                            break;
                        case 2: 
                            GetDialogue(1, 11); //junk item 2
                            break;
                        case 3: 
                            GetDialogue(1, 12); //junk item 3
                            break;
                        case 4: 
                            GetDialogue(1, 13); //junk item 4
                            break;
                        case 5: 
                            GetDialogue(1, 14); //junk item 5
                            break;
                        case 6: 
                            GetDialogue(1, 15); //junk item
                            break;
                        case 7: 
                            GetDialogue(1, 16); //junk item
                            break;
                        case 8: 
                            GetDialogue(1, 17); //junk item
                            break;
                        case 9: 
                            GetDialogue(1, 18); //junk item
                            break;
                        case 10: 
                            GetDialogue(1, 19); //junk item 
                            Debug.Log("hitting 19");
                            break;
                        default:
                            GetDialogue(1, 20); //nothing!!!
                            break;
                    }
                    GlobalData.Instance.alreadyMentionedJunk = true;
                }
                break;
        }
        hatchAndHide(); //hide and hatch logic

        switch(GlobalData.Instance.tiredCounter){
            case 0: break; //if none, no dialogue
            default:  //separate index by 3
                tiredDefault();

                break;
        }          
    }//end of after essential

#endregion

#region miscellaneous dialogue stuff
    public void healthWarning() {
        if (warningGiven) return;

        if (GlobalData.Instance.currentHunger <= 50) {
            dialogueScript.dialogueTag = "npcGuy";
            if (GlobalData.Instance.currentFriendship > 30) {
                GetDialogue(1, 60);
                warningGiven = true;
            }
            else {
                GetDialogue(1, 61);
                warningGiven = true;
            }
        }
        else if (GlobalData.Instance.currentFriendship < 30) {
            dialogueScript.dialogueTag = "npcGuy";
            GetDialogue(1, 62);
            warningGiven = true;
        } else return;
        
        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false

        InteractDialogueMediary();
    }
#endregion
}
