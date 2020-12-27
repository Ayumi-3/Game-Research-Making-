using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Next : MonoBehaviour
{
    public void NextOnClick()
    {
        Questionnaire.Instance.NextQuestionPanel();
    }
}
