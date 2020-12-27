using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Previous : MonoBehaviour
{
    public void PreviousOnClick()
    {
        Questionnaire.Instance.PreviousQuestionPanel();
    }
}
