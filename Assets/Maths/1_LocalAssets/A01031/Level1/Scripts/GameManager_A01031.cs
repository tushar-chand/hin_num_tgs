using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_A01031 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial=true;
    public GameObject TutorialObj;
    public GameObject[] TutNumberObjs;
    public GameObject TutNumber;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1,TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;

    public GameObject LevelObj;
    public GameObject LevelHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public Image[] NumberObjs;
    public Sprite[] NumSprites;

    [HideInInspector]
    public Sprite[] RandNumSprites;    

    [HideInInspector]
    public List<int> QuestionOrderList;    

    [HideInInspector]
    public bool Is_CanClick;

    public int[] QuestionOrder;
    public int QuestionOrder1;

    public List<int> MissedQuestion;
    public int QuestionOrder2;

    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;

    int WrongAnsrsCount;

    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;
    public GameObject btn_Back;

    void Start()
    {
        if(Testing == true && FrameworkOff == true)
        {
           
            TotalQues = 6;
            Thisgamekey = "na01031";

            SetTutorial(gameInputData);
        }
        /*if (Is_Tutorial)
        {
            SetTutorial();
        }
        else
        {
            SetGamePlay();
        }*/
    }

    public void StartGame(GameInputData data)
    {
        ProgreesBar.SetActive(false);
        btn_Back.SetActive(false);
        SetTutorial(data);
    }

    public void CleanUp()
    {
        // throw new System.NotImplementedException();
    }

    #region TUTORIAL

    public void SetTutorial(GameInputData gameInputData)
    {
        if (FrameworkOff == false && Testing == false)
        {
            this.gameInputData = gameInputData;
            ////////////////What the value should add in progress bar aftereach question//////////////////
            TotalQues = gameInputData.Mechanics.Count;
            //***************************************************
            AddValueInProgress = 1 / (float)TotalQues;
            Thisgamekey = gameInputData.Key;
        }

        SetQues(TotalQues, Thisgamekey);

        TutorialObj.gameObject.SetActive(true);
        LevelObj.gameObject.SetActive(false);

        PlayAudio(Sound_Intro1, 2f);

        float _delay = 0;
        for (int i = 0; i < TutNumberObjs.Length; i++)
        {
            iTween.ScaleTo(TutNumberObjs[i].gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Invoke("EnableAnimator", 6.5f);
        Invoke("EnableTutNoTRaycatTarget", 20f);
        Invoke("EnableAnimator", 13.5f);
    }

    void SetQues(int TotalQues, string Thisgamekey)
    {

        //int[] QuesTemp = new int[TotalQues];
        Ques_1 = new int[TotalQues];
       /* int j = 0;
        int random = Random.Range(0, 5);
        if (random == 0)
            j = 0;
        else if (random == 1)
            j = 2;
        else if (random == 2)
            j = 4;
        else if (random == 3)
            j = 6;
        else if (random == 4)
            j = 8;

        //Debug.Log("Picked Ques from : " + j);
        int p = 0;
        for (int i = j; i < (j + TotalQues); i++)
        {
            QuesTemp[p] = Ques_1[p];
            p++;
        }
        
        System.Array.Copy(QuesTemp, Ques_1, QuesTemp.Length);*/

        // create a list of questions being posed in the game
        List<string> QuesKeys = new List<string>();


        // Add the questions keys to the list
        for (int i = 0; i < TotalQues; i++)
        {
            // example key A05011_Q01
            string AddKey = "" + Thisgamekey + "_Q" + Ques_1[i];
            QuesKeys.Add(AddKey);
            Debug.Log("Add : " + AddKey);
        }
        // send the list of questions to initialize the 
        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

    }

    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;       
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }

    public void EnableTutNoTRaycatTarget()
    {
        TutNumber.GetComponent<Image>().raycastTarget = true;
        PlayAudioRepeated(Sound_Intro2);
    }

    public void Selected_TutNumber()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutNumber.transform.parent.GetComponent<PopTweenCustom>().StartAnim();
        TutNumber.GetComponent<Image>().raycastTarget = false;
        TutBtn_Okay.gameObject.SetActive(true);

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        StopAudio(Sound_Intro3);        
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(1, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 1);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + 3);
    }

    #endregion 

    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);

        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionOrder.Length;
        }


        if (Is_NeedRandomizedQuestions)
        { QuestionOrder = RandomArray_Int(QuestionOrder); }

        RandNumSprites = new Sprite[NumSprites.Length];

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionOrder.Length; i++)
        {
            QuestionOrderList.Add(QuestionOrder[i]);
            RandNumSprites[i] = NumSprites[QuestionOrder[i]];
        }

        StartCoroutine(SetOk_Button(false, 0f));
        Debug.Log("QuestionOrder.Length: " + QuestionOrder.Length);
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        LevelObj.gameObject.SetActive(true);
        LevelHolder.gameObject.SetActive(true);
        for (int i = 0; i < NumberObjs.Length; i++)
        {           
            NumberObjs[i].transform.localScale = Vector3.zero;
            NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
        }

        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionOrder.Length; i++)
        {
            QuestionOrderListtemp.Add(QuestionOrder[i]);
            Debug.Log("Here");
        }

        if (QuestionOrder1 < (QuestionOrder.Length))
        {
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionOrder[tempq]);
            CurrentQuestion = QuestionOrder[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionOrder[QuestionOrder1]);
            QuestionOrder1++;
        }       
        else
        if (QuestionOrder2 < (MissedQuestion.Count))
        {
            tempq = QuestionOrder2;         
            QuestionOrderListtemp.Remove(MissedQuestion[tempq]);
            CurrentQuestion = MissedQuestion[tempq];
            Debug.Log("MissedQuestion No : " + QuestionOrder2 + " A : " + MissedQuestion[QuestionOrder2]);
            QuestionOrder2++;
        }

        float _delay = 0;
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                NumberObjs[i].sprite = NumSprites[CurrentQuestion];
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_ixx];
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_iyy];
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_izz];
                    QuestionOrderListtemp.Remove(_izz);
                }
            }

            iTween.ScaleTo(NumberObjs[i].gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;            
        }

        Is_OkButtonPressed = false;

        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            NumberObjs[i].GetComponent<Image>().raycastTarget = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (_Qi)
        {
            case 0:
                QVOLength = Sound_Q0[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q0);
                break;
            case 1:
                QVOLength = Sound_Q1[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q1);
                break;
            case 2:
                QVOLength = Sound_Q2[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q2);
                break;
            case 3:
                QVOLength = Sound_Q3[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q3);
                break;
            case 4:
                QVOLength = Sound_Q4[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q4);
                break;
            case 5:
                QVOLength = Sound_Q5[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q5);
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        switch (_Ai)
        {
            case 0:
                PlayAudio(Sound_A0, _delay);
                ClipLength = Sound_A0[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudio(Sound_A1, _delay);
                ClipLength = Sound_A1[VOLanguage].clip.length;
                break;
            case 2:
                PlayAudio(Sound_A2, _delay);
                ClipLength = Sound_A2[VOLanguage].clip.length;
                break;
            case 3:
                PlayAudio(Sound_A3, _delay);
                ClipLength = Sound_A3[VOLanguage].clip.length;
                break;
            case 4:
                PlayAudio(Sound_A4, _delay);
                ClipLength = Sound_A4[VOLanguage].clip.length;
                break;
            case 5:
                PlayAudio(Sound_A5, _delay);
                ClipLength = Sound_A5[VOLanguage].clip.length;
                break;
        }
        return ClipLength;
    }

    int UserAnsr;
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (i == _Ansrindex)
            {
                NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        PlayAudio(Sound_Selection, 0);

        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption",7);
    }

    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedCall");
    }

    void HighlightOptions()
    {
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
    }
    
    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);

        for (int i = 0; i < NumberObjs.Length; i++)
        {
            NumberObjs[i].GetComponent<Image>().raycastTarget = false;
        }

        StopRepetedAudio();

        if (CorrectAnsrIndex == UserAnsr)
        {
            /// IF RIGHT ANSWER
            ValueAdd = ValueAdd + AddValueInProgress;
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.UpdateProgress(ValueAdd);
            Debug.Log("progressBar Value Add : " + ValueAdd);

            // update the ResultData in the framework
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Correct);
                Debug.Log("Add : " + AddKey + ": Correct");
            }

            Debug.Log("CORRECT ANSWER");
            if (Testing)
            {
                ProgreesBar.GetComponent<Slider>().value += 1;
            }

            WrongAnsrsCount = 0;   

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length)+ Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay+0.25f);

            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2+0.5f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2+0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + LengthDelay2+2));

            if (QuestionOrder1 < (QuestionOrder.Length))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2+2.5f);
            }
            else
            if (QuestionOrder2 < (MissedQuestion.Count))
            {
                Invoke("GenerateLevel", LengthDelay+ LengthDelay2+2.5f);
            }
            else
            {
                Debug.Log("Game Over");
                //Invoke("ShowLC", LengthDelay + LengthDelay2+2.5f);
                SendResultFinal();
            }
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
        else
        {
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");
            }

            Debug.Log("WRONG ANSWER : ");
            iTween.ShakePosition(NumberObjs[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0);           
            WrongAnsrsCount++;

            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 1f);

                Invoke("HighlightOptions", 1);

                if (!MissedQuestion.Contains(CurrentQuestion))
                {
                    MissedQuestion.Add(CurrentQuestion);
                }
                else
                {
                   

                }

                StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + 2f));

                if (QuestionOrder1 < (QuestionOrder.Length))
                {
                    Invoke("GenerateLevel", LengthDelay+2.5f);
                }
                else
                if (QuestionOrder2 < (MissedQuestion.Count))
                {
                    Invoke("GenerateLevel", LengthDelay+2.5f);
                }
                else
                {
                    Debug.Log("Game Over");
                    //Invoke("ShowLC", LengthDelay + 2.75f);
                    SendResultFinal();
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");
                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                //StartCoroutine("PlayAudioRepeatedCall");
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                for (int i = 0; i < NumberObjs.Length; i++)
                {
                    NumberObjs[i].GetComponent<Image>().raycastTarget = true;
                }              
            }

            for (int i = 0; i < NumberObjs.Length; i++)
            {
                NumberObjs[i].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
       
        StartCoroutine(SetOk_Button(false,0.25f));
    }

    public IEnumerator SetOk_Button(bool _IsSet,float _delay)
    {
        Is_CanClick=_IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);        
    }    

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    public void ShowLC()
    {
       // LCObj.gameObject.SetActive(true);
    }    

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public AudioSource[] Sound_Q0;
    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;
    public AudioSource[] Sound_Q4;
    public AudioSource[] Sound_Q5;

    public AudioSource[] Sound_A0;
    public AudioSource[] Sound_A1;
    public AudioSource[] Sound_A2;
    public AudioSource[] Sound_A3;
    public AudioSource[] Sound_A4;
    public AudioSource[] Sound_A5;

    public AudioSource[] Appriciation_Good;
    public AudioSource[] Appriciation_Excellent;
    public AudioSource[] Appriciation_Great;
    public AudioSource[] Appriciation_Nice;
    public AudioSource[] Appriciation_Splended;
    public AudioSource[] Appriciation_Weldone;

    public AudioSource Sound_CorrectAnswer;
    public AudioSource Sound_IncorrectAnswer;
    public AudioSource Sound_Selection;
    public AudioSource Sound_Ting;
    public AudioSource Sound_BtnOkClick;

    public void PlayAudio(AudioSource _audio, float _delay)
    {
        _audio.PlayDelayed(_delay);
    }

    public IEnumerator PlayAudioAtOneShot(AudioSource _audio, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _audio.PlayOneShot(_audio.clip);
    }

    public void PlayAudio(AudioSource[] _audio,float _delay)
    {
        _audio[VOLanguage].PlayDelayed(_delay);
    }

    public void StopAudio(AudioSource[] _audio)
    {
        _audio[VOLanguage].Stop();       
    }

    public void PlayAudioRepeated(AudioSource[] _audio)
    {
        _audiotorepeat = _audio;
        StartCoroutine("PlayAudioRepeatedCall");
    }

    AudioSource[] _audiotorepeat;
    float QVOLength;
    IEnumerator PlayAudioRepeatedCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeat[VOLanguage].PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        StopAudio(_audiotorepeat);
        StopCoroutine("PlayAudioRepeatedCall");
    }

    int _appri;
    public float PlayAppreciationVoiceOver(float _delay)
    {
        float ClipLength = 0;
        _appri++;
        switch (_appri)
        {
            case 0:
                PlayAudio(Appriciation_Good, _delay);
                ClipLength = Appriciation_Good[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudio(Appriciation_Great, _delay);
                ClipLength = Appriciation_Great[VOLanguage].clip.length;
                break;
            case 2:
                PlayAudio(Appriciation_Excellent, _delay);
                ClipLength = Appriciation_Excellent[VOLanguage].clip.length;
                break;
            case 3:
                PlayAudio(Appriciation_Nice, _delay);
                ClipLength = Appriciation_Nice[VOLanguage].clip.length;
                break;
            case 4:
                PlayAudio(Appriciation_Splended, _delay);
                ClipLength = Appriciation_Good[VOLanguage].clip.length;
                break;
            case 5:
                PlayAudio(Appriciation_Weldone, _delay);
                ClipLength = Appriciation_Weldone[VOLanguage].clip.length;
                break;
        }
        if (_appri >= 5)
        {
            _appri = 0;
        }
        return ClipLength;
    }
    #endregion

    #region RANDOMIZE AN ARRAY
    public static int[] RandomArray_Int(int[] _SourceArray)
    {
        for (int i = 0; i < _SourceArray.Length; i++)
        {
            int tmp = _SourceArray[i];
            int rand = Random.Range(i, _SourceArray.Length);
            _SourceArray[i] = _SourceArray[rand];
            _SourceArray[rand] = tmp;
        }
        return _SourceArray;
    }
    #endregion

    #region RANDOM INT FROM A LIST
    public static int RandomNoFromList_Int(List<int> _SourceList)
    {
        int _randreturnno = 0;

        List<int> _templist = _SourceList;

        int _pickrandno = Random.Range(0, _SourceList.Count);

        _randreturnno = _SourceList[_pickrandno];

        return _randreturnno;
    }
    #endregion

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }
}
