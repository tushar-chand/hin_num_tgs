﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

[System.Serializable]
public class QuestionItems
{
    public Sprite[] Question_Item;
}

public class GameManager_C09011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1, TutHand2;
    public GameObject Tut_Item1;
    public GameObject Tut_Item2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject Back_Button;

    [HideInInspector]
    public bool Is_CanClick;

    public GameObject[] CurrentQuestionItems;
    public int[] ItemsOrder;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;

    public int CurrentQuestion;
    int WrongAnsrsCount;
    public GameObject QuestionObj;
    public QuestionObjs[] QtnObjs;

    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;

    void Start()
    {
        if (Testing == true && FrameworkOff == true)
        {

            TotalQues = 6;
            Thisgamekey = "nc08071";

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

    /*bool Init;
    private void OnEnable()
    {
       
    }

     private void OnDisable()
     {
         if (!Init)
         {
             MultiLevelManager.instance.LoadProgressMaxValues(QuestionsOrder1.Length);
             Init = true;
         }
     }

     private void Update()
     {
         // TEST
         if (Input.GetKeyDown(KeyCode.S))
         {
             ShowLC();
         }
     }*/

    public void StartGame(GameInputData data)
    {
        ProgreesBar.SetActive(false);
        // btn_Back.SetActive(false);
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
            AddValueInProgress = 1 / (float)NoOfQuestionsToAsk;
            Thisgamekey = gameInputData.Key;
        }

        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 2.8f);
        LevelsHolder.gameObject.SetActive(false);
        StartCoroutine(SetActiveWithDelayCall(Tut_Item1, false, 9.6f));
        StartCoroutine(SetActiveWithDelayCall(Tut_Item2, true,9.6f));

        float _delay = 1f;
        for (int i = 0; i < Tut_Item1.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Item1.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        //for (int i = 0; i < Tut_Item2.transform.childCount; i++)
        //{
        //    iTween.ScaleFrom(Tut_Item2.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
        //    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
        //    _delay += 0.1f;
        //}

        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 2.5f);
        Invoke("EnableAnimator", 2f);
        Invoke("EnableTutNoTRaycatTarget", Sound_Intro1[VOLanguage].clip.length+Sound_Intro2[VOLanguage].clip.length+2.5f);
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
        Tut_Item2.transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
    }

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
    }
    
    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_Item2.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        Tut_Item2.transform.GetChild(0).GetComponent<PopTweenCustom>().StartAnim();
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(0, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
        PlayAudio(Sound_Intro4, LengthDelay+ LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 4f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelsHolder.gameObject.SetActive(true);
        ProgreesBar.SetActive(true);

        ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i];
            QuesKeys.Add(AddKey);
        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        LevelsHolder.gameObject.SetActive(true);

        QuestionObj.SetActive(true);

        QuestionObj.transform.Find("Item_1").transform.SetSiblingIndex(0);
        QuestionObj.transform.Find("Item_3_Answer").transform.SetSiblingIndex(2);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        QuestionOrderListtemp = new List<int>();

        TutorialObj.gameObject.SetActive(false);

        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CorrectAnsrIndex = QuestionsOrder1[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            CorrectAnsrIndex = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        for (int i = 0; i < QuestionObj.transform.childCount; i++)
        {
            QuestionObj.transform.GetChild(i).transform.GetComponent<Image>().sprite = QtnObjs[CurrentQuestion].Question_Item[3];
            QuestionObj.gameObject.SetActive(true);
        }

        ItemsOrder = RandomArray_Int(ItemsOrder);
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i] = QuestionObj.transform.GetChild(ItemsOrder[i]).gameObject;

            //CurrentQuestionItems[i] = QuestionObj.transform.GetChild(ItemsOrder[3]).gameObject;
        }

        float _delay = 0;
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            iTween.ScaleFrom(CurrentQuestionItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        Invoke("SetQuestionSprites", 1);
        Is_OkButtonPressed = false;

        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void SetQuestionSprites()
    {
        for (int i = 0; i < QuestionObj.transform.childCount; i++)
        {
            QuestionObj.transform.GetChild(i).transform.GetComponent<Image>().sprite = QtnObjs[CurrentQuestion].Question_Item[i];
        }

        PlayAudio(Sound_Cut, 0);

        int _randi = RandomNoFromList_Int(ItemsOrder);
        QuestionObj.transform.GetChild(2).SetSiblingIndex(_randi);
        if (_randi != 2)
        { QuestionObj.transform.GetChild(_randi + 1).SetSiblingIndex(2); }
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = true;
            }
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
                break;
        }
    }

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }

    string UserAnsr;
    int UserAnseri;
    public void Check_Answer(GameObject _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex.name;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i].name == _Ansrindex.name)
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
                UserAnseri = i;
            }
            else
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }

    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;
        Is_OkButtonPressed = true;

        PlayAudio(Sound_Btn_Ok_Click, 0);
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
        }
        StopRepetedAudio();
        if (UserAnsr.Contains("Answer"))
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

            Debug.Log("CORRECT ANSWER : " + UserAnsr);
           // ProgreesBar.GetComponent<Slider>().value += 1;
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;

            float LengthDelay = PlayAppreciationVoiceOver(Sound_Btn_Ok_Click.clip.length + 0.25f) + Sound_Btn_Ok_Click.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay+0.25f);

            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 1.5f));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);

            Invoke("HighlightCorrectOption", LengthDelay);

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Questions Finished");
                SendResultFinal();
                //StartCoroutine(SetActiveWithDelayCall(ProgreesBar, false, LengthDelay + LengthDelay2 + 1.5f));
                //StartCoroutine(SetActiveWithDelayCall(Back_Button, false, LengthDelay + LengthDelay2 + 1.5f));
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3f);
            }
        }
        else
        {
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");
            }

            Debug.Log("WRONG ANSWER : " + UserAnseri);
            iTween.ShakePosition(CurrentQuestionItems[UserAnseri].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 1f);
                Invoke("HighlightCorrectOption", 0.5f);
                StopRepetedAudio();

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                   // ProgreesBar.GetComponent<Slider>().value += 1;
                    //INGAME_COMMON
                    //MultiLevelManager.instance.UpdateProgress(1, 0);
                    //INGAME_COMMON
                }

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {

                    Debug.Log("Questions Finished");
                    /*StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
                    StartCoroutine(SetActiveWithDelayCall(ProgreesBar, false, LengthDelay + 2f));
                    StartCoroutine(SetActiveWithDelayCall(Back_Button, false, LengthDelay + 2f));
                    Invoke("ShowLC", LengthDelay + 2.5f);   */
                    SendResultFinal();
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");

                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                for (int i = 0; i < CurrentQuestionItems.Length; i++)
                {
                    CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = true;
                    CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }

    void HighlightCorrectOption()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (!CurrentQuestionItems[i].name.Contains("Answer"))
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim(); ;
            }
            else
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai].clip.length;

        return ClipLength;
    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    #region INGAME_COMMON

    public void ShowLC()
    {
        if (!MultiLevelManager.instance.CheckForNextLevel())
        {
            InGameManager.instance.Activity_Finished(MultiLevelManager.instance.Total_Questions, MultiLevelManager.instance.Total_CorrectAnswers);
        }
    }

    public void BtnAct_Back()
    {
        InGameManager.instance.BtnAct_BackInGame();
    }

    #endregion INGAME_COMMON

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public QVO_AudioSource_C09011 Sound_QVO;
    public AVO_AudioSource_C09011 Sound_AVO;

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
    public AudioSource Sound_Btn_Ok_Click;
    public AudioSource Sound_Cut;

    public void PlayAudio(AudioSource _audio, float _delay)
    {
        _audio.PlayDelayed(_delay);
    }

    public IEnumerator PlayAudioAtOneShot(AudioSource _audio, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _audio.PlayOneShot(_audio.clip);
    }

    public void PlayAudio(AudioSource[] _audio, float _delay)
    {
        _audio[VOLanguage].PlayDelayed(_delay);
    }

    public void StopAudio(AudioSource[] _audio)
    {
        _audio[VOLanguage].Stop();
    }

    public void StopAudio(AudioSource _audio)
    {
        _audio.Stop();
    }

    public void PlayAudioRepeated(AudioSource _audio)
    {
        _audiotorepeat = _audio;
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }
    AudioSource _audiotorepeat;
    float QVOLength;
    IEnumerator PlayAudioRepeatedSingleCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeat.PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedSingleCall");
        }
    }

    public void PlayAudioRepeated(AudioSource[] _audio)
    {
        _audiotorepeatarray = _audio;
        StartCoroutine("PlayAudioRepeatedCall");
    }
    AudioSource[] _audiotorepeatarray;
    IEnumerator PlayAudioRepeatedCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeatarray[VOLanguage].PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        if (_audiotorepeatarray != null)
        { StopAudio(_audiotorepeatarray); }

        if (_audiotorepeat != null)
        { StopAudio(_audiotorepeat); }

        StopCoroutine("PlayAudioRepeatedCall");
        StopCoroutine("PlayAudioRepeatedSingleCall");
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

    #region RANDOM INT FROM A ARRAY
    public static int RandomNoFromList_Int(int[] _SourceList)
    {
        int _randreturnno = 0;

        int[] _templist = _SourceList;

        int _pickrandno = Random.Range(0, _SourceList.Length);

        _randreturnno = _SourceList[_pickrandno];

        return _randreturnno;
    }
    #endregion

    [System.Serializable]
    public class AVO_AudioSource_C09011
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_C09011
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
