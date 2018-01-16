using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;



///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class GesturesManager: Singleton<GesturesManager> {


    /// <summary>
    /// recognizes user's air tap
    /// </summary>
    private GestureRecognizer gestureRecognizer;
    const float DELAY = .5f;


    // Use this for initialization
    void Start()
    {



        // create an new gesture recognizer to detect when user taps to shoot a screenshot
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        gestureRecognizer.TappedEvent += OnTap;
        gestureRecognizer.StartCapturingGestures();


    }


    /// <summary>
    /// called whenever the user performs an air tap gesture
    /// </summary>
    /// <param name="source"> source of the interaction </param>
    /// <param name="tapCount"> count of the tap </param>
    /// <param name="headRay"> ray of the head </param>
    private void OnTap(InteractionSourceKind source, int tapCount, Ray headRay)
    {

        if (tapCount == 1)
            Invoke("SingleTap", DELAY);
        else if (tapCount == 2)
        {
            CancelInvoke("SingleTap");
            DoubleTap();
        }

    }

    void SingleTap()
    {
        System.Diagnostics.Debug.WriteLine("SingleTap was recognized.");
        // select words
    }

    void DoubleTap()
    {
        System.Diagnostics.Debug.WriteLine("DoubleTap was recognized.");
            #if (!UNITY_EDITOR)
                    Controller.Instance.TakeScreenshot(RequestCause.USERINITIATED);
            #endif
    }




}
