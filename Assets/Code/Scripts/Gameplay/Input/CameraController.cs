using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float AutoBankingSpeed = 10f;
    private float AutoBankingValue = 0f; 
    private float AutobankingVelocity = 0f;
    public float SmoothTime = 0.3f;
    public float AutoBankingTreshold = 2f;

    void Update()
    {
        float mouseX = InputManager.InputMap.Overworld.MouseX.ReadValue<float>();

        
        if (Mathf.Abs(mouseX) > AutoBankingTreshold)
        { 
            AutoBankingValue += mouseX > 0 ? -AutoBankingSpeed * Time.deltaTime : AutoBankingSpeed * Time.deltaTime;
            AutoBankingValue = Mathf.Clamp(AutoBankingValue, -10, 10); 
        }
        else
        {
            //AutoBankingValue = Mathf.MoveTowards(AutoBankingValue, 0, AutoBankingSpeed * Time.deltaTime);
            AutoBankingValue = Mathf.SmoothDamp(AutoBankingValue, 0, ref AutobankingVelocity, SmoothTime);
        }
            
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, AutoBankingValue);

        
    }

}
